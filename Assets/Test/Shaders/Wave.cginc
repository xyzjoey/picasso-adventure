#ifndef WAVE_CGINC
#define WAVE_CGINC

float4 _Wave1;
float4 _Wave2;
float4 _Wave3;
float4 _WaveRangeXZ;

void computeWave(float4 waveParam, float3 p, inout float3 pdelta, inout float3 tangentx, inout float3 tangentz)
{
	float3 d = normalize(float3(waveParam.x, 0, waveParam.y)); //direction xz
	float k = 2 * UNITY_PI / waveParam.w; //frequency
	float a = waveParam.z; //amplitude
	float s = sqrt(9.8 / k); //speed
	float t = _Time.y; //time

	float f = k * (dot(d.xz, p.xz) - s * t);
	float sinf = sin(f);
	float cosf = cos(f);

	pdelta += float3(a*d.x*cosf,
					a*sinf,
					a*d.z*cosf);
	tangentx += float3(1 - a * k*d.x*d.x*sinf,
						a*k*d.x*cosf,
						-a * k*d.x*d.z*sinf);
	tangentz += float3(-a * k*d.x*d.z*sinf,
						a*k*d.z*cosf,
						1 - a * k*d.z*d.z*sinf);
}

float3 uvToObjectCoord(float3 v, float3 tangentObj, float3 normalObj, float3 binormalObj)
{
	return v.z * tangentObj + v.y * normalObj + -v.z * binormalObj;
}

void vertWave(inout float3 vertex, inout float3 normal, inout float4 tangent, float3 binormal, float2 uv)
{
	float xrange = _WaveRangeXZ.x - _WaveRangeXZ.y;
	float zrange = _WaveRangeXZ.z - _WaveRangeXZ.w;
	float x = (uv.x - 0.5) * xrange + xrange/2; //[0, 1] to [RangeXZ.x, RangeXZ.y]
	float z = (uv.y - 0.5) * zrange + zrange/2; //[0, 1] to [RangeXZ.z, RangeXZ.w]

	float3 p = float3(x, 0, z);
	float3 pdelta = float3(0, 0, 0);
	float3 tangentx = float3(0, 0, 0);
	float3 tangentz = float3(0, 0, 0);

	computeWave(_Wave1, p, pdelta, tangentx, tangentz);
	computeWave(_Wave2, p, pdelta, tangentx, tangentz);
	computeWave(_Wave3, p, pdelta, tangentx, tangentz);

	vertex.xyz += uvToObjectCoord(pdelta, tangent.xyz, normal, binormal);
}

#endif