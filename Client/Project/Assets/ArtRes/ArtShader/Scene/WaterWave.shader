Shader "ORII/Scene/WaterWave"
{
    Properties
    {
        _BaseColor ("Main Color", Color) = (1,1,1,1)
        _FoamMaskTex ("Foam Mask Texture", 2D) = "white" { }
        _FoamColor ("Foam Color", Color) = (1,1,1,1)
        _FoamSpeed ("Foam Speed", Float) = 1
        _FoamTex ("Foam Texture", 2D) = "white" { }
        _FoamValue ("Foam Intensity", Range(0.5, 10)) = 1
        _WaveTex ("Water Mask Texture", 2D) = "white" { }
        _WaveColor ("HighLight Color", Color) = (1,1,1,1)
        _WaveSpeed_A ("HighLight X Speed", Float) = 1
        _WaveSpeed_B ("HighLight Y Speed", Float) = 1
        _DeepWaveColor ("Wave Color", Color) = (1,1,1,1)
        _WaveInt ("Wave Intensity", Float) = 1
        _MasterColor ("Master Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags
        {
            "QUEUE" = "Transparent" "RenderType" = "Opaque"
        }
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 100

        Pass
        {
            Tags
            {
                "LightMode"="UniversalForward"
            }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
     


            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 uv1 : TEXCOORD1;
                float4 uv2 : TEXCOORD2;
                float4 vertex : SV_POSITION;
            };

            sampler2D _FoamMaskTex;
            sampler2D _FoamTex;
            sampler2D _WaveTex;
            float4 _FoamMaskTex_ST;
            float4 _FoamTex_ST;
            float4 _WaveTex_ST;
            half4 _BaseColor;
            half4 _FoamColor;
            half4 _WaveColor;
            half4 _DeepWaveColor;
            half4 _MasterColor;
            half _FoamValue;
            half _WaveInt;
            half _WaveSpeed_A;
            half _WaveSpeed_B;
            half _FoamSpeed;
       

            v2f vert(appdata v)
            {
                v2f o;
                half4 u_xlat0;
                half4 u_xlat1;
                half4 u_xlat2;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv.xy = TRANSFORM_TEX(v.uv, _FoamMaskTex);
                u_xlat0.x = _Time.y * 0.1f;
                u_xlat1 = u_xlat0.xxxx * float4(_FoamSpeed, _FoamSpeed, _WaveSpeed_A, _WaveSpeed_B) + v.uv.xyxy;
                u_xlat0 = (-u_xlat0.xxxx) * float4(_FoamSpeed, _WaveSpeed_A, _FoamSpeed, _WaveSpeed_B) + v.uv.yxxy;
                u_xlat2.x = u_xlat1.x;
                u_xlat2.y = u_xlat0.x;
                o.uv1.xy = u_xlat2.xy * _FoamTex_ST.xy + _FoamTex_ST.zw;
                o.uv2.zw = u_xlat0.yw * _WaveTex_ST.xy + _WaveTex_ST.zw;
                u_xlat0.w = u_xlat1.y;
                o.uv2.xy = u_xlat1.zw * _WaveTex_ST.xy + _WaveTex_ST.zw;
                o.uv1.zw = u_xlat0.zw * _FoamTex_ST.xy + _FoamTex_ST.zw;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                half4 u_xlat0;
                half4 finalColor;
                half4 u_xlat16_1;
                half3 u_xlat16_5;
                half u_xlat16_3;
                half u_xlat16_4;
                half u_xlat16_7;
                half u_xlat16_9;
                finalColor.x = tex2D(_WaveTex, i.uv2.xy).z;
                u_xlat16_1.x = finalColor.x * _WaveInt;
                u_xlat0 = u_xlat16_1.xxxx * float4(0.100000001, 0.100000001, 0.100000001, 0.100000001) + i.uv2;
                finalColor.x = tex2D(_WaveTex, u_xlat0.xy).x;
                u_xlat16_4 = tex2D(_WaveTex, u_xlat0.zw).y;
                u_xlat16_1.x = u_xlat16_4 * finalColor.x;
                u_xlat16_5.xyz = u_xlat16_4 * _DeepWaveColor.xyz;

                finalColor.xyz = u_xlat16_5.xyz;
                finalColor.w = 0.0;
                finalColor = u_xlat16_1.xxxx * _WaveColor + finalColor;
                u_xlat16_3 = tex2D(_FoamTex, i.uv1.xy).x;
                u_xlat16_7 = tex2D(_FoamTex, i.uv1.zw).y;
                u_xlat16_1.x = u_xlat16_7 * u_xlat16_3;
                u_xlat16_3 = tex2D(_FoamMaskTex, i.uv.xy).x;
                u_xlat16_5.x = log2(u_xlat16_3);
                u_xlat16_5.x = u_xlat16_5.x * _FoamValue;
                u_xlat16_9 = u_xlat16_5.x * 0.600000024;
                u_xlat16_5.x = exp2(u_xlat16_5.x);
                u_xlat16_9 = exp2(u_xlat16_9);
                u_xlat16_1.x = u_xlat16_1.x * u_xlat16_9 + u_xlat16_5.x;
                u_xlat16_1.x = u_xlat16_5.x * u_xlat16_1.x;
                u_xlat16_1 = u_xlat16_1.xxxx * _FoamColor + _BaseColor;
                finalColor = finalColor + u_xlat16_1;
                return finalColor * _MasterColor;;
            }
            ENDCG
        }
    }
}