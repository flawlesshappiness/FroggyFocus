shader_type spatial;
render_mode world_vertex_coords;

//render_mode ambient_light_disabled;


uniform sampler2D texture1 : source_color;
uniform sampler2D texture2 : source_color;
uniform sampler2D texture3 : source_color;
uniform sampler2D texture4 : source_color;
uniform vec4 albedo1 : source_color = vec4(1.0f);
uniform vec4 albedo2 : source_color = vec4(1.0f);
uniform vec4 albedo3 : source_color = vec4(1.0f);
uniform vec4 albedo4 : source_color = vec4(1.0f);

void fragment() {
	vec4 col1 = albedo1 * texture(texture1, UV);
	vec4 col2 = albedo2 * texture(texture2, UV);
	vec4 col3 = albedo3 * texture(texture3, UV);
	vec4 col4 = albedo4 * texture(texture4, UV);
	vec4 mix12 = mix(col1, col2, col2.a);
	vec4 mix23 = mix(mix12, col3, col3.a);
	vec4 mix34 = mix(mix23, col4, col4.a);
	vec3 col = mix34.rgb;
	ALBEDO = col;
}