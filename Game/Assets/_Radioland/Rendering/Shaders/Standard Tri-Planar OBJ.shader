Shader "Standard Tri-Planar Obj" {
  Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _Side("Side", 2D) = "white" {}
        //_GlossinessSide ("Smoothness Side", Range(0,1)) = 0.5
        //_MetallicSide ("Metallic Side", Range(0,1)) = 0.0
        _Top("Top", 2D) = "white" {}
        //_GlossinessTop ("Smoothness Top", Range(0,1)) = 0.5
        //_MetallicTop ("Metallic Top", Range(0,1)) = 0.0
        _Bottom("Bottom", 2D) = "white" {}
        //_GlossinessBottom ("Smoothness Bottom", Range(0,1)) = 0.5
        //_MetallicBottom ("Metallic Bottom", Range(0,1)) = 0.0

        _SideScale("Side Scale", Float) = 2
        _TopScale("Top Scale", Float) = 2
        _BottomScale ("Bottom Scale", Float) = 2

    }

    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows vertex:vert

        #pragma target 3.0

        fixed4 _Color;
        sampler2D _Side, _Top, _Bottom;
        //half _GlossinessSide, _GlossinessTop, _GlossinessBottom;
        //half _MetallicSide, _MetallicTop, _MetallicBottom;
        float _SideScale, _TopScale, _BottomScale;

        struct Input {
            //float3 worldPos;
            //float3 worldNormal;
            float3 vertNormal;
            float3 objPos;
        };

        void vert (inout appdata_full v, out Input o) {
          UNITY_INITIALIZE_OUTPUT(Input, o);
          o.vertNormal = v.normal;
          o.objPos = v.vertex;
        }

        void surf (Input IN, inout SurfaceOutputStandard o) {
            //float3 projNormal = saturate(pow(IN.worldNormal * 1.4, 4));

            //o.Smoothness = _GlossinessSide;
            //o.Smoothness = lerp(o.Smoothness, _GlossinessSide, abs(IN.vertNormal.x));

            //o.Metallic = _MetallicSide;
            //o.Metallic = lerp(o.Metallic, _MetallicSide, abs(IN.vertNormal.x));

            // SIDE X
            float3 x = tex2D(_Side, frac(IN.objPos.zy * _SideScale)) * abs(IN.vertNormal.x);

            // TOP / BOTTOM
            float3 y = 0;
            if (IN.vertNormal.y > 0) {
                y = tex2D(_Top, frac(IN.objPos.zx * _TopScale)) * abs(IN.vertNormal.y);
                //o.Smoothness = lerp(o.Smoothness, _GlossinessTop, abs(IN.vertNormal.y));
                //o.Metallic = lerp(o.Metallic, _MetallicTop, abs(IN.vertNormal.y));
            } else {
                y = tex2D(_Bottom, frac(IN.objPos.zx * _BottomScale)) * abs(IN.vertNormal.y);
                //o.Smoothness = lerp(o.Smoothness, _GlossinessBottom, abs(IN.vertNormal.y));
                //o.Metallic = lerp(o.Metallic, _MetallicBottom, abs(IN.vertNormal.y));
            }

            // SIDE Z
            float3 z = tex2D(_Side, frac(IN.objPos.xy * _SideScale)) * abs(IN.vertNormal.z);

            o.Albedo = z;
            o.Albedo = lerp(o.Albedo, x, abs(IN.vertNormal.x));
            o.Albedo = lerp(o.Albedo, y, abs(IN.vertNormal.y));

            o.Albedo *=  _Color;
            
            //o.Albedo = IN.vertNormal;
            //o.Albedo = float3((IN.vertNormal.x+1)/2.0,(IN.vertNormal.y+1)/2.0,(IN.vertNormal.z+1)/2.0);
            //o.Albedo = float3(1.0-IN.vertNormal.x,1.0-IN.vertNormal.y,1.0-IN.vertNormal.z);
        }
        ENDCG
    }
    Fallback "Diffuse"
}
