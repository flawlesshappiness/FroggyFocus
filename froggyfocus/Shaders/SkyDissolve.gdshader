shader_type sky;

uniform sampler2D sky1 : source_color;
uniform sampler2D sky2 : source_color;
uniform float blend;

void sky() {
	vec3 col1 = texture(sky1, SKY_COORDS).rgb;
	vec3 col2 = texture(sky2, SKY_COORDS).rgb;
	COLOR = mix(col1, col2, clamp(blend, 0.0, 1.0));
}
