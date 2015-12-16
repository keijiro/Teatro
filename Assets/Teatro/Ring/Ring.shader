Shader "Hidden/Teatro/Ring"
{
    Properties
    {
        _Color ("-", Color) = (1,1,1,1)
        _Glossiness ("-", Range(0,1)) = 0.5
        _Metallic ("-", Range(0,1)) = 0.0
        [HDR] _Emission ("-", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }

        CGPROGRAM

        #pragma surface surf Standard vertex:vert nolightmap
        #pragma target 3.0

        #include "SimplexNoise2D.cginc"

        struct Input { float dummy; };

        fixed4 _Color;
        half _Glossiness;
        half _Metallic;
        half3 _Emission;

        float2 _Config; // throttle, random seed
        half _Width;
        half2 _Length;  // min, max
        half2 _Radius;  // min, max
        half _Slide;
        half2 _Angles;  // arc angle, current angle
        half3 _Noise;   // freq, offset x, y

        // PRNG function
        float nrand(float2 uv, float salt)
        {
            uv += float2(salt, _Config.y);
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

        void vert(inout appdata_full v)
        {
            float2 uv = v.texcoord.xy;

            float sw = uv.x < _Config.x;

            float phi = uv.x * _Angles.x;
            phi += (nrand(uv.y, 0) - 0.5) * 2 * _Angles.y;

            float2 np = float2(uv.x * _Noise.x + _Noise.y, uv.y + _Noise.z);
            float ns = snoise(np) + 0.5 * snoise(np * 2);
            ns = saturate(ns * 5);

            float rad = lerp(_Radius.x, _Radius.y, nrand(uv.y, 1));
            float sld = rad + _Slide * ns;

            float len = lerp(_Length.x, _Length.y, nrand(uv.y, 2));

            float3 p = v.vertex.xyz;

            p.x *= _Width * (nrand(uv.y, 3) + 1) * 0.5;
            p.z = p.z * len * lerp(0.5, 1, ns) + sld;

            p = rotate_vector(p, y_rotation(phi));

            v.vertex.xyz = p * sw;
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            o.Albedo = _Color.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Emission = _Emission;
        }

        ENDCG
    }
    FallBack "Diffuse"
}
