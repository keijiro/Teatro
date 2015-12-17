Shader "Hidden/Teatro/Disc"
{
    Properties
    {
        _BaseColor ("-", Color) = (0,0,0)
        [HDR] _Emission1 ("-", Color) = (1, 0, 0)
        [HDR] _Emission2 ("-", Color) = (0, 0, 1)
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

    // Rotation around the Y axis.
    float4 y_rotation(float r)
    {
        return float4(0, sin(r * 0.5), 0, cos(r * 0.5));
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
            float4 xzuv : TEXCOORD0;
            float3 cparams : TEXCOORD1;
        };

        half3 _BaseColor;
        half3 _Emission1;
        half3 _Emission2;
        half _Glossiness;
        half _Metallic;

        sampler2D _MainTex;
        half _TexScale;
        sampler2D _NormalTex;
        half _NormalScale;

        float2 _TParams; // rotation time, animation time
        float3 _AParams; // scale, displacement, highlight
        float _RandomSeed;

        void vert(inout appdata_full v, out Input data)
        {
            UNITY_INITIALIZE_OUTPUT(Input, data);

            float2 xz = v.vertex.xz;
            float2 uv0 = v.texcoord.xy;
            float2 uv1 = v.texcoord1.xy + _RandomSeed;
            float3 uv2 = v.texcoord2.xyz;

            // rotation
            float r_spin = (nrand(uv1.y, 0) - 0.5f) * _TParams.x;
            float4 q_spin = y_rotation(r_spin);

            // displacement with noise
            float dy = snoise(uv1 * 0.67 + _TParams.y * 0.4);
            dy += max(sin(uv1.y * 25 - _TParams.y * 1.8), 0);
            dy *= _AParams.y;

            // color selection
            float csel = nrand(uv1, 1) > 0.8;

            // emission intensity
            float em = pow(max(snoise(uv1 * 4 + _TParams.y * 0.4), 0), 8);
            em *= _AParams.z;

            // modify vertex
            v.vertex.xyz = lerp(uv2, v.vertex.xyz, _AParams.x);
            v.vertex.y += dy;
            v.vertex.xyz = rotate_vector(v.vertex.xyz, q_spin);
            v.normal = rotate_vector(v.normal, q_spin);
            v.tangent.xyz = rotate_vector(v.tangent.xyz, q_spin);

            // other parameters
            data.xzuv = float4(xz * _TexScale, uv0);
            data.cparams = float3(v.normal.y, csel, em);
        }

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            half4 tex_c = tex2D(_MainTex, IN.xzuv.xy);
            half4 tex_n = tex2D(_NormalTex, IN.xzuv.xy);

            half use_tex = IN.cparams.x;
            half3 ecolor = lerp(_Emission1, _Emission2, IN.cparams.y);
            half eadd = IN.cparams.z;

            float2 edge_uv = abs(IN.xzuv.zw - 0.5) * 2;
            float edge = pow(max(edge_uv.x, edge_uv.y), 90);

            o.Albedo = _BaseColor * lerp(0, tex_c.rgb, use_tex);
            o.Normal = UnpackScaleNormal(tex_n, _NormalScale * use_tex);
            o.Emission = ecolor * (edge + eadd);
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
        }

        ENDCG
    }
    FallBack "Diffuse"
}
