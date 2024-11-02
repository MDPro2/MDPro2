Shader "TimelineShader/SM/SM_FogLight_Shader" {
	Properties {
		[NoScaleOffset] _MainTex ("_MainTex", 2D) = "white" {}
		_FogSpeed ("FogSpeed", Range(0, 1)) = 0.56
		_Transparency ("Transparency", Vector) = (1,1,1,1)
		[HDR] _LightBlendColor ("LightBlendColor", Vector) = (5.278032,5.278032,5.278031,0)
		_LightSpeed ("LightSpeed", Range(0, 10)) = 0.5
		_LightRemapMinMax ("LightRemapMinMax", Vector) = (0,0.5,0,0)
		_FogPower ("FogPower", Float) = 10
		[ToggleUI] _FogSmoothSwitch ("FogSmoothON", Float) = 0
		_FogSmoothStepX ("FogSmoothStepX", Range(0, 1)) = 0
		_FogSmoothStepY ("FogSmoothStepY", Range(0, 1)) = 1
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