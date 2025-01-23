// NOTE: Shader automatically converted from Godot Engine 4.2.1.stable.mono's StandardMaterial3D.

shader_type spatial;
render_mode unshaded, cull_back;
uniform sampler2D texture_albedo : source_color;

void fragment() {
	vec2 base_uv = SCREEN_UV;
	vec4 albedo_tex = texture(texture_albedo, base_uv);
	ALBEDO = albedo_tex.rgb;
}