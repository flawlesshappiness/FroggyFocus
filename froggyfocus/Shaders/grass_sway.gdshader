shader_type spatial;
render_mode world_vertex_coords;

uniform float min_height = 0.0;
uniform float max_height = 10.0;

void vertex() {
	// Called for every vertex the material is visible on.
	float height_mul = clamp((VERTEX.y - min_height) / (max_height - min_height), 0, 1);
	VERTEX.x += sin(TIME + VERTEX.z * 0.1) * 1.0 * height_mul;
}

void fragment() {
	// Called for every pixel the material is visible on.
}
