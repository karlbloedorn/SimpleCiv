#version 440

uniform vec3 lineColor;
uniform float lineAlpha;

void main(void) {
	gl_FragColor = vec4(lineColor, lineAlpha);
}