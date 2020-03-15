// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

#ifndef STANDARDGEOMETRY_CGINC
#define STANDARDGEOMETRY_CGINC

#include "UnityCG.cginc"
#include "UnityGBuffer.cginc"
#include "UnityStandardUtils.cginc"

// Cube map shadow caster; Used to render point light shadows on platforms
// that lacks depth cube map support.
#if defined(SHADOWS_CUBE) && !defined(SHADOWS_CUBE_IN_DEPTH_TEX)
#define PASS_CUBE_SHADOWCASTER
#endif

// Shader uniforms
half4 _Color;
sampler2D _MainTex;
float4 _MainTex_ST;

half _Glossiness;
half _Metallic;

sampler2D _BumpMap;
float _BumpScale;

sampler2D _OcclusionMap;
float _OcclusionStrength;

float _LocalTime;

struct app2vBasic
{
	float4 vertex : POSITION;
	float3 normal : NORMAL;
	float4 tangent : TANGENT;
	float2 uv : TEXCOORD;
};

// Vertex input attributes
struct v2gBasic
{
    float4 vertex : POSITION;
	float3 normal : NORMAL;
    float4 tangent : TANGENT;
	float2 uv : TEXCOORD;
};

// Fragment varyings
struct g2fBasic
{
    float4 vertexClip : SV_POSITION;

#if defined(PASS_CUBE_SHADOWCASTER)
    // Cube map shadow caster pass
    float3 shadow : TEXCOORD0;

#elif defined(UNITY_PASS_SHADOWCASTER)
    // Default shadow caster pass

#else
    // GBuffer construction pass
	float4 vertex : TEXCOORD0;
    float3 normal : TEXCOORD1;
	float4 tangent : TEXCOORD2;
	float4 binormal : TEXCOORD3;
    float2 uv : TEXCOORD4;
    half3 ambient : TEXCOORD5;

#endif
};

// vertex
void vertBasic(in app2vBasic i, out v2gBasic o)
{
	UNITY_INITIALIZE_OUTPUT(v2gBasic, o);

	o.vertex = i.vertex;
	o.normal = i.normal;
	o.tangent = i.tangent;
	o.uv = TRANSFORM_TEX(i.uv, _MainTex);
}

// geometry
void geomBasic(in v2gBasic i, out g2fBasic o)
{
	UNITY_INITIALIZE_OUTPUT(g2fBasic, o);

	float4 wpos = mul(unity_ObjectToWorld, i.vertex);
	half3 wnrm = UnityObjectToWorldNormal(i.normal);
	half4 wtan = half4(UnityObjectToWorldDir(i.tangent.xyz), i.tangent.w);

#if defined(PASS_CUBE_SHADOWCASTER)
    // Cube map shadow caster pass: Transfer the shadow vector.
    o.vertexClip = UnityWorldToClipPos(wpos);
    o.shadow = wpos.xyz - _LightPositionRange.xyz;

#elif defined(UNITY_PASS_SHADOWCASTER)
    // Default shadow caster pass: Apply the shadow bias.
    float scos = dot(wnrm, normalize(UnityWorldSpaceLightDir(wpos)));
    wpos.xyz -= wnrm * unity_LightShadowBias.z * sqrt(1 - scos * scos);
    o.vertexClip = UnityApplyLinearShadowBias(UnityWorldToClipPos(wpos));

#else
    // GBuffer construction pass
    half3 bi = cross(wnrm, wtan) * wtan.w * unity_WorldTransformParams.w;
    o.vertexClip = UnityWorldToClipPos(float4(wpos.xyz, 1));
	o.vertex = wpos;
    o.normal = wnrm;
	o.tangent = wtan;
	o.binormal = float4(bi, 1);
    o.uv = i.uv;
    o.ambient = ShadeSHPerVertex(wnrm, 0);

#endif
}

// fragment
#if defined(PASS_CUBE_SHADOWCASTER)
half4 fragBasic(g2fBasic input) : SV_Target
{
    float depth = length(input.shadow) + unity_LightShadowBias.x;
    return UnityEncodeCubeShadowDepth(depth * _LightPositionRange.w);
}
#elif defined(UNITY_PASS_SHADOWCASTER)
half4 fragBasic(g2fBasic input) : SV_Target { return 0; }
#else
void fragBasic(
	half3 albedo,
	g2fBasic input,
    out half4 outGBuffer0 : SV_Target0,
    out half4 outGBuffer1 : SV_Target1,
    out half4 outGBuffer2 : SV_Target2,
    out half4 outEmission : SV_Target3
)
{
    // Sample textures
    half4 normal = tex2D(_BumpMap, input.uv);
    normal.xyz = UnpackScaleNormal(normal, _BumpScale);

    half occlusion = tex2D(_OcclusionMap, input.uv).g;
    occlusion = LerpOneTo(occlusion, _OcclusionStrength);

    // PBS workflow conversion (metallic -> specular)
    half3 diffuse, specular;
    half refl10;
    diffuse = DiffuseAndSpecularFromMetallic(
        albedo, _Metallic, // input
        specular, refl10   // output
    );

    // Tangent space conversion (tangent space normal -> world space normal)
	float3 normalWorld = normalize(float3(
		input.tangent.x*normal.x + input.binormal.x*normal.y + input.normal.x*normal.z,
		input.tangent.y*normal.x + input.binormal.y*normal.y + input.normal.y*normal.z,
		input.tangent.z*normal.x + input.binormal.z*normal.y + input.normal.z*normal.z
    ));

    // Update the GBuffer.
    UnityStandardData data;
    data.diffuseColor = diffuse;
    data.occlusion = occlusion;
    data.specularColor = specular;
    data.smoothness = _Glossiness;
    data.normalWorld = normalWorld;
    UnityStandardDataToGbuffer(data, outGBuffer0, outGBuffer1, outGBuffer2);

    // Calculate ambient lighting and output to the emission buffer.
	half3 sh = ShadeSHPerPixel(data.normalWorld, input.ambient, input.vertex);
    outEmission = half4(sh * diffuse, 1) * occlusion;
}
#endif

#endif