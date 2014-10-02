Shader "Custom/RampColors" {
	Properties {
		_MainTex ("Ramp (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		#pragma surface surf Lambert finalcolor:rampcolor

		sampler2D _MainTex;
		
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
			color = saturate(color);
			color = tex2D(_MainTex, float2(color.r, y));
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
	}
	FallBack "Diffuse"
}
