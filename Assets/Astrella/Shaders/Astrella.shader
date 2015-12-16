Shader "Custom/Astrella"
{
    Properties
    {
        _MainTex ("Albedo, Metallic", 2D) = "white" {}
        _Color ("Color", Color) = (1, 1, 1, 1)
        _BumpMap ("Normal", 2D) = "bump"{}
        _BumpScale("Scale", Range(0, 2)) = 1
        _MetallicBias ("Metallic Bias", Range(0, 1)) = 0
        _MetallicScale ("Metallic Scale", Range(0, 2)) = 1
        _SmoothnessBias ("Smoothness Bias", Range(0, 1)) = 0
        _SmoothnessScale ("Smoothness Scale", Range(0, 2)) = 1
        _Emission ("Emission", Color) = (0, 0, 0, 0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        CGPROGRAM

        #pragma multi_compile _ _TRANSITION
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0

        #include "SimplexNoise2D.cginc"

        sampler2D _MainTex;
        sampler2D _BumpMap;
        half _BumpScale;
        half4 _Color;
        half _MetallicBias;
        half _MetallicScale;
        half _SmoothnessBias;
        half _SmoothnessScale;

        float4 _NParams; // noise freq x,y / noise offset x,y
        half2 _RParams; // cutoff offset, gradient range
        half3 _Emission;
        half _Transition;

        struct Input
        {
            float2 uv_MainTex;
        };

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            #if _TRANSITION
            float2 np = IN.uv_MainTex * _NParams.xy + _NParams.zw;
            float ns = (snoise(np) + 1) * 0.5;

            clip(ns * (1 - _RParams.x) - _Transition + _RParams.x);

            float ep0 = _RParams.y - _Transition * 1.2;
            o.Emission = smoothstep(ep0, ep0 + 0.2, ns * _RParams.y) * _Emission;
            #endif

            fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
            o.Albedo = c.rgb * _Color.rgb;
            o.Normal = UnpackScaleNormal(tex2D(_BumpMap, IN.uv_MainTex), _BumpScale);
            o.Metallic = saturate(c.a * _MetallicScale + _MetallicBias);
            o.Smoothness = saturate(c.a * _SmoothnessScale + _SmoothnessBias);
        }

        ENDCG
    }
    FallBack "Diffuse"
}
