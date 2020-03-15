#ifndef RIPPLE_CGINC
#define RIPPLE_CGINC

#include "Wave.cginc"

float _RippleWavelength;
float _RippleRadiusMax;

float _RippleU1, _RippleU2, _RippleU3, _RippleU4, _RippleU5, _RippleU6, _RippleU7, _RippleU8, _RippleU9, _RippleU10;
float _RippleV1, _RippleV2, _RippleV3, _RippleV4, _RippleV5, _RippleV6, _RippleV7, _RippleV8, _RippleV9, _RippleV10;
float _RippleAmplitude1, _RippleAmplitude2, _RippleAmplitude3, _RippleAmplitude4, _RippleAmplitude5, _RippleAmplitude6, _RippleAmplitude7, _RippleAmplitude8, _RippleAmplitude9, _RippleAmplitude10;
float _RippleRadius1, _RippleRadius2, _RippleRadius3, _RippleRadius4, _RippleRadius5, _RippleRadius6, _RippleRadius7, _RippleRadius8, _RippleRadius9, _RippleRadius10;

void computeRipple(float3 p, inout float3 pdelta, inout float3 tangentx, inout float3 tangentz, float centerx, float centerz, float a, float radius)
{
	float dist = sqrt(pow(p.x - centerx, 2) + pow(p.z - centerz, 2));
	if (a <= 0 || radius <= 0 || dist > radius || dist > _RippleRadiusMax) return;

	a = lerp(a, 0, dist / radius); //amplitude
	float k = 2 * UNITY_PI / _RippleWavelength; //frequency
	float s = sqrt(9.8 / k); //speed
	float t = _Time.y; //time

	float f = k * (pow(p.x - centerx, 2) + pow(p.z - centerz, 2) - s * t);
	float sinf = sin(f);
	float cosf = cos(f);

	pdelta += float3(0, a*sinf, 0);
}

void vertRipple(inout float3 vertex, inout float3 normal, inout float4 tangent, float3 binormal, float2 uv)
{
	float3 p = float3(uv.x, 0, uv.y);
	float3 pdelta = float3(0, 0, 0);
	float3 tangentx = float3(0, 0, 0);
	float3 tangentz = float3(0, 0, 0);

	computeRipple(p, pdelta, tangentx, tangentz, _RippleU1, _RippleV1, _RippleAmplitude1, _RippleRadius1);
	computeRipple(p, pdelta, tangentx, tangentz, _RippleU2, _RippleV2, _RippleAmplitude2, _RippleRadius2);
	computeRipple(p, pdelta, tangentx, tangentz, _RippleU3, _RippleV3, _RippleAmplitude3, _RippleRadius3);
	computeRipple(p, pdelta, tangentx, tangentz, _RippleU4, _RippleV4, _RippleAmplitude4, _RippleRadius4);
	computeRipple(p, pdelta, tangentx, tangentz, _RippleU5, _RippleV5, _RippleAmplitude5, _RippleRadius5);
	computeRipple(p, pdelta, tangentx, tangentz, _RippleU6, _RippleV6, _RippleAmplitude6, _RippleRadius6);
	computeRipple(p, pdelta, tangentx, tangentz, _RippleU7, _RippleV7, _RippleAmplitude7, _RippleRadius7);
	computeRipple(p, pdelta, tangentx, tangentz, _RippleU8, _RippleV8, _RippleAmplitude8, _RippleRadius8);
	computeRipple(p, pdelta, tangentx, tangentz, _RippleU9, _RippleV9, _RippleAmplitude9, _RippleRadius9);
	computeRipple(p, pdelta, tangentx, tangentz, _RippleU10, _RippleV10, _RippleAmplitude10, _RippleRadius10);
		
	vertex.xyz += uvToObjectCoord(pdelta, tangent.xyz, normal, binormal);
}

#endif