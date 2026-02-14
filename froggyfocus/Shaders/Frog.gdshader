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

uniform bool clamp_diffuse_to_max = false;

uniform int cuts : hint_range(1, 8) = 3;
uniform float wrap : hint_range(-2.0f, 2.0f) = 0.0f;
uniform float steepness : hint_range(1.0f, 8.0f) = 1.0f;

uniform bool use_attenuation = true;

uniform bool use_specular = true;
uniform float specular_strength : hint_range(0.0f, 1.0f) = 1.0f;
uniform float specular_shininess : hint_range(0.0f, 32.0f) = 16.0f;
uniform sampler2D specular_map : source_color;

uniform bool use_rim = true;
uniform float rim_width : hint_range(0.0f, 16.0f) = 8.0f;
uniform vec4 rim_color : source_color = vec4(1.0f);

uniform bool use_ramp = false;
uniform sampler2D ramp : source_color;

uniform bool use_borders = false;
uniform float border_width = 0.01f;

uniform bool apply_wind = false;
uniform float y_multiplier = 1.0;
uniform float local_wind_scale = 1.0;

global uniform float global_wind_time;
global uniform float global_wind_scale;

float split_specular(float specular) {
	return step(0.5f, specular);
}

void vertex() {
	if(apply_wind) {
		float x_freq = NODE_POSITION_WORLD.x + global_wind_time * 1.25 + UV.y * y_multiplier;
		float z_freq = NODE_POSITION_WORLD.z + global_wind_time * 0.45 + UV.y * y_multiplier;
	    //NORMAL = vec3(0.0, 1.0, 0.0);
	    VERTEX.x += global_wind_scale * local_wind_scale * sin(x_freq) * (1.0 - UV.y) * 0.2;
	    VERTEX.z += global_wind_scale * local_wind_scale * cos(z_freq) * (1.0 - UV.y) * 0.15;
	}
}

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

void light() {
	// Attenuation.
	float attenuation = 1.0f;
	if (use_attenuation) {
		attenuation = ATTENUATION;
	}

	// Diffuse lighting.
	float NdotL = dot(NORMAL, LIGHT);
	float diffuse_amount = NdotL + (attenuation - 1.0) + wrap;
	//float diffuse_amount = NdotL * attenuation + wrap;
	diffuse_amount *= steepness;
	float cuts_inv = 1.0f / float(cuts);
	float diffuse_stepped = clamp(diffuse_amount + mod(1.0f - diffuse_amount, cuts_inv), 0.0f, 1.0f);

	// Calculate borders.
	float border = 0.0f;
	if (use_borders) {
		float corr_border_width = length(cross(NORMAL, LIGHT)) * border_width * steepness;
		border = step(diffuse_stepped - corr_border_width, diffuse_amount)
				 - step(1.0 - corr_border_width, diffuse_amount);
	}

	// Apply diffuse result to different styles.
	vec3 diffuse = ALBEDO.rgb * LIGHT_COLOR / PI;
	if (use_ramp) {
		diffuse *= texture(ramp, vec2(diffuse_stepped * (1.0f - border), 0.0f)).rgb;
	} else {
		diffuse *= diffuse_stepped * (1.0f - border);
	}

	if (clamp_diffuse_to_max) {
		// Clamp diffuse to max for multiple light sources.
		DIFFUSE_LIGHT = max(DIFFUSE_LIGHT, diffuse);
	} else {
		DIFFUSE_LIGHT += diffuse;
	}

	// Specular lighting.
	if (use_specular) {
		vec3 H = normalize(LIGHT + VIEW);
		float NdotH = dot(NORMAL, H);
		float specular_amount = max(pow(NdotH, specular_shininess*specular_shininess), 0.0f)
							    * texture(specular_map, UV).r
								* attenuation;
		specular_amount = split_specular(specular_amount);
		SPECULAR_LIGHT += specular_strength * specular_amount * LIGHT_COLOR;
	}

	// Simple rim lighting.
	if (use_rim) {
		float NdotV = dot(NORMAL, VIEW);
		float rim_light = pow(1.0 - NdotV, rim_width);
		SPECULAR_LIGHT += rim_light * rim_color.rgb * rim_color.a * LIGHT_COLOR / PI; //Changed DIFFUSE_LIGHT to SPECULAR_LIGHT while migrating for similar result
	}
}