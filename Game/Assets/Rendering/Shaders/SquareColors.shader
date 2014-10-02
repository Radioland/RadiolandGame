Shader "Custom/SquareColors" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Ramp ("Ramp (RGB custom)", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		#pragma surface surf Lambert

		sampler2D _MainTex;
		sampler2D _Ramp;

		struct Input {
			float2 uv_MainTex;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			half4 c = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
			o.Alpha = c.a;
			
			float y = 0;
			if (IN.uv_MainTex.x < 0.5 && IN.uv_MainTex.y < 0.5) {
				y = 0 / 16.0;
			} else if (IN.uv_MainTex.x > 0.5 && IN.uv_MainTex.y < 0.5) {
				y = 1 / 16.0;
			} else if (IN.uv_MainTex.x < 0.5 && IN.uv_MainTex.y > 0.5) {
				y = 2 / 16.0;
			} else {
				y = 3 / 16.0;
			}
			o.Albedo = tex2D(_Ramp, float2(0, y));
		}
		ENDCG
	}
	FallBack "Diffuse"
}
