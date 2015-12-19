#version 440

in vec3 in_Position;
uniform mat4 projectionMatrix;
uniform mat4 viewMatrix;
uniform mat4 modelMatrix;
out vec2 texCoord;

void main(void) {
	texCoord = in_Position.xz;
	gl_Position = projectionMatrix * viewMatrix * modelMatrix * vec4(in_Position, 1.0);
}