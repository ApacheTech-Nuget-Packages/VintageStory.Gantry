#version 330 core

// Inputs from ShaderProgram.
uniform sampler2D	iChannel0;				// Primary Scene Texture
uniform int			iColourVisionType;		// The type of colour vision profile to simulate.

uniform float		iRedBalance;			// The multiplier to add to the frag colour's Red channel.
uniform float		iGreenBalance;			// The multiplier to add to the frag colour's Green channel.
uniform float		iBlueBalance;			// The multiplier to add to the frag colour's Blue channel.
uniform float		iSaturation;			// The multiplier to add to the frag colour's Satuation.

// Inputs from Vertex Shader.
in vec2				fragCoord;				// The centred, clip-space uv coords of the fragment.

// Outputs to GPU.
out vec4			outColour;				// The final outputted colour of the fragment.

#include daltonise.ash
#include hsl.ash

void main()
{
	vec4 screenColour = texture(iChannel0, fragCoord);
	screenColour.rgb *= Daltonise[iColourVisionType];

	vec4 hsl = RGBtoHSL(vec4(
		screenColour.r * iRedBalance,
		screenColour.g * iGreenBalance,
		screenColour.b * iBlueBalance,
		screenColour.a));
		
	hsl.y = hsl.y * iSaturation;

	outColour = HSLtoRGB(hsl);
}