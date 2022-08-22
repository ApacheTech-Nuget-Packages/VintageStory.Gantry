#version 330 core
#extension GL_ARB_explicit_attrib_location : enable
layout(location = 0) in vec3 vertexPos;

out vec2 fragCoord;

void main(void) 
{ 
	gl_Position = vec4(vertexPos.xy, 0, 1);
	fragCoord = (vertexPos.xy + 1.0) / 2.0;
}