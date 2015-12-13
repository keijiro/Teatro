Shader "Hidden/Teatro/Wire"
{
    Properties
    {
        [HDR] _Color ("Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        CGPROGRAM

        #pragma surface surf Standard vertex:vert
        #pragma target 3.0

        #include "ClassicNoise2D.cginc"

        struct Input {
            float2 uv_MainTex;
        };

        half4 _Color;
        float _Offset;

        void vert(inout appdata_full v)
        {
            float2 np = float2(v.vertex.x * 4, _Time.y + _Offset);
            v.vertex.y +=
                max(0,
                cnoise(np * 1) * 0.5 +
                cnoise(np * 2) * 0.25 +
                cnoise(np * 4) * 0.125) * 0.2;
        }

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            o.Albedo = 0;
            o.Metallic = 0;
            o.Smoothness = 0;
            o.Emission = _Color;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
