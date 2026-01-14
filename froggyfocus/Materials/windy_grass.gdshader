shader_type spatial;
render_mode cull_disabled, depth_draw_opaque;

uniform sampler2D grass_texture : filter_linear, source_color;

uniform sampler2D gradient_texture : filter_linear_mipmap, source_color;
uniform float gradient_blend : hint_range(0, 1) = 0.5;
uniform float alpha_scissor_threshold : hint_range(0, 1) = 0.1;
uniform float y_multiplier = 1.0;
uniform float local_wind_scale = 1.0;

global uniform float wind_time;
global uniform float wind_scale;

void vertex() {
	float x_freq = NODE_POSITION_WORLD.x + wind_time * 1.25 + UV.y * y_multiplier;
	float z_freq = NODE_POSITION_WORLD.z + wind_time * 0.45 + UV.y * y_multiplier;
    NORMAL = vec3(0.0, 1.0, 0.0);
    VERTEX.x += wind_scale * local_wind_scale * sin(x_freq) * (1.0 - UV.y) * 0.2;
    VERTEX.z += wind_scale * local_wind_scale * cos(z_freq) * (1.0 - UV.y) * 0.15;
}

void fragment() {
    vec4 grass_sample = texture(grass_texture, UV);
    vec3 base_color = grass_sample.rgb;
    float alpha = grass_sample.a;

    vec3 gradient_color = texture(gradient_texture, vec2(UV.y, 0.5)).rgb;

    vec3 final_color = mix(base_color, base_color * gradient_color, gradient_blend);
	
	if(!FRONT_FACING){
		NORMAL *= -1.0f;
	}

    ALBEDO = final_color;
    ALPHA = alpha;
	ALPHA_SCISSOR_THRESHOLD = alpha_scissor_threshold;
}