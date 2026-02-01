shader_type spatial;

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
	// Called for every vertex the material is visible on.
}

void fragment() {
	// Called for every pixel the material is visible on.
}

//void light() {
//	// Called for every pixel for every light affecting the material.
//	// Uncomment to replace the default light processing function with this one.
//}
