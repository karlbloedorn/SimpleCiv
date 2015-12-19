#version 440
out vec4 out_Color;

uniform sampler2D myTexture;
in vec2 texCoord;

void main(void) {
	out_Color =  texture2D( myTexture, texCoord);
}