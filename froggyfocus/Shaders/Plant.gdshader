shader_type spatial;
render_mode cull_disabled, depth_draw_opaque;

uniform sampler2D grass_texture : filter_linear, source_color;
uniform float alpha_scissor_threshold : hint_range(0, 1) = 0.1;

uniform bool wind_vertical = false;
uniform float wind_scale = 1.0;
uniform vec3 frequency = vec3(1.0, 1.0, 1.0);
uniform vec3 wave = vec3(1.0, 1.0, 1.0);

uniform float bounce_distance_max = 1.0;
uniform float bounce_settle_duration = 1.0;
uniform vec3 bounce_amount_max = vec3(0.5, 0.1, 0.5);
uniform bool dampening_y_enabled = true;
uniform bool dampening_xz_enabled = true;
uniform vec2 bounce_dampening = vec2(0.05, 0.05);

varying vec3 dir_to_vertex;

const float distance_fade_radius = 2.0;

global uniform float global_wind_time;
global uniform float global_wind_scale;
global uniform vec3 global_player_position;
global uniform float global_time_player_move;
global uniform float global_game_time;
global uniform bool setting_hide_mesh_near_player;
global uniform bool setting_hide_mesh_near_view;

vec3 vertex_wind_vertical(vec3 node_position_world, vec2 uv){
	float x_freq = node_position_world.x + global_wind_time * frequency.x + uv.y * 4.0;
	float z_freq = node_position_world.z + global_wind_time * frequency.z + uv.y * 4.0;
	float x = global_wind_scale * wind_scale * sin(x_freq) * (1.0 - uv.y) * wave.x;
	float z = global_wind_scale * wind_scale * cos(z_freq) * (1.0 - uv.y) * wave.z;
	return vec3(x, 0.0, z);
}

vec3 vertex_wind_complex(vec3 node_position_world, vec3 vertex) {
	float time = global_wind_time;
	float scale = global_wind_scale * wind_scale;
	vec3 freq = node_position_world + vec3(time, time, time) * frequency;
	vec3 av = vec3(abs(vertex.x), abs(vertex.y), abs(vertex.z));
	float x = scale * cos(freq.x + av.z) * sin(av.z * wave.x);
	float y = scale * cos(freq.y + av.z) * sin(av.z * wave.y);
	float z = scale * cos(freq.z + av.x) * sin(av.x * wave.z);
	return vec3(x, y, z);
}

vec3 vertex_wind(vec3 node_position_world, vec3 vertex, vec2 uv){
	if(wind_vertical) {
		return vertex_wind_vertical(node_position_world, uv);
	}
	else {
		return vertex_wind_complex(node_position_world, vertex);
	}
}

vec3 vertex_bounce(vec3 vertex_local) {
	float time = global_game_time;
	float mul_move_time = clamp(1.0 - abs(time - global_time_player_move) / bounce_settle_duration, 0.0, 1.0);
	float bounce_frequency = time * 20.0;
	float v_xz = max(abs(vertex_local.x), abs(vertex_local.z));
	float dampening_y = dampening_y_enabled ? clamp(0, vertex_local.y, bounce_dampening.y) / bounce_dampening.y : 1.0;
	float dampening_xz = dampening_xz_enabled ? clamp(0, v_xz, bounce_dampening.x) / bounce_dampening.x : 1.0;
	float dampening = dampening_xz * dampening_y;
	float dist_to_vertex = length(dir_to_vertex);
	float bounce_t = abs(bounce_distance_max - min(dist_to_vertex, bounce_distance_max)) / bounce_distance_max;
	vec3 bounce_amount = bounce_t * bounce_amount_max * dampening;
	float bounce_wave = (sin(bounce_frequency) + 1.0) * 0.5 * mul_move_time;
	return normalize(dir_to_vertex) * bounce_amount * bounce_wave;
}

void vertex() {
	vec3 vertex_global = (MODEL_MATRIX * vec4(VERTEX, 1.0)).xyz; // global vertex position
	dir_to_vertex = vertex_global - global_player_position; // direction from player to vertex
	
	VERTEX += vertex_bounce(VERTEX);
	VERTEX += vertex_wind(NODE_POSITION_WORLD, VERTEX, UV);
	NORMAL = vec3(0.0, 1.0, 0.0);
}

float alternate_alpha(float alpha, float dist, vec2 coord){
	float t = 1.0 - clamp(dist / distance_fade_radius, 0.0, 1.0);
	float ease_in = pow(t, 3.0);
	float m = max(1.0, float(int(30.0 * ease_in)));
	float x = coord.x;
	return mod(x, m) <= 1.0 ? alpha : 0.0;
}

float distace_fade_alpha(float alpha, mat4 inv_projection_matrix, vec2 screen_uv, vec4 fragcoord) {
	vec4 upos = inv_projection_matrix * vec4(screen_uv * 2.0 - 1.0, fragcoord.z, 1.0);
	vec3 pixel_position = upos.xyz / upos.w;
	float dist_to_pixel = setting_hide_mesh_near_view ? length(pixel_position) : distance_fade_radius;
	float dist_to_player = setting_hide_mesh_near_player ? length(dir_to_vertex) : distance_fade_radius;
	float dist = min(dist_to_pixel, dist_to_player);
	bool show = dist > distance_fade_radius;
	return show ? alpha : alternate_alpha(alpha, dist, vec2(fragcoord.x, fragcoord.y));
}

void fragment() {
	vec4 color = texture(grass_texture, UV);
	ALBEDO = color.rgb;
	ALPHA = distace_fade_alpha(color.a, INV_PROJECTION_MATRIX, SCREEN_UV, FRAGCOORD);
	ALPHA_SCISSOR_THRESHOLD = alpha_scissor_threshold;
	NORMAL *= FRONT_FACING ? 1.0 : -1.0;
	SPECULAR = 0.0;
}