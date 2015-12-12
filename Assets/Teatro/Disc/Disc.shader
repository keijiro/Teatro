Shader "Teatro/Disc"
{
	Properties
    {
        _MainTex ("Albedo, Metallic", 2D) = "white" {}
		_Color ("Color", Color) = (1,1,1,1)
		_Glossiness ("Smoothness", Range(0, 1)) = 0.5
		_Metallic ("Metallic", Range(0, 1)) = 0.0
	}
	SubShader
    {
		Tags { "RenderType"="Opaque" }
		
		CGPROGRAM

		#pragma surface surf Standard
		#pragma target 3.0

		struct Input {
			float2 uv_MainTex;
		};
        sampler2D _MainTex;

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		void surf(Input IN, inout SurfaceOutputStandard o)
        {
			o.Albedo = _Color.rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
            float2 x = abs(IN.uv_MainTex.xy - 1);
            o.Emission = pow(max(x.x, x.y), 60) * float3(7, 0.4, 0.5);
		}
		ENDCG
	}
	FallBack "Diffuse"
}
