// NOTE: Shader automatically converted from Godot Engine 4.2.1.stable.mono's StandardMaterial3D.

shader_type spatial;
render_mode unshaded, cull_disabled, alpha_to_coverage, blend_add;
uniform sampler2D texture_albedo : source_color, filter_nearest;

void fragment() {
	vec2 base_uv = UV;
	vec4 albedo_tex = texture(texture_albedo, base_uv);
	ALBEDO = albedo_tex.rgb;
	ALPHA = albedo_tex.a * 2.0;
}