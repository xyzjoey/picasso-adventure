Shader "Unlit/BrushMain"
{
	Properties
	{
		_Color("Color", Color) = (1, 1, 1, 1)
		_MainTex("Albedo", 2D) = "white" {}

		[Space]
		_Glossiness("Smoothness", Range(0, 1)) = 0.5
		[Gamma] _Metallic("Metallic", Range(0, 1)) = 0

		[Space]
		_BumpMap("Normal Map", 2D) = "bump" {}
		_BumpScale("Scale", Float) = 1

		[Space]
		_OcclusionMap("Occlusion Map", 2D) = "white" {}
		_OcclusionStrength("Strength", Range(0, 1)) = 1

		[Space]
		_Thickness("Thickness", Range(0, 10)) = 0.05
		_Tess("Tessellation", Range(1, 32)) = 4

		[Space]
		_WaveRangeXZ("Wave UV Scale (xmin, xmax, zmin, zmax)", Vector) = (-3,3,-3,3)
		_Wave1("Wave 1 (dirX, dirZ, amplitude, wavelength)", Vector) = (1,1,0.3,5)
		_Wave2("Wave 2 (dirX, dirZ, amplitude, wavelength)", Vector) = (1,0.6,0.3,4)
		_Wave3("Wave 3 (dirX, dirZ, amplitude, wavelength)", Vector) = (1,1.3,0.3,3)

		[Space]
		_RippleWavelength("RippleWavelength", Float) = 0.7
		_RippleRadiusMax("RippleRadiusMax", Float) = 2
	}
	SubShader
	{
		Tags { "RenderType" = "Opaque" }

		CGINCLUDE
		#include "BrushMain.cginc"
		#include "Tess.cginc"
		ENDCG

		Pass
		{
			Tags { "LightMode" = "Deferred" }
			//Cull Off
			CGPROGRAM
			#pragma target 4.0
			#pragma vertex vertTess
			#pragma hull hs
			#pragma domain ds
			#pragma geometry geom
			#pragma fragment frag
			#pragma multi_compile_prepassfinal noshadowmask nodynlightmap nodirlightmap nolightmap	
			ENDCG
		}

		//Pass
		//{
		//	Tags { "LightMode" = "ShadowCaster" }
		//	CGPROGRAM
		//	#pragma target 4.0
		//	#pragma vertex vert
		//	#pragma geometry geom
		//	#pragma fragment frag
		//	#pragma multi_compile_shadowcaster noshadowmask nodynlightmap nodirlightmap nolightmap
		//	ENDCG
		//}
	}
}
