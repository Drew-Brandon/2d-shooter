shader_type canvas_item;
render_mode light_only;

void light()
{
	float cNdotL = max(0.0, dot(NORMAL, LIGHT_DIRECTION));
	LIGHT = vec4(LIGHT_COLOR.rgb * COLOR.rgb * LIGHT_ENERGY * cNdotL, length(LIGHT_COLOR.rgb));;
}
