Shader "Custom/Shell"
{
    Properties
    {
        _Color1 ("Albedo (front)", Color) = (1,1,1,1)
        _Color2 ("Albedo (back)", Color) = (1,1,1,1)
        _Glossiness1 ("Smoothness (front)", Range(0,1)) = 0.5
        _Glossiness2 ("Smoothness (back)", Range(0,1)) = 0.5
        _Metallic1 ("Metallic (front)", Range(0,1)) = 0.0
        _Metallic2 ("Metallic (back)", Range(0,1)) = 0.0
        [HDR] _Emission ("Emission (front)", Color) = (1,1,1,1)
    }

    CGINCLUDE

    #include "ClassicNoise3D.cginc"

    float _WTime;
    float3 _WParams1;
    float3 _WParams2;
    float3 _WParams3;

    float3 _NOffset;
    float3 _NParams; // frequency, amplitude, exponent

    float wave_alpha(float3 p)
    {
        float a =
            sin(p.x * _WParams1.x * sin(_WTime * _WParams2.x)  * _WParams3.x +
            sin(p.y * _WParams1.y * sin(_WTime * _WParams2.y)) * _WParams3.y +
            sin(p.z * _WParams1.z * sin(_WTime * _WParams2.z)) * _WParams3.z + _WTime);
        return (a + 1) / 2;
    }

    float3 noise_disp(float3 vp)
    {
        float n = cnoise(vp * _NParams.x + _NOffset);
        return vp * (1.0 + pow(abs(n), _NParams.z) * _NParams.y);
    }

    ENDCG

    SubShader
    {
        Tags { "RenderType"="TransparentCutout" "Queue"="AlphaTest" }

        // front-face

        Cull Back

        CGPROGRAM

        #pragma surface surf Standard vertex:vert alphatest:_Cutoff nolightmap addshadow
        #pragma target 3.0

        struct Input {
            float3 worldPos;
        };

        half4 _Color1;
        half _Glossiness1;
        half _Metallic1;
        half4 _Emission;

        void vert(inout appdata_full v)
        {
            float3 v1 = noise_disp(v.vertex.xyz);
            float3 v2 = noise_disp(v.normal);
            float3 v3 = noise_disp(v.tangent.xyz);
            v.vertex.xyz = v1;
            v.normal = normalize(cross(v2 - v1, v3 - v1));
        }

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            o.Albedo = _Color1.rgb;
            o.Metallic = _Metallic1;
            o.Smoothness = _Glossiness1;
            o.Emission = _Emission;
            o.Alpha = wave_alpha(IN.worldPos);
        }

        ENDCG

        // back-face

        Cull Front

        CGPROGRAM

        #pragma surface surf Standard vertex:vert alphatest:_Cutoff nolightmap addshadow
        #pragma target 3.0

        struct Input {
            float3 worldPos;
        };

        half4 _Color2;
        half _Glossiness2;
        half _Metallic2;

        void vert(inout appdata_full v)
        {
            float3 v1 = noise_disp(v.vertex.xyz);
            float3 v2 = noise_disp(v.normal);
            float3 v3 = noise_disp(v.tangent.xyz);
            v.vertex.xyz = v1;
            v.normal = -normalize(cross(v2 - v1, v3 - v1));
        }

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            o.Albedo = _Color2.rgb;
            o.Metallic = _Metallic2;
            o.Smoothness = _Glossiness2;
            o.Alpha = wave_alpha(IN.worldPos);
        }

        ENDCG
    }
}
