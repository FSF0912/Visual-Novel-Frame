Shader "Hidden/UITextOutline"
{
    Properties
    {
        [PerRendererData]_MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags
        {
            "Queue"="Transparent" "IgnoreProjector"="true" "RenderType"="Transparent"
        }
        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float4 uv : TEXCOORD0;
                float4 uv1 : TEXCOORD1;
                float4 uv2 : TEXCOORD2;
                float4 color:COLOR;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 border : TEXCOORD1;
                float4 color:COLOR;
                float width: TEXCOORD2;
                float4 edgeColor: TEXCOORD3;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize;
            
            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.width = v.uv.z;
                o.color = v.color;
                o.border = v.uv1;
                o.edgeColor=v.uv2;
                return o;
            }

            fixed isInRange(fixed2 uv_min,fixed2 uv_max,fixed2 uv)
            {
                fixed2 rs = step(uv_min, uv) * step(uv, uv_max);
                return rs.x * rs.y;
            }

            fixed SampleTex(v2f i,fixed ii,fixed color_a)
            {
                const fixed OffsetX[12] = {1, 0.866, 0.5, 0, -0.5, -0.866, -1, -0.866, -0.5, 0, 0.5, 0.866};
                const fixed OffsetY[12] = {0, 0.5, 0.866, 1, 0.866, 0.5, 0, -0.5, -0.866, -1, -0.866, -0.5};
                float2 offset_uv = i.uv + float2(OffsetX[ii], OffsetY[ii]) * _MainTex_TexelSize.xy * i.width;
                fixed sample_a = (tex2D(_MainTex, offset_uv)).a;
                fixed a = sample_a ;
                a *= isInRange(i.border.xy, i.border.zw, offset_uv);
                return a;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv).a*i.color;
                col.a *= isInRange(i.border.xy, i.border.zw, i.uv);
                fixed sum_a = 0;
                sum_a += SampleTex(i, 0, col.a);
                sum_a += SampleTex(i, 1, col.a);
                sum_a += SampleTex(i, 2, col.a);
                sum_a += SampleTex(i, 3, col.a);
                sum_a += SampleTex(i, 4, col.a);
                sum_a += SampleTex(i, 5, col.a);
                sum_a += SampleTex(i, 6, col.a);
                sum_a += SampleTex(i, 7, col.a);
                sum_a += SampleTex(i, 8, col.a);
                sum_a += SampleTex(i, 9, col.a);
                sum_a += SampleTex(i, 10, col.a);
                sum_a += SampleTex(i, 11, col.a);
                sum_a = saturate(sum_a);
                fixed4 outLineColor = fixed4(i.edgeColor.rgb,sum_a);
                fixed a=step(i.width,0.001);
                fixed4 finalCol = lerp(outLineColor, col, saturate(a+col.a));
                return finalCol;
            }
            ENDCG
        }
    }
}
