#ifndef SHADERMAIN_CGINC
// Upgrade NOTE: excluded shader from DX11, OpenGL ES 2.0 because it uses unsized arrays
#pragma exclude_renderers d3d11 gles
#define SHADERMAIN_CGINC

#include "UnityCG.cginc"
#include "CustomBasic.cginc"
#include "Wave.cginc"
#include "Ripple.cginc"

float _Thickness;

struct app2v
{
	app2vBasic basic;
	float2 uv2 : TEXCOORD1;
	float2 uv3 : TEXCOORD2;
	float2 uv4 : TEXCOORD3;
	float4 color : COLOR;
};

struct v2g
{
	v2gBasic basic;
	float2 uv2 : TEXCOORD1;
	float2 uv3 : TEXCOORD2;
	float2 uv4 : TEXCOORD3;
	float4 color : COLOR;
};

struct g2f
{
	g2fBasic basic;
	float4 color : COLOR;
	UNITY_FOG_COORDS(1)
};

//bool isBoundary(float colora)
//{ return colora == 0; }
bool isBoundary(float2 uv2)
{ return uv2.x == 0 || uv2.y == 0 || uv2.x == 1 || uv2.y == 1; }

float3 getNormal(float3 v0, float3 v1, float3 v2)
{ return normalize(cross(v1 - v0, v2 - v0)); }

float getLength(float3 v)
{
	return sqrt(v.x*v.x + v.y*v.y + v.z*v.z);
}

float3 CoordConvert(float3 v, float3 t1, float3 b1, float3 n1, float3 t2, float3 b2, float3 n2)
{
	return dot(v, t1) * t2 + dot(v, b1) * b2 + dot(v, n1) * n2;
}

v2g vert(app2v v)
{
	v2g o;
	o.uv2 = v.uv2;
	o.uv3 = v.uv3;
	o.uv4 = v.uv4;
	o.color = v.color;

	float eps = 0.005;
	float3 vertex = v.basic.vertex;
	//float3 vertexdx = vertex + float3(eps,0,0);
	//float3 vertexdz = vertex + float3(0,0, eps);
	float3 binormal = normalize(cross(v.basic.tangent.xyz, v.basic.normal));

	vertWave(vertex, v.basic.normal, v.basic.tangent, binormal, v.basic.uv);
	vertRipple(vertex, v.basic.normal, v.basic.tangent, binormal, v.basic.uv);

	//vertWave(vertexdx, v.basic.normal, v.basic.tangent, binormal, v.basic.uv + v.uv3 * eps);
	//vertWave(vertexdz, v.basic.normal, v.basic.tangent, binormal, v.basic.uv + v.uv4 * eps);
	//vertRipple(vertexdx, v.basic.normal, v.basic.tangent, binormal, v.basic.uv + v.uv3 * eps);
	//vertRipple(vertexdz, v.basic.normal, v.basic.tangent, binormal, v.basic.uv + v.uv4 * eps);

	//float3 t = normalize(vertexdx - vertex);
	//float3 b = normalize(vertexdz - vertex);
	//float3 n = normalize(cross(b, t));

	v.basic.vertex.xyz = vertex;
	//if (v.uv2.x != 0 || v.uv2.y != 0) {
	//	v.basic.tangent.xyz += normalize(uvToObjectCoord(t, v.basic.tangent.xyz, v.basic.normal, binormal));
	//	v.basic.normal.xyz += normalize(uvToObjectCoord(n, v.basic.tangent.xyz, v.basic.normal, binormal));
	//	v.basic.tangent.xyz = normalize(v.basic.tangent.xyz);
	//	v.basic.normal = normalize(v.basic.normal);
	//}
	vertBasic(v.basic, o.basic);
	return o;
}

