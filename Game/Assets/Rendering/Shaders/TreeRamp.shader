Shader "Custom/TreeBarkRamp" {
Properties {
	_MainTex ("Base (RGB)", 2D) = "white" {}
	_Ramp ("Toon Ramp (RGB)", 2D) = "gray" {}
	_Tint ("Tint Color", Color) = (1,1,1,0.5)
	
	_Scale ("Scale", Vector) = (1,1,1,1)
	_SquashAmount ("Squash", Float) = 1
	
	_OutlineColor ("Outline Color", Color) = (0,0,0,1)
	_Outline ("Outline width", Range (.002, 0.03)) = .005
}

SubShader { 
	Tags { "RenderType"="TreeBark" }
	LOD 200
	
CGPROGRAM
#pragma surface surf ToonRamp vertex:TreeVertBark addshadow nolightmap finalcolor:tint
#pragma exclude_renderers flash
#pragma glsl_no_auto_normalization
#include "Tree.cginc"

sampler2D _MainTex;
uniform sampler2D _Ramp;
fixed4 _Tint;

inline half4 LightingToonRamp (SurfaceOutput s, half3 lightDir, half atten)
{
	#ifndef USING_DIRECTIONAL_LIGHT
	lightDir = normalize(lightDir);
	#endif
	// Wrapped lighting
	half d = dot (s.Normal, lightDir) * 0.5 + 0.5;
	// Applied through ramp
	half3 ramp = tex2D (_Ramp, float2(d,d)).rgb;
	half4 c;
	c.rgb = s.Albedo * _LightColor0.rgb * ramp * (atten * 2);
	c.a = 0;
	return c;
}

struct Input {
	float2 uv_MainTex;
};

void tint (Input IN, SurfaceOutput o, inout fixed4 color) {
	color *= _LightColor0 * _Tint;
}

void surf (Input IN, inout SurfaceOutput o) {
	half4 c = tex2D (_MainTex, IN.uv_MainTex);
	o.Albedo = c.rgb;
	o.Alpha = c.a;
}
ENDCG
UsePass "Toon/Basic Outline/OUTLINE"
}
Dependency "BaseMapShader" = "Toon/Lighted Outline"
Dependency "BillboardShader" = "Hidden/Nature/Tree Creator Bark Rendertex"
Dependency "OptimizedShader" = "Custom/TreeBarkRamp"
}
