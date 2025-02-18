Shader "Custom/UI/BlendFade"
{
    Properties
    {
        _FirstTex ("Texture1", 2D) = "white" {}
        _SecondTex ("Texture2", 2D) = "black" {}
        _Weight ("Weight", Range(0,1)) = 0.0
        _TotalFade ("Total Fade", Range(0,1)) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _FirstTex;
            sampler2D _SecondTex;
            float4 _FirstTex_ST;
            float _Fade;
            float _TotalFade;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _FirstTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col1 = tex2D(_FirstTex, i.uv);
                fixed4 col2 = tex2D(_SecondTex, i.uv);
                return lerp(col1, col2, _Fade) * _TotalFade;
            }
            ENDCG
        }
    }
}