Shader "Custom/VertexLit-Chelsea" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_VColorFactor ("Vertex Color Factor", Range(0,2)) = 2.0
		_VRamp ("Vertex Color Ramp", 2D) = "white" {}

		_LightSource ("Light Source Position", Vector) = (0,0,0,1)
		_LSIntensity ("Light Source Intensity", Range(0,2)) = 1.0
		_Lit ("Lit", Color) = (1,1,1,1)
		_Shade ("Shade", Color) = (0,0,0,1)
	}
	SubShader {
	
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows vertex:vert

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		struct Input {
			float2 uv_MainTex;
			float4 color : COLOR;
			//float3 vertNormal;
			float3 viewing : TEXCOORD1;
		};

		sampler2D _MainTex;
		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		half _VColorFactor;
		sampler2D _VRamp;
		half4 _LightSource;
		half _LSIntensity;
		fixed4 _Lit;
		fixed4 _Shade;
		
		void vert (inout appdata_full v, out Input o) {
          UNITY_INITIALIZE_OUTPUT(Input,o);
          //o.vertNormal = v.normal;
          o.viewing = normalize(_LightSource - mul(_Object2World, v.vertex).xyz);
      	}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			fixed4 r = tex2D (_VRamp, IN.color.rg);
			o.Albedo = c.rgb * r * _VColorFactor;
			
			//float3 n = saturate(IN.worldNormal);
			
			float3 lightN = normalize(_LightSource);
			
			//viewDir = _WorldSpaceCameraPos.xyz - mul(_Object2World, v).xyz;
			
			//c = n.rgb;
			float d = dot(IN.viewing, o.Normal) * 0.5 + 0.5;
			fixed4 fakeLighting = lerp(_Shade, _Lit, d) * _LSIntensity;
			
			
			o.Albedo = o.Albedo * fakeLighting;

			
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a * IN.color.a;
		}
		ENDCG
		
	} 
	FallBack "Diffuse"
}
