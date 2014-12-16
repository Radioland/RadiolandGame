Shader "Custom/RimMetal" {
	Properties {
		_Ramp ("Toon Ramp (RGB)", 2D) = "gray" {}
		_RimPower("_RimPower", Range(0.1,10) ) = 1.707772
		
		_Tint ("Tint Color", Color) = (1,1,1,0.5)
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Eyes finalcolor:tint

		uniform sampler2D _Ramp;
		uniform float _RimPower;
		fixed4 _Tint;

		struct Input {
			float3 viewDir;
		};
		
		inline half4 LightingEyes (SurfaceOutput s, half3 lightDir, half atten)
    	{
	        half d = s.Albedo.r;
	        // Applied through ramp
	        half3 ramp = tex2D (_Ramp, float2(d,d)).rgb;
	        half4 c;
	        c.rgb = s.Albedo * ramp;
	        c.a = 0;
	        return c;
    	}
    	
    	void tint (Input IN, SurfaceOutput o, inout fixed4 color) {
			color = saturate(color);
			color *= _Tint;
		}
		
		void surf (Input IN, inout SurfaceOutput o) {
			o.Albedo = (1.0,1.0,1.0);
			float rim = pow(1.0 - saturate(dot(o.Normal, normalize(IN.viewDir))), _RimPower);
			o.Albedo = (1.0-rim,1.0-rim,1.0-rim);
			o.Alpha = 1.0;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}