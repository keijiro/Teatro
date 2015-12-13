Shader "Teatro/Disc"
{
    Properties
    {
        _BaseColor ("-", Color) = (0,0,0,1)
        [HDR] _LineColor ("-", Color) = (1,1,1,1)

        _Glossiness ("-", Range(0, 1)) = 0.5
        _Metallic ("-", Range(0, 1)) = 0.0

        _MainTex ("-", 2D) = "white"{}
        _TexScale ("-", Float) = 1
        _NormalTex ("-", 2D) = "bump"{}
        _NormalScale ("-", Range(0,2)) = 1
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
            float2 uv_MainTex;
            half4 color : COLOR;
        };

        half4 _BaseColor;
        half4 _LineColor;

        half _Glossiness;
        half _Metallic;

        sampler2D _MainTex;
        half _TexScale;
        sampler2D _NormalTex;
        half _NormalScale;

        void vert(inout appdata_full v)
        {
            float2 uv0 = v.texcoord;
            float2 uv1 = v.texcoord1;

            float a = (nrand(uv1.x, 0) - 0.5f) * _Time.y * 0.4f;
            float4 r = float4(0, sin(a), 0, cos(a));

            float dy = snoise(uv1 * 11 + _Time.x * 3) * 0.02;
            float l = pow(max(snoise(uv1 * 11 + _Time.x * 4), 0), 10);

            v.texcoord = v.vertex.xzxz * _TexScale;

            v.vertex.xyz = rotate_vector(v.vertex.xyz, r);
            v.vertex.y += dy;
            v.normal = rotate_vector(v.normal, r);
            v.tangent.xyz = rotate_vector(v.tangent.xyz, r);
            v.color = float4(uv0.xy, l, v.normal.y);
        }

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            half4 c = lerp(0.5, tex2D(_MainTex, IN.uv_MainTex), IN.color.w);
            half4 n = tex2D(_NormalTex, IN.uv_MainTex);

            float2 uv = abs(IN.color.xy - 0.5) * 2;
            float ln = pow(max(uv.x, uv.y), 120);

            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;

            o.Albedo = c.rgb * _BaseColor.rgb;
            o.Normal = UnpackScaleNormal(n, _NormalScale * IN.color.w);
            o.Emission = _LineColor.rgb * (ln + IN.color.z);
        }

        ENDCG
    }
    FallBack "Diffuse"
}
