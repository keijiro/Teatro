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

        #pragma surface surf Standard vertex:vert nolightmap
        #pragma target 3.0

        #include "SimplexNoise2D.cginc"

        struct Input {
            float worldPos;
            half4 color : COLOR;
        };

        half4 _Color;

        void vert(inout appdata_full v)
        {
            float radius = 1 + snoise(float2(v.texcoord.y * 33.3, _Time.y * 0.05));

            v.vertex.xz *= radius;

            float2 np = v.vertex.xz * 1.2 - float2(0, _Time.y) * 0.3;

            float n = snoise(np * 1) * 0.5 + snoise(np * 2) * 0.25 + snoise(np * 4) * 0.125 + snoise(np * 8) * 0.0625;

            v.vertex.y += max(n, 0) * 0.2;

            v.color = _Color * (0.5 + v.texcoord.y * 0.5);
        }

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            o.Emission = IN.color;
        }

        ENDCG
    }
    FallBack "Diffuse"
}
