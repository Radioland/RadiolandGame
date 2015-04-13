Shader "Custom/VertexLit-noRamp" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_GlossyMetalTex ("Smoothness (R) / Metallic (G)", 2D) = "white" {}
		_VColorFactor ("Vertex Color Factor", Range(0,2)) = 2.0

	}
	SubShader {
	
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		struct Input {
			float2 uv_MainTex;
			float4 color : COLOR;
			//float3 vertNormal;
		};

		fixed4 _Color;
		sampler2D _MainTex;
		half _Glossiness;
		half _Metallic;
		sampler2D _GlossyMetalTex;
		half _VColorFactor;


		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb * IN.color * _VColorFactor;

			// Metallic and smoothness come from slider variables & map
			fixed4 SM = tex2D (_GlossyMetalTex, IN.uv_MainTex);
			o.Smoothness = SM.r * _Glossiness;
			o.Metallic = SM.g * _Metallic;
			o.Alpha = c.a;
		}
		ENDCG

	} 
	FallBack "Diffuse"
}

