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
            half4 color : COLOR;
        };

        float2 _Radius;
        float4 _RingParams; // end phi, wave amp, wave freq, wave speed
        float4 _NoiseParams; // amp, freq, speed, threshold
        half4 _Color;

        void vert(inout appdata_full v)
        {
            float phi = _RingParams.x * v.texcoord.x;

            float2 np_r = float2(phi * _RingParams.z, _Time.x * _RingParams.w);
            float r = v.texcoord.x + _RingParams.y * snoise(np_r);
            r = lerp(_Radius.x, _Radius.y, smoothstep(0, 1, r));

            float cs, sn;
            sincos(phi, sn, cs);
            float3 p = float3(cs, 0, sn) * r;

            float2 np_p = p.xz * _NoiseParams.y;
            np_p -= float2(0, _Time.y) * _NoiseParams.z;

            float n =
                snoise(np_p * 1) * 0.5 +
                snoise(np_p * 2) * 0.25 +
                snoise(np_p * 4) * 0.125;

            p.y = max(n * _NoiseParams.x, _NoiseParams.w);

            v.vertex.xyz = p;
            v.color = _Color * v.texcoord.x;
        }

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            o.Emission = IN.color;
        }

        ENDCG
    }
    FallBack "Diffuse"
}
