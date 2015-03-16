Shader "Custom/VertexLit-Chelsea" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_VColorFactor ("Vertex Color Factor", Range(0,2)) = 2.0
		_LightSource ("Light Source Position", Vector) = (0,0,0,1)
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

		
		
// ------------------------------------------------------------------
// HSV code from http://chilliant.blogspot.com/2010/11/rgbhsv-in-hlsl.html
float3 Hue(float H)
{
	float R = abs(H * 6 - 3) - 1;
	float G = 2 - abs(H * 6 - 2);
	float B = 2 - abs(H * 6 - 4);
	return saturate(float3(R,G,B));
}

float4 HSVtoRGB(in float3 HSV)
{
	return float4(((Hue(HSV.x) - 1) * HSV.y + 1) * HSV.z,1);
}

float3 RGBtoHSV(in float3 RGB)
{
	float3 HSV = 0;
	HSV.z = max(RGB.r, max(RGB.g, RGB.b));
	float M = min(RGB.r, min(RGB.g, RGB.b));
	float C = HSV.z - M;
	if (C != 0)
	{
		HSV.y = C / HSV.z;
		float3 Delta = (HSV.z - RGB) / C;
		Delta.rgb -= Delta.brg;
		Delta.rg += float2(2,4);

		if (RGB.r >= HSV.z)
		 HSV.x = Delta.b;
		else if (RGB.g >= HSV.z)
		 HSV.x = Delta.r;
		else
		 HSV.x = Delta.g;

		HSV.x = frac(HSV.x / 6);
	}
	return HSV;
}
// ------------------------------------------------------------------

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
		half4 _LightSource;
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
			o.Albedo = c.rgb * IN.color.rgb * _VColorFactor;
			
			//float3 n = saturate(IN.worldNormal);
			
			float3 lightN = normalize(_LightSource);
			
			//viewDir = _WorldSpaceCameraPos.xyz - mul(_Object2World, v).xyz;
			
			//c = n.rgb;
			float d = dot(IN.viewing, o.Normal) * 0.5 + 0.5;
			fixed4 fakeLighting = lerp(_Shade, _Lit, d);
			
			
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
