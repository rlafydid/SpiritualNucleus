Shader "ORII/Role/Unlit/Role_Unlit_Face"
{
    Properties
    {
        [HDR]_BrightColor ("BrightColor", Color) = (1,1,1,1)
        [HDR]_DarkColor("DarkColor", Color) = (0,0,0,0)
        _MainTex ("MainTex", 2D) = "white" {}
        _LightMap ("LightMap", 2D) = "white" {}
        [HDR]_OutLineColor ("OutLineColor", Color) = (0,0,0,0)
        _OutLineStrength ("OutLineStrength", float) = 0.001
        _ShiftX ("ShiftX", float) = 0
        _ShiftY ("ShiftY", float) = 0
        _LightDir ("LighrDir", Vector) = (0.5,0.5,0,0)
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline" "RenderType"="Opaque" "Queue"="Geometry-20"
        }
        LOD 100

        Pass
        {
            Tags
            {
                "LightMode"="UniversalForward"
            }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            CBUFFER_START(UnityPerMaterial)
            float4 _MainTex_ST;
            half3 _BrightColor;
            half3 _DarkColor;
            half _ShiftX;
            half _ShiftY;
            CBUFFER_END
            sampler2D _MainTex;
            sampler2D _LightMap;

            v2f vert(appdata v)
            {
                v2f o;
                half4 viewvertex = mul(UNITY_MATRIX_MV, v.vertex);
                half4x4 PMatrix = UNITY_MATRIX_P;
                PMatrix[0][2] = _ShiftX;
                PMatrix[1][2] = _ShiftY;
                o.vertex = mul(PMatrix, viewvertex);
                o.uv = v.uv * _MainTex_ST.xy + _MainTex_ST.zw;
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                half3 maintex = tex2D(_MainTex, i.uv);
                half3 lightmaptex01 = tex2D(_LightMap, float2(1 - i.uv.x, i.uv.y));
                half3 lightmaptex02 = tex2D(_LightMap, i.uv);
                half2 left = normalize(TransformObjectToWorldDir(half3(1, 0, 0)).xz);
                half2 front = normalize(TransformObjectToWorldDir(half3(0, 0, 1)).xz);
                Light light = GetMainLight();
                half3 worldlightdir = normalize(light.direction);
                half ctrl = 1 - max(dot(front, -worldlightdir.xz) * 0.5 + 0.5, 0);
                half ilm = dot(worldlightdir.xz, left) > 0 ? lightmaptex01.r : lightmaptex02.r;
                half isSahdow = smoothstep(ilm, ilm + 0.01, ctrl);
                half3 finalcolor = maintex * lerp(_BrightColor, _DarkColor, isSahdow);
                return half4(finalcolor, 1);
            }
            ENDHLSL
        }
        //
        //        Pass
        //        {
        //            Cull Front
        //            HLSLPROGRAM
        //            #pragma vertex vert
        //            #pragma fragment frag
        //
        //            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        //
        //            struct appdata
        //            {
        //                float4 vertex : POSITION;
        //                float2 uv : TEXCOORD0;
        //                float3 normal : NORMAL;
        //            };
        //
        //            struct v2f
        //            {
        //                float2 uv : TEXCOORD0;
        //                float4 vertex : SV_POSITION;
        //            };
        //
        //            sampler2D _MainTex;
        //            float4 _MainTex_ST;
        //            float3 _OutLineColor;
        //            float _OutLineStrength;
        //            float _ShiftX;
        //            float _ShiftY;
        //
        //            v2f vert (appdata v)
        //            {
        //                v2f o;
        //                v.vertex.xyz += v.normal.xyz * _OutLineStrength;
        //                float4 viewvertex = mul(UNITY_MATRIX_MV,v.vertex);
        //                float4x4 PMatrix = UNITY_MATRIX_P;
        //                PMatrix[0][2] = _ShiftX;
        //                PMatrix[1][2] = _ShiftY;
        //                o.vertex = mul(PMatrix,viewvertex);
        //                o.uv = v.uv * _MainTex_ST.xy + _MainTex_ST.zw;
        //                return o;
        //            }
        //
        //            float4 frag (v2f i) : SV_Target
        //            {
        //                float3 maintex = tex2D(_MainTex, i.uv);
        //                return float4(maintex * _OutLineColor,1);
        //            }
        //            ENDHLSL
        //        }
    }
    FallBack "Universal Render Pipeline/Lit"
}