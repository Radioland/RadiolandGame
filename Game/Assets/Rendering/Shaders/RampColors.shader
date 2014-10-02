Shader "Custom/SquareColors" {
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
			if (IN.uv_MainTex.x < 0.5 && IN.uv_MainTex.y < 0.5) {
				y = 0.5 / 16.0;
			} else if (IN.uv_MainTex.x > 0.5 && IN.uv_MainTex.y < 0.5) {
				y = 1.5 / 16.0;
			} else if (IN.uv_MainTex.x < 0.5 && IN.uv_MainTex.y > 0.5) {
				y = 2.5 / 16.0;
			} else {
				y = 3.5 / 16.0;
			}
			color = saturate(color);
			color *= tex2D(_MainTex, float2(color.r, y));			
		}

		void surf (Input IN, inout SurfaceOutput o) {
			//o.Albedo = float3(0.0,0.0,0.0);
			o.Albedo = float3(1.0,1.0,1.0);
			o.Alpha = 1.0;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
