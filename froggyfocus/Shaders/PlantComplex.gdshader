shader_type spatial;
render_mode cull_disabled, depth_draw_opaque;

uniform sampler2D grass_texture : filter_linear, source_color;

uniform float alpha_scissor_threshold : hint_range(0, 1) = 0.1;
uniform float wind_scale = 1.0;
uniform vec3 frequency = vec3(1.0, 1.0, 1.0);
uniform vec3 wave = vec3(1.0, 1.0, 1.0);
uniform float time_offset = 0.0;
uniform float time_test = 0.0;
uniform float scale_test = 0.0;
uniform sampler2D character_distance_falloff_curve : hint_default_black;
uniform float character_push_strength = 1.0;

global uniform float global_wind_time;
global uniform float global_wind_scale;
global uniform vec3 global_player_position;

vec3 Rotate(vec3 vert, vec3 speed) {
	mat4 xrot = mat4(
		vec4(1.0, 0.0, 0.0, 0.0),
		vec4(0.0, cos(speed.x), -sin(speed.x), 0.0),
		vec4(0.0, sin(speed.x), cos(speed.x), 0.0),
		vec4(0.0, 0.0, 0.0, 1.0));
	
	mat4 yrot = mat4(
		vec4(cos(speed.y), 0.0, -sin(speed.y), 0.0),
		vec4(0.0, 1.0, 0.0, 0.0),
		vec4(sin(speed.y), 0.0, cos(speed.y), 0.0),
		vec4(0.0, 0.0, 0.0, 1.0));
	
	mat4 zrot = mat4(
		vec4(cos(speed.z), -sin(speed.z), 0.0, 0.0),
		vec4(sin(speed.z), cos(speed.z), 0.0, 0.0),
		vec4(0.0, 0.0, 1.0, 0.0),
		vec4(0.0, 0.0, 0.0, 1.0));
		
	return (xrot * yrot * zrot * vec4(vert, 1.0)).xyz;
}

void vertex() {
	//vec3 vertex_global = (MODEL_MATRIX * vec4(VERTEX, 1.0)).xyz; // global vertex position
	//vec3 dir = (-1.0 * normalize(vertex_global - global_player_position)) * 30.0;
	//VERTEX = Rotate(VERTEX, vec3(radians(-dir.z), 0.0, radians(-dir.x)));
	
	float time = global_wind_time + time_test + time_offset;
	float scale = (global_wind_scale + scale_test) * wind_scale;
	vec3 freq = NODE_POSITION_WORLD + vec3(time, time, time) * frequency;
	vec3 av = vec3(abs(VERTEX.x), abs(VERTEX.y), abs(VERTEX.z));
	VERTEX.x += scale * cos(freq.x + av.z) * sin(av.z * wave.x);
	VERTEX.y += scale * cos(freq.y + av.z) * sin(av.z * wave.y);
    VERTEX.z += scale * cos(freq.z + av.x) * sin(av.x * wave.z);
    NORMAL = vec3(0.0, 1.0, 0.0);
}

void fragment() {
    vec4 color = texture(grass_texture, UV);
    ALBEDO = color.rgb;
    ALPHA = color.a;
	ALPHA_SCISSOR_THRESHOLD = alpha_scissor_threshold;
	NORMAL *= FRONT_FACING ? 1.0 : -1.0;
	SPECULAR = 0.0;
}