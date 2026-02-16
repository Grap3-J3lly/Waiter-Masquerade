shader_type spatial;
uniform sampler2D outline_texture : repeat_disable;
uniform vec3 emission_color;
uniform float emission_strength;

render_mode cull_disabled;

void vertex() {
	// Called for every vertex the material is visible on.
	
}

void fragment() {
	// Called for every pixel the material is visible on.
	vec2 origin = UV;
	vec2 flipped_x = vec2(1.0 - UV.x, UV.y);
	
	vec3 world_normal = (INV_VIEW_MATRIX * vec4(NORMAL, 0.0)).xyz;
	
	if(world_normal.z > 0.0) {
		ALBEDO = texture(outline_texture, UV).rgb;
		ALPHA = texture(outline_texture, UV).a;
	}
	else {
		ALBEDO = texture(outline_texture, flipped_x).rgb;
		ALPHA = texture(outline_texture, flipped_x).a;
	}
	EMISSION = emission_color * emission_strength;
}

//void light() {
//	// Called for every pixel for every light affecting the material.
//	// Uncomment to replace the default light processing function with this one.
//}
