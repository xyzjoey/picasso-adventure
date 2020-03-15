#ifndef TESS_CGINC
#define TESS_CGINC

#include "Lighting.cginc"
#include "CustomBasic.cginc"
#include "BrushMain.cginc"

float _Tess;

v2g vertTess(app2v v)
{
	v2g o;
	o.basic.vertex = v.basic.vertex;
	o.basic.tangent = v.basic.tangent;
	o.basic.normal = v.basic.normal;
	o.basic.uv = v.basic.uv;
	o.uv2 = v.uv2;//
	o.uv3 = v.uv3;//
	o.uv4 = v.uv4;//
	o.color = v.color;//
	return o;
}

UnityTessellationFactors hsconst(InputPatch<v2g, 3> v) {
	UnityTessellationFactors o;
	float4 tf;
	tf = float4(1, 1, 1, 1) * _Tess;
	o.edge[0] = tf.x;
	o.edge[1] = tf.y;
	o.edge[2] = tf.z;
	o.inside = tf.w;
	return o;
}

[UNITY_domain("tri")]
[UNITY_partitioning("fractional_odd")]
[UNITY_outputtopology("triangle_cw")]
[UNITY_patchconstantfunc("hsconst")]
[UNITY_outputcontrolpoints(3)]
v2g hs(InputPatch<v2g, 3> v, uint id : SV_OutputControlPointID) {
	return v[id];
}

[UNITY_domain("tri")]
v2g ds(UnityTessellationFactors tessFactors, const OutputPatch<v2g, 3> vi, float3 bary : SV_DomainLocation) {
	app2v v;
	v.basic.vertex = vi[0].basic.vertex*bary.x + vi[1].basic.vertex*bary.y + vi[2].basic.vertex*bary.z;
	v.basic.tangent = vi[0].basic.tangent*bary.x + vi[1].basic.tangent*bary.y + vi[2].basic.tangent*bary.z;
	v.basic.normal = vi[0].basic.normal*bary.x + vi[1].basic.normal*bary.y + vi[2].basic.normal*bary.z;
	v.basic.uv = vi[0].basic.uv*bary.x + vi[1].basic.uv*bary.y + vi[2].basic.uv*bary.z;
	v.uv2 = vi[0].uv2*bary.x + vi[1].uv2*bary.y + vi[2].uv2*bary.z;
	v.uv3 = vi[0].uv3*bary.x + vi[1].uv3*bary.y + vi[2].uv3*bary.z;
	v.uv4 = vi[0].uv4*bary.x + vi[1].uv4*bary.y + vi[2].uv4*bary.z;
	v.color = vi[0].color*bary.x + vi[1].color*bary.y + vi[2].color*bary.z;

	//int i1, i2, i3;
	//bool isBoundary = false;
	//for (int i = 0; i < 3; ++i) {
	//	i1 = i; i2 = (i + 1) % 3; i3 = (i + 2) % 3;
	//	if (bary[i3] == 0 && vi[i1].color.w == 0 && vi[i2].color.w == 0) {
	//		isBoundary == true; 
	//		break;
	//	}
	//}
	//v.color.w = isBoundary? 0 : 1;

	return vert(v);
}

#endif