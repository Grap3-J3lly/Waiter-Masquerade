shader_type spatial;
uniform sampler2D front_texture;
uniform sampler2D back_texture;

void vertex() {
	// Called for every vertex the material is visible on.
	
}

void fragment() {
	// Called for every pixel the material is visible on.
	vec2 origin = UV;
	origin.x = origin.x * 3.0;
	origin.y = origin.y * 1.9;
	
	vec3 world_normal = (INV_VIEW_MATRIX * vec4(NORMAL, 0.0)).xyz;
	
	if(world_normal.z > 0.0) {
		ALBEDO = texture(front_texture, origin).rgb;
		ALPHA = texture(front_texture, origin).a;
	} else {
		ALBEDO = texture(back_texture, origin).rgb;
		ALPHA = texture(back_texture, origin).a;
	}
}

//void light() {
//	// Called for every pixel for every light affecting the material.
//	// Uncomment to replace the default light processing function with this one.
//}