[maxvertexcount(12)]
void geom(triangle v2g input[3], inout TriangleStream<g2f> triStream)
{
	g2f o;

	bool isBoundarys[3];
	float3 vertexUp[3];
	float3 vertexDown[3];
	float3 normal;

	for (int i = 0; i < 3; ++i) o.color = input[i].color;

	for (int i = 0; i < 3; ++i) {
		isBoundarys[i] = isBoundary(input[i].uv4);
	}

	//up vertex
	for (int i = 0; i < 3; ++i) {
		if (isBoundarys[i]) vertexUp[i] = input[i].basic.vertex.xyz;
		else vertexUp[i] = input[i].basic.vertex.xyz + input[i].basic.normal * _Thickness / 2;
	}
	//down vertex
	for (int i = 0; i < 3; ++i) {
		if (isBoundarys[i]) vertexDown[i] = vertexUp[i];
		else vertexDown[i] = input[i].basic.vertex.xyz - input[i].basic.normal * _Thickness / 2;
	}

	//up
	normal = getNormal(vertexUp[0], vertexUp[1], vertexUp[2]);
	for (int i = 0; i < 3; ++i) {
		input[i].basic.vertex.xyz = vertexUp[i];
		input[i].basic.normal = normal; //to mute

		geomBasic(input[i].basic, o.basic);
		UNITY_TRANSFER_FOG(o, o.basic.vertex);
		triStream.Append(o);
	}
	triStream.RestartStrip();

	//down
	normal = getNormal(vertexDown[2], vertexDown[1], vertexDown[0]);
	for (int i = 2; i >= 0; --i) {
		input[i].basic.vertex.xyz = vertexDown[i];
		input[i].basic.normal *= -1;
		input[i].basic.normal = normal; //to mute

		geomBasic(input[i].basic, o.basic);
		UNITY_TRANSFER_FOG(o, o.basic.vertex);
		triStream.Append(o);
	}
	triStream.RestartStrip();

	////side
	//int i1, i2;
	//float3 vertexSide[4];
	//for (int i = 0; i < 3; ++i) {
	//	i1 = i;
	//	i2 = (i + 1) % 3;
	//	if (isBoundarys[i1] && isBoundarys[i2]) {
	//		normal = getNormal(vertexUp[i1], vertexDown[i1], vertexDown[i2]);
	//		vertexSide[0] = vertexUp[i1]; 
	//		vertexSide[1] = vertexDown[i1]; 
	//		vertexSide[2] = vertexDown[i2];
	//		vertexSide[3] = vertexUp[i2];
	//		for (int i = 0; i < 3; ++i) {
	//			input[i].basic.vertex.xyz = vertexSide[i];
	//			input[i].basic.normal = normal;

	//			geomBasic(input[i].basic, o.basic);
	//			UNITY_TRANSFER_FOG(o, o.basic.vertex);
	//			triStream.Append(o);
	//		}
	//		triStream.RestartStrip();

	//		for (int i = 0; i < 3; ++i) {
	//			int j = (i + 2) % 4;
	//			input[i].basic.vertex.xyz = vertexSide[j];
	//			input[i].basic.normal = normal;

	//			geomBasic(input[i].basic, o.basic);
	//			UNITY_TRANSFER_FOG(o, o.basic.vertex);
	//			triStream.Append(o);
	//		}
	//		triStream.RestartStrip();
	//	}
	//}
}

#if defined(PASS_CUBE_SHADOWCASTER) || defined(UNITY_PASS_SHADOWCASTER)
half4 frag(g2f input) : SV_Target{ return fragBasic(input.basic); }
#else
void frag(
	g2f input,
	out half4 gbuffer0 : SV_Target0,
	out half4 gbuffer1 : SV_Target1,
	out half4 gbuffer2 : SV_Target2,
	out half4 emission : SV_Target3)
{
	//half3 albedo = input.basic.normal;
	//half3 albedo = tex2D(_MainTex, input.basic.uv).rgb * _Color.rgb * 1.4;
	half3 albedo = input.color * 1.3;
	fragBasic(albedo, input.basic, gbuffer0, gbuffer1, gbuffer2, emission);
}
#endif

#endif