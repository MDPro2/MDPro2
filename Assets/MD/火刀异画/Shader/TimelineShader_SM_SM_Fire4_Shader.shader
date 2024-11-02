Shader "TimelineShader/SM/SM_Fire4_Shader" {
	Properties {
		[NoScaleOffset] _MainTex ("_MainTex", 2D) = "white" {}
		[NoScaleOffset] _MainTex2 ("_MainTex2", 2D) = "white" {}
		_Transparent ("Transparent", Range(0, 1)) = 1
		_ShakePower ("ShakePower", Range(-0.2, 0.2)) = 0.01
		_ShakeSpeed ("ShakeSpeed", Float) = 2
		_ShakeLeftRight ("ShakeLeftRight", Range(-1, 1)) = 0
		_Hue ("Hue", Range(-50, 50)) = 10
		_Saturation ("Saturation", Range(-10, 10)) = 2
		_Value ("Value", Range(0, 1)) = 1
		_FireVolume ("FireVolume", Range(0, 1)) = 1
		[HideInInspector] _QueueOffset ("_QueueOffset", Float) = 0
		[HideInInspector] _QueueControl ("_QueueControl", Float) = -1
		[HideInInspector] [NoScaleOffset] unity_Lightmaps ("unity_Lightmaps", 2DArray) = "" {}
		[HideInInspector] [NoScaleOffset] unity_LightmapsInd ("unity_LightmapsInd", 2DArray) = "" {}
		[HideInInspector] [NoScaleOffset] unity_ShadowMasks ("unity_ShadowMasks", 2DArray) = "" {}
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType"="Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard
#pragma target 3.0

		sampler2D _MainTex;
		struct Input
		{
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
	Fallback "Hidden/Shader Graph/FallbackError"
	//CustomEditor "UnityEditor.ShaderGraph.GenericShaderGraphMaterialGUI"
}