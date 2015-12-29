Shader "Teatro/Core"
{
    Properties
    {
        [HDR] _Color ("", Color) = (1,1,1)
    }

    CGINCLUDE

    #include "SimplexNoise3D.cginc"

    float3 _MaskOffset;
    float3 _SpikeOffset;
    float _MaskFreq;
    float3 _SpikeParams; // freq, amp, exp

    float3 spike_displacement(float3 vp)
    {
        float n = snoise(vp * _SpikeParams.x + _SpikeOffset);
        return vp * (1.0 + pow(abs(n), _SpikeParams.z) * _SpikeParams.y);
    }

    float mask_alpha(float3 vp)
    {
        vp *= _MaskFreq;
        float n1 = snoise(vp + _MaskOffset);
        float n2 = snoise(vp * 2 - _MaskOffset) * 0.5;
        return (n1 + n2) * 0.5 + 0.5;
    }

    ENDCG

    SubShader
    {
        Tags { "RenderType"="TransparentCutout" "Queue"="AlphaTest" }

        CGPROGRAM

        #pragma surface surf Standard vertex:vert alphatest:_Cutoff nolightmap
        #pragma target 3.0

        struct Input { float3 vp; };

        half3 _Color;

        void vert(inout appdata_full v, out Input data)
        {
            UNITY_INITIALIZE_OUTPUT(Input, data);

            data.vp = v.vertex.xyz;

            float3 v1 = spike_displacement(v.vertex.xyz);
            float3 v2 = spike_displacement(v.texcoord.xyz);
            float3 v3 = spike_displacement(v.texcoord1.xyz);

            v.vertex.xyz = v1;
            v.normal = normalize(cross(v2 - v1, v3 - v1));
        }

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            o.Emission = _Color;
            o.Alpha = mask_alpha(IN.vp);
        }

        ENDCG
    }
}
