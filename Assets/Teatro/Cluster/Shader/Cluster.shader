Shader "Hidden/Cluster"
{
    Properties
    {
        _Color ("-", Color) = (1,1,1,1)
        _MainTex ("-", 2D) = "white"{}
        _NormalTex ("-", 2D) = "bump"{}
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

    // Uniform random unit quaternion
    // http://www.realtimerendering.com/resources/GraphicsGems/gemsiii/urot.c
    float4 random_rotation(float2 uv)
    {
        float r = nrand(uv, 30);
        float r1 = sqrt(1.0 - r);
        float r2 = sqrt(r);
        float t1 = UNITY_PI * 2 * nrand(uv, 40);
        float t2 = UNITY_PI * 2 * nrand(uv, 50);
        return float4(sin(t1) * r1, cos(t1) * r1, sin(t2) * r2, cos(t2) * r2);
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
        };

        half _InstanceCount;
        half _Throttle;
        half _Transition;
        float4 _TubeParams;
        float _Scatter;
        float2 _NoiseParams;
        float _Scale;

        half4 _Color;
        half _Glossiness;
        half _Metallic;

        sampler2D _MainTex;
        half _TexScale;

        sampler2D _NormalTex;
        half _NormalScale;

        void vert(inout appdata_full v)
        {
            float uv = v.texcoord1.xy;

            float v_phi = lerp(-1, 1, nrand(uv, 3)) * _TubeParams.x;
            float phi = nrand(uv, 1) * UNITY_PI * 2 + v_phi * _Time.y;

            float sin_phi, cos_phi;
            sincos(phi, sin_phi, cos_phi);

            float radius = lerp(_TubeParams.y, _TubeParams.z, nrand(uv, 0));
            float height = (nrand(uv, 2) - 0.5) * _TubeParams.w;

            float3 pos0 = float3(cos_phi * radius, height, sin_phi * radius);
            float3 pos1 = float3(nrand(uv, 5), nrand(uv, 6), nrand(uv, 7));
            pos1 = (pos1 - 0.5) * float3(_Scatter, _TubeParams.w, _Scatter);

            float2 np = pos0.xy * _NoiseParams.x + _Time.y * _NoiseParams.y;
            float scale = (1.0 + snoise(np)) * _Scale;
            scale *= saturate((_Throttle - uv.x) * _InstanceCount);

            float4 rot0 = y_rotation(-phi);
            float4 rot1 = random_rotation(uv);

            float3 pos = lerp(pos0, pos1, _Transition);
            float4 rot = normalize(lerp(rot0, rot1, _Transition));
            float2 uv_rand = float2(nrand(uv, 8), nrand(uv, 9));

            v.vertex.xyz = rotate_vector(v.vertex.xyz * scale, rot) + pos;
            v.normal = rotate_vector(v.normal, rot);
            v.texcoord.xy = v.texcoord.xy * _TexScale + uv_rand;
        }

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            half4 tex_c = tex2D(_MainTex, IN.uv_MainTex);
            half4 tex_n = tex2D(_NormalTex, IN.uv_MainTex);

            o.Albedo = _Color.rgb * tex_c.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Normal = UnpackScaleNormal(tex_n, _NormalScale);
        }

        ENDCG
    }
    FallBack "Diffuse"
}
