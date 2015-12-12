Shader "Teatro/Disc"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (0,0,0,1)
        [HDR] _LineColor ("Line Color", Color) = (1,1,1,1)
        _Glossiness ("Smoothness", Range(0, 1)) = 0.5
        _Metallic ("Metallic", Range(0, 1)) = 0.0
    }

    CGINCLUDE

    // PRNG function
    float nrand(float2 uv, float salt)
    {
        uv += float2(salt, 0);
        return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453);
    }

    // Quaternion multiplication
    // http://mathworld.wolfram.com/Quaternion.html
    float4 qmul(float4 q1, float4 q2)
    {
        return float4(
            q2.xyz * q1.w + q1.xyz * q2.w + cross(q1.xyz, q2.xyz),
            q1.w * q2.w - dot(q1.xyz, q2.xyz)
        );
    }

    // Vector rotation with a quaternion
    // http://mathworld.wolfram.com/Quaternion.html
    float3 rotate_vector(float3 v, float4 r)
    {
        float4 r_c = r * float4(-1, -1, -1, 1);
        return qmul(r, qmul(float4(v, 0), r_c)).xyz;
    }

    ENDCG

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

        half _Glossiness;
        half _Metallic;
        half4 _BaseColor;
        half4 _LineColor;

        void vert(inout appdata_full v)
        {
            float2 uv0 = v.texcoord;
            float2 uv1 = v.texcoord1;

            float a = (nrand(uv1.x, 0) - 0.5f) * _Time.y * 0.4f;
            float4 r = float4(0, sin(a), 0, cos(a));

            float dy = snoise(uv1 * 11 + _Time.x * 3) * 0.02;
            float l = pow(max(snoise(uv1 * 11 + _Time.x * 4), 0), 10);

            v.vertex.xyz = rotate_vector(v.vertex.xyz, r);
            v.vertex.y += dy;
            v.color = float4(uv0.xy, l, 0);
        }

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            float2 uv = abs(IN.color.xy - 0.5) * 2;
            float ln = pow(max(uv.x, uv.y), 80);

            o.Albedo = _BaseColor.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Emission = _LineColor.rgb * (ln + IN.color.z);
        }

        ENDCG
    }
    FallBack "Diffuse"
}
