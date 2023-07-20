// 源地址 : https://github.com/ColinLeung-NiloCat/UnityURPUnlitScreenSpaceDecalShader

Shader "ORII/UI/Effect/Decal"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" { }
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Overlay" "Queue" = "Transparent-499" "DisableBatching" = "True"
        }

        Pass
        {
            ZWrite off
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
            

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata
            {
                float3 positionOS : POSITION;
            };

            struct v2f
            {
                float4 positionCS : SV_POSITION;
                float4 screenPos : TEXCOORD0;
                float4 viewRayOS : TEXCOORD1;
                float4 cameraPosOSAndFogFactor : TEXCOORD2;
            };

            sampler2D _MainTex;
            sampler2D _CameraDepthTexture;

            CBUFFER_START(UnityPerMaterial)
            float4 _MainTex_ST;
            half _MulAlphaToRGB;
            CBUFFER_END

            v2f vert(appdata input)
            {
                v2f o;
                VertexPositionInputs vertexPositionInput = GetVertexPositionInputs(input.positionOS);
                o.positionCS = vertexPositionInput.positionCS;
                o.cameraPosOSAndFogFactor.a = 0;
                o.screenPos = ComputeScreenPos(o.positionCS);
                o.viewRayOS.w = vertexPositionInput.positionVS.z;

                float3 viewRay = vertexPositionInput.positionVS * - 1;
                float4x4 ViewToObjectMatrix = mul(UNITY_MATRIX_I_M, UNITY_MATRIX_I_V);
                o.viewRayOS.xyz = mul((float3x3)ViewToObjectMatrix, viewRay);
                o.cameraPosOSAndFogFactor.xyz = mul(ViewToObjectMatrix, float4(0, 0, 0, 1)).xyz;

                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                i.viewRayOS.xyz /= i.viewRayOS.w;

                float2 screenSpaceUV = i.screenPos.xy / i.screenPos.w;
                float sceneRawDepth = tex2D(_CameraDepthTexture, screenSpaceUV).r;
                float3 decalSpaceScenePos;
                float sceneDepthVS;

                if (unity_OrthoParams.w)
                {
                    sceneRawDepth = 1 - sceneRawDepth;
                    #if UNITY_REVERSED_Z
                    sceneDepthVS = lerp(_ProjectionParams.y, _ProjectionParams.z, sceneRawDepth);
                    #else
                            sceneDepthVS = lerp(_ProjectionParams.z, _ProjectionParams.y, sceneRawDepth);
                    #endif
                    float2 viewRayEndPosVS_xy = float2(unity_OrthoParams.xy * (i.screenPos.xy - 0.5) * 2);
                    float4 vposOrtho = float4(viewRayEndPosVS_xy, -sceneDepthVS, 1);
                    float3 wposOrtho = mul(UNITY_MATRIX_I_V, vposOrtho).xyz;
                    decalSpaceScenePos = mul(GetWorldToObjectMatrix(), float4(wposOrtho, 1)).xyz;
                }
                else
                {
                    sceneDepthVS = LinearEyeDepth(sceneRawDepth, _ZBufferParams);
                    decalSpaceScenePos = i.cameraPosOSAndFogFactor.xyz + i.viewRayOS.xyz * sceneDepthVS;
                }


                float2 decalSpaceUV = decalSpaceScenePos.xy + 0.5;
                float shouldClip = 0;

                clip(0.5 - abs(decalSpaceScenePos) - shouldClip);

                float2 uv = decalSpaceUV.xy * _MainTex_ST.xy + _MainTex_ST.zw;
                half4 col = tex2D(_MainTex, uv);
                col.rgb *= lerp(1, col.a, _MulAlphaToRGB);
                return col;
            }
            ENDHLSL
        }
    }
}