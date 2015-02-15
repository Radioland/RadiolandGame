Shader "Jonatron/NPR"
{
    Properties
    {
        _MainTex("Main Texture", 2D) = "white" {}    // These properties must have the same names as in standard Unity 
													//  shaders in order for fallback shaders to correctly work.
        _Ramp("Ramp", 2D) = "white" {}
        _Color("Color", Color) = (1.0, 1.0, 1.0, 1.0)
        _DiffuseScale("Diffuse Scale", Float) = 1
        _DiffuseBias("Diffuse Bias", Float) = 0
        _DiffuseExponent("Diffuse Exponent", Float) = 1
        _RimColor("Rim Color", Color) = (0.26,0.19,0.16,0.0)
        _RimPower("Rim Power", Range(0.5, 8.0)) = 3.0
        _SpecColor("Specular Color", Color) = (0.0, 0.0, 0.0, 1.0)
        _SpecPower("Specular Power", Range(0.5, 128.0)) = 3.0
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }    // This allows Unity to intelligently substitute this shader when needed.
        LOD 200
 
        CGPROGRAM
        #include "UnityCG.cginc"
        #pragma surface surf NPR noambient
 
        // These match the shader properties
        uniform sampler2D _MainTex, _Ramp;
        uniform float4 _Color, _RimColor;
        uniform float _DiffuseScale, _DiffuseBias, _DiffuseExponent;
        uniform float _RimPower, _SpecPower;
 
        half4 LightingNPR(SurfaceOutput o, half3 lightdir, half3 viewdir, half atten)
        {
            // Wrapped Lambertian diffuse term
            float lambert = saturate(dot(o.Normal, lightdir));
            lambert = pow(lambert*_DiffuseScale + _DiffuseBias, _DiffuseExponent);
            half4 diffuse = half4(_LightColor0.rgb * atten * o.Albedo.rgb, 1.0);
            diffuse *= tex2D(_Ramp, float2(lambert, 0.0));
 
            // Rim lighting
            float rim_term = 1.0 - saturate(dot(viewdir, o.Normal));
            rim_term = pow(rim_term, _RimPower);
            half4 rim = half4(_RimColor.rgb * rim_term, 1.0); 
 
            // Phong's specular term
            half3 r = reflect(-lightdir, o.Normal);
            float phong = pow(saturate(dot(r, viewdir)), _SpecPower);
            half4 specular = half4(phong * _SpecColor * atten);
 
            return diffuse + rim + specular;
        }
 
        struct Input
        {
            // This contains the inputs to the surface function
            // Valid fields are listed at:
            // http://docs.unity3d.com/Documentation/Components/SL-SurfaceShaders.html 
            float2 uv_MainTex;
        };
 
        void surf(Input IN, inout SurfaceOutput o)
        {
            // This is where we prepare the surface for lighting by propagating a SurfaceOutput structure
            half4 c = tex2D(_MainTex, IN.uv_MainTex);    // Sample the texture
            o.Albedo = _Color.rgb * c;                    // Modulate by main colour
            o.Alpha = 1.0;                                // No alpha in this shader
 
        }
        ENDCG
    }
    FallBack "Diffuse"    // Shader to use if the user's hardware cannot incorporate this one
}