shader_type spatial;
render_mode cull_disabled, blend_add, unshaded;
uniform sampler2D textureAlbedo : source_color, filter_nearest;
uniform float scale;

void vertex() {
	vec3 dir = normalize(VERTEX.xyz);
	float dist = length(VERTEX.xyz);
	dist = (1.5-dist);
	dist *= 0.2f;
	VERTEX.xyz = dir * dist * scale;
	VERTEX = VERTEX;
}

void fragment() {
	vec4 color = texture(textureAlbedo, UV);
	ALBEDO = color.rgb;
	ALPHA = clamp(color.a * 0.1, 0.0f, 1.0);
}

//void light() {
	// Called for every pixel for every light affecting the material.
	// Uncomment to replace the default light processing function with this one.
//}
