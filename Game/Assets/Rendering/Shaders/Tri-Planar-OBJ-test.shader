Shader "Tri-Planar Obj" {
  Properties {
		_Side("Side", 2D) = "white" {}
		_Top("Top", 2D) = "white" {}
		_Bottom("Bottom", 2D) = "white" {}
		_SideScale("Side Scale", Float) = 2
		_TopScale("Top Scale", Float) = 2
		_BottomScale ("Bottom Scale", Float) = 2
		
		_Ramp ("Toon Ramp (RGB)", 2D) = "gray" {}
		
	    _OutlineColor ("Outline Color", Color) = (0,0,0,1)
   		_Outline ("Outline width", Range (.002, 0.03)) = .005
	}
	
	SubShader {
		Tags {
			"Queue"="Geometry"
			"IgnoreProjector"="False"
			"RenderType"="Opaque"
		}
 
		Cull Back
		ZWrite On
		
		CGPROGRAM
		#pragma surface surf ToonRamp exclude_path:prepass vertex:vert
		#pragma exclude_renderers flash
 
		sampler2D _Side, _Top, _Bottom;
		float _SideScale, _TopScale, _BottomScale;
		uniform sampler2D _Ramp;
		
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
			float3 worldPos;
			float3 worldNormal;
			float3 vertNormal;
			float3 objPos;
		};
		
		void vert (inout appdata_full v, out Input o) {
          UNITY_INITIALIZE_OUTPUT(Input,o);
          o.vertNormal = v.normal;
          o.objPos = v.vertex;
      }
			
		void surf (Input IN, inout SurfaceOutput o) {
			float3 projNormal = saturate(pow(IN.worldNormal * 1.4, 4));
			
			// SIDE X
			float3 x = tex2D(_Side, frac(IN.objPos.zy * _SideScale)) * abs(IN.vertNormal.x);
			
			// TOP / BOTTOM
			float3 y = 0;
			if (IN.vertNormal.y > 0) {
				y = tex2D(_Top, frac(IN.objPos.zx * _TopScale)) * abs(IN.vertNormal.y);
			} else {
				y = tex2D(_Bottom, frac(IN.objPos.zx * _BottomScale)) * abs(IN.vertNormal.y);
			}
			
			// SIDE Z	
			float3 z = tex2D(_Side, frac(IN.objPos.xy * _SideScale)) * abs(IN.vertNormal.z);
			
			o.Albedo = z;
			o.Albedo = lerp(o.Albedo, x, abs(IN.vertNormal.x));
			o.Albedo = lerp(o.Albedo, y, abs(IN.vertNormal.y));
			//o.Albedo = IN.vertNormal;
			//o.Albedo = float3((IN.vertNormal.x+1)/2.0,(IN.vertNormal.y+1)/2.0,(IN.vertNormal.z+1)/2.0);
			//o.Albedo = float3(1.0-IN.vertNormal.x,1.0-IN.vertNormal.y,1.0-IN.vertNormal.z);
		} 
		ENDCG
		UsePass "Toon/Basic Outline/OUTLINE"
	}
	Dependency "BaseMapShader" = "Toon/Lighted Outline"
	Fallback "Diffuse"
}