Shader "Custom/Glass" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_AlphaLevel ("Alpha Level", Range(0,1)) = 0.0
		_NormalMap ("Normal Map", 2D) = "" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
		Blend SrcAlpha OneMinusSrcAlpha
		Tags { 
			"Queue"="Transparent"
			"RenderType"="Transparent"
			"IgnoreProjector"="True"
		}
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard alpha

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		half _AlphaLevel;
		sampler2D _NormalMap;

		struct Input {
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Normal = tex2D(_NormalMap, IN.uv_MainTex);
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = max(0.0, lerp(0.0 - _AlphaLevel, 1.0, c.a));
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
