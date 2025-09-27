shader_type spatial;
render_mode unshaded;

uniform vec4 _albedo: source_color;
uniform mat4 cutplane1;
uniform mat4 cutplane2;
varying vec3 wvtx;

void vertex(){
	wvtx = (MODEL_MATRIX * vec4(VERTEX, 1.0)).xyz;
}

bool is_cut(mat4 plane){
	vec3 planeNormal = normalize(-plane[1].xyz);
	float planeDistance = dot(planeNormal, plane[3].xyz);
	float vertexDistance = dot(planeNormal, wvtx);
	float dist = vertexDistance - planeDistance;
	return dist < 0.0;
}

void fragment() {
	bool cut1 = is_cut(cutplane1);
	bool cut2 = is_cut(cutplane2);
	
	if(cut1 || cut2) discard;
	
	ALBEDO = _albedo.rgb;
}

//void light() {
//	// Called for every pixel for every light affecting the material.
//	// Uncomment to replace the default light processing function with this one.
//}
