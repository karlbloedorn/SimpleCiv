#version 440

in vec3 in_Position;
in vec3 in_Normal;

uniform mat4 projectionMatrix;
uniform mat4 viewMatrix;
uniform mat4 modelMatrix;
out vec3 pass_Normal;
out vec3 vertPos;

void main(void) {
	pass_Normal = in_Normal;
	gl_Position = projectionMatrix * viewMatrix * modelMatrix * vec4(in_Position, 1.0);
	vec4 vertPos4 = (viewMatrix * modelMatrix ) * vec4(in_Position, 1.0);
    vertPos = vec3(vertPos4) / vertPos4.w;
}