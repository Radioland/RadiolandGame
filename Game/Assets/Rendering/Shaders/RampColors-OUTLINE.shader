Shader "Custom/RampColors-OUTLINE" {
	Properties {
		_MainTex ("Ramp (RGB)", 2D) = "white" {}
		
		_OutlineColor ("Outline Color", Color) = (0,0,0,1)
		_Outline ("Outline width", Range (.002, 0.03)) = .005
	}
	SubShader {
		Tags { "RenderType"="Opaque"}
		LOD 200

		CGPROGRAM
		#pragma surface surf Lambert finalcolor:rampcolor
		#include "UnityCG.cginc"

		sampler2D _MainTex;
		
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
		};
		
		void rampcolor (Input IN, SurfaceOutput o, inout fixed4 color) {
			float y = 0;
			if (IN.uv_MainTex.y < 0.25) {
				y = 0.5 / 4.0;
			} else if (IN.uv_MainTex.y < 0.5) {
				y = 1.5 / 4.0;
			} else if (IN.uv_MainTex.y < 0.75) {
				y = 2.5 / 4.0;
			} else {
				y = 3.5 / 4.0;
			}
			y = IN.uv_MainTex.y;
			
			color = saturate(color);
			
			half3 lightinghsv = RGBtoHSV(float3 (color.r, color.g, color.b));
			
			fixed4 lightcolor = color; // has lighting information... just want the base color :/
			//half3 lightcolorHSV = RGBtoHSV(_LightColor0.rgb);
			
			
			fixed lumi = Luminance(color);
			color = tex2D(_MainTex, float2(lightinghsv.z, y));
			//color = tex2D(_MainTex, float2(lumi, y));
			
			//color = float4(o.Emission.rgb,1.0);
			//color = float4(0.988,0.690,0.251,1.0);		
			//color *= 0.6;	
		}

		void surf (Input IN, inout SurfaceOutput o) {
			//o.Albedo = float3(0.0,0.0,0.0);
			o.Albedo = float3(1.0,1.0,1.0);
			//o.Albedo = float3(0.988,0.690,0.251);
			//o.Albedo = tex2D(_MainTex, IN.uv_MainTex);
			o.Alpha = 1.0;
		}
		ENDCG
		UsePass "Toon/Basic Outline/OUTLINE"
	} 
	Dependency "BaseMapShader" = "Toon/Lighted Outline"
	FallBack "Diffuse"
}
