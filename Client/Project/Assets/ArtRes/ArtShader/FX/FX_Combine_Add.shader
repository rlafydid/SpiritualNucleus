Shader "ORII/FX/FX_Combine_Add"
{
    Properties
    {
        //[Header(Baisc Module)]
        //[Space(5)]
        _TintColor ("Tint Color", Color) = (1, 1, 1, 1)
        _MainTex ("Main Tex", 2D) = "white" { }//主材质
        _glow ("glow", Float) = 1//辉光度
        [Toggle(ENABLE_UICLIP)]_UIClip ("UIClip", Float) = 0
        [HideInInspector] _ClipRange0 ("Clip Range", Vector) = (0, 0, 0, 0)
        [Toggle]_UseCustomData1xy ("Use CustomData1 xy", Float) = 0
        //[Toggle(USECUSTOMDATALXY)]_UseCustomData1xy ("Use CustomData1 xy", Float) = 0//是否连接CustomData1的xy         //-------------------DOD
        //[Space(10)]
        //[Header(RimLit Module)]//菲涅尔选项
        //[Space(5)]
        [Toggle(FX_FRESNEL_RIMLIGHT)]_UseFresnel ("Use Fresnel", Float) = 0//是否开启边缘光
        _FresnelStr ("Fresnel Strength", Range(0, 10)) = 6.128205//菲涅尔强度
        //[Space(10)]
        //[Header(UVTransform Module)]//UV运动选项
        //[Space(5)]
        [Toggle(FX_UVTRANSFORM)]_UseUVTran ("Use UV Transform", Float) = 0//是否开启UV运动
        _U ("U Speed", Float) = 0
        _V ("V Speed", Float) = 0
        //[Space(10)]
        //[Header(Distort Module)]//扰动选项
        //[Space(5)]
        [Toggle(FX_DISTORT)]_UseDistort ("Use Distort", Float) = 0//是否开启扰动
        //[Toggle]_DistortAddtive ("Use Distort Addtive", Float) = 0//是否开启时间函数
        _DistortTex ("Distort Tex", 2D) = "white" { }//扭曲纹理
        _DistortStr ("Distort Strength", Float) = 0.05//扭曲强度
        _U_twi ("Distort U Speed", Float) = 0
        _V_twi ("Distort V Speed", Float) = 0
        //[Space(10)]
        //[Header(Dissolve Module)]//溶解
        //[Space(5)]
        [Toggle(FX_DISSOLVE)]_UseDissolve ("Use Dissolve", Float) = 0//是否开启扰动
        [Toggle]_DissolveRangeUseCustomData ("Dissolve Range Use CustomData[CustomData.z]", Float) = 0//溶解周期连接customData1
        _DissolveTex ("Dissolve Tex", 2D) = "white" { }
        _DissolveStr ("Dissolve Strength", Range(0, 1)) = 0
        _DissolveRange ("Dissolve Range", Range(0, 1)) = 0
        _DissolveRimStr ("Dissolve Rimlit", Range(0, 1)) = 0
        //[Space(10)]
        //[Header(Mask Module)]//遮罩图使用
        //[Space(5)]
        [Toggle(FX_MASK)]_UseMask ("Use Mask", Float) = 0//是否开启遮罩
        [Toggle]_UseCustomDataMaskUV ("Use CustomData maskUV", Float) = 1 //默认  CustomData 会同时控制主贴图和Mask贴图的UV运动 关闭后mask将不再被控制移动

        _isLux ("Use Mask GLOW", Float) = 0
        _Lux ("Mask GLOW", Range(0, 7)) = 0
        _MaskTex ("Mask Tex", 2D) = "white" { }
        _U_Mask ("Mask U Speed", Float) = 0
        _V_Mask ("Mask V Speed", Float) = 0

        [HideInInspector] _Cull ("Culling Switch", Float) = 0
        [HideInInspector] _ZTest ("ZTest Switch", Float) = 4
        [HideInInspector] _ZWrite ("ZWrite Switch", Float) = 0
    }
    SubShader
    {
        //透明物体的渲染队列
        Tags
        {
            "IgnoreProjector" = "True" "Queue" = "Transparent" "RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline"
        }

        Pass
        {
            //Pass名字
            Name "Particle_Forward_CombineAdd"
            Tags
            {
                "LightMode" = "UniversalForward"
            }

            //Alpha混合
            //Blend SrcAlpha OneMinusSrcAlpha
            Blend One One

            //双面渲染，不写入深度
            Cull [_Cull]
            ZWrite [_ZWrite]
            //保证一直位于最上层
            ZTest [_ZTest]
            Lighting Off
            Fog
            {
                Mode Off
            }
            //HLSL代码段
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            #pragma shader_feature_local FX_FRESNEL_RIMLIGHT
            #pragma shader_feature_local ENABLE_UICLIP
            #pragma shader_feature_local FX_UVTRANSFORM
            #pragma shader_feature_local FX_DISTORT
            #pragma shader_feature_local FX_DISSOLVE
            #pragma shader_feature_local FX_MASK

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_DistortTex);
            SAMPLER(sampler_DistortTex);
            TEXTURE2D(_DissolveTex);
            SAMPLER(sampler_DissolveTex);
            TEXTURE2D(_MaskTex);
            SAMPLER(sampler_MaskTex);

            //变量声明
            //基础变量
            CBUFFER_START(UnityPerMaterial)
            float4x4 _WorldToPanelMatrix;
            float4 _MainTex_ST;
            float4 _DissolveTex_ST;
            float4 _ClipRange0 = float4(0.0, 0.0, 0.1, 0.1);
            float4 _DistortTex_ST;
            float4 _MaskTex_ST;
            half4 _TintColor;
            float _glow;
            float _UseUVTran;
            float _U;
            float _V;
            half _UseCustomData1xy;
            half _UseFresnel;
            half _FresnelStr;
            half _U_twi;
            half _V_twi;
            half _UseDistort;
            half _DistortStr;
            half _DissolveRangeUseCustomData;
            half _UseDissolve;
            half _DissolveStr;
            half _DissolveRange;
            half _DissolveRimStr;
            half _isLux;
            half _Lux;
            half _UseMask;
            half _UseCustomDataMaskUV;
            half _U_Mask;
            half _V_Mask;
            CBUFFER_END

            //顶点数据结构体
            struct a2v
            {
                float4 vertex : POSITION;
                half4 texcoord0 : TEXCOORD0;
                half2 customData1 : TEXCOORD1;
                half4 vertexColor : COLOR;

                //使用菲涅尔项——边缘光
                #if FX_FRESNEL_RIMLIGHT
                half3 normal : NORMAL;
                #endif
            };

            //顶点着色器结构体
            struct v2f
            {
                float4 pos : SV_POSITION;
                half4 uv0 : TEXCOORD0; // xy(uv0) zw(uv1)
                half4 vertexColor : COLOR;
                float4 worldPos : TEXCOORD1;
                //使用菲涅尔项——边缘光
                float3 normalDir : TEXCOORD2;

                half4 customData1 : TEXCOORD3;
            };

            //顶点着色器
            v2f vert(a2v v)
            {
                v2f o = (v2f)0;

                //#if USECUSTOMDATALXY             //-------------------DOD
                o.uv0.xy = v.texcoord0.xy + v.texcoord0.zw * _UseCustomData1xy; //连接CustomData1的xy
                o.uv0.zw = v.texcoord0.xy + (v.texcoord0.zw * _UseCustomData1xy * _UseCustomDataMaskUV);
                //#endif
                o.vertexColor = v.vertexColor;
                o.pos = TransformObjectToHClip(v.vertex.xyz);
                o.worldPos.xyz = TransformObjectToWorld(v.vertex.xyz);
                o.customData1.xy = v.texcoord0.zw;
                o.customData1.zw = v.customData1.xy;

                //使用UI裁切
                #if ENABLE_UICLIP
                float3 vertexWorldPos = TransformObjectToWorld(v.vertex.xyz);
                float4 panelPos = mul(_WorldToPanelMatrix, vertexWorldPos);
                panelPos /= panelPos.w;
                o.worldPos.xy = panelPos.xy * _ClipRange0.zw + _ClipRange0.xy;
                #endif

                //法线相关计算——边缘光
                #if FX_FRESNEL_RIMLIGHT
                o.normalDir = TransformObjectToWorldNormal(v.normal);
                #endif

                return o;
            }

            //片元着色器
            half4 frag(v2f i, float facing : VFACE) : COLOR
            {
                half isFrontFace = (facing >= 0 ? 1 : 0);
                half faceSign = (facing >= 0 ? 1 : - 1);
                float2 UV = i.uv0.xy;
                //使用UV运动                 //----------------DOD
                #if FX_UVTRANSFORM
                UV = ((float2((_Time.g * _U), (_Time.g * _V)) + i.uv0));
                #endif


                #if FX_DISTORT//启用扭曲                //----------------DOD
                float2 flowUV_twi = (float2((_Time.g * _U_twi), (_Time.g * _V_twi)) + UV);
                half4 _Distort_tex_var = SAMPLE_TEXTURE2D(_DistortTex, sampler_DistortTex,
                                                          TRANSFORM_TEX(flowUV_twi, _DistortTex));
                UV = (_Distort_tex_var.r * _DistortStr + UV);

                #endif

                //_MaskTex_ST.x = _MaskTex_ST.x;
                half4 _Texture_var = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, TRANSFORM_TEX(UV, _MainTex));

                half3 emissive = (_Texture_var.rgb * _Texture_var.a * i.vertexColor.rgb * i.vertexColor.a * (_TintColor.
                    rgb * _glow));
                half4 finalColor = half4(emissive, 1.0);


                #if FX_MASK
                float2 flowUV_mask = (float2((_Time.g * _U_Mask), (_Time.g * _V_Mask)) + i.uv0.zw);
                half4 _Mask_var = SAMPLE_TEXTURE2D(_MaskTex, sampler_MaskTex, TRANSFORM_TEX(flowUV_mask, _MaskTex));
                #endif

                //使用UV运动
                #if FX_UVTRANSFORM
                emissive = (_Texture_var.rgb * _Texture_var.a * i.vertexColor.rgb * i.vertexColor.a * (_TintColor.rgb *
                    _glow));
                finalColor = half4(emissive, 1.0);
                #endif


                #if FX_DISSOLVE //启用溶解

                //连接CustomData
                _DissolveRange = lerp(_DissolveRange, i.customData1.z, _DissolveRangeUseCustomData);

                float4 _Dissolve_var = SAMPLE_TEXTURE2D(_DissolveTex, sampler_DissolveTex,
                                                        TRANSFORM_TEX(i.uv0, _DissolveTex));
                _Dissolve_var = 1 - _Dissolve_var;
                half rimflag = _DissolveRimStr;
                //第一步:获取01黑白通道图
                half3 RimDissolveMap_var = step((_Dissolve_var.r + (_DissolveRange - 0.5) * 2 / 1.8), 0.5);
                //第二步:双插边缘
                _DissolveRimStr *= 0.5;
                _DissolveRimStr -= 0.0001;
                _DissolveRimStr = 1.2 * (1 - _DissolveRimStr) - 0.6;
                float RimlerpA = step(_Dissolve_var.r + (_DissolveRange - 0.5) * 2 / 1.8, _DissolveRimStr);
                float RimlerpB = step(_DissolveRimStr, _Dissolve_var.r + (_DissolveRange - 0.5) * 2 / 1.8);


                //_DissolveRange ;
                _DissolveStr += 0.025;
                _DissolveStr *= 64;
                float DissolveValue = 1 - saturate(
                    ((_Dissolve_var.r * _DissolveStr) - lerp(_DissolveStr, -1, _DissolveRange)));
                DissolveValue = max(step(0.0001, rimflag), DissolveValue);

                //finalColor.rgb = float3(1,1,1)*lerp(RimlerpA,1.0,RimlerpA*RimlerpB);
                finalColor.rgb += (2 * _glow * finalColor.rgb * (1 - lerp(RimlerpA, 1.0, RimlerpA * RimlerpB)) *
                    RimDissolveMap_var) * step(0.0001, _DissolveRimStr) * step(0.00001, _DissolveRange);
                finalColor.rgb *= (DissolveValue) * max(step(rimflag, 0.0001), RimDissolveMap_var);
                //finalColor.rgb += 2*_glow*finalColor.rgb*lerp(RimlerpA,1.0,RimlerpA*RimlerpB);
                #endif

                //使用UI裁切
                #if ENABLE_UICLIP
                float2 factor = (float2(1.0, 1.0) - abs(i.worldPos));
                finalColor *= step(0, min(factor.x, factor.y));
                #endif

                //法线相关计算——边缘光
                #if FX_FRESNEL_RIMLIGHT
                i.normalDir = normalize(i.normalDir); //归一化法线
                i.normalDir *= faceSign;
                half3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.worldPos.xyz);
                half FresnelPower = pow(abs(1.0 - max(0, dot(i.normalDir, viewDirection))), _FresnelStr);
                finalColor *= FresnelPower;
                #endif

                #if FX_MASK
                finalColor *= lerp(_Mask_var.r, _isLux, _isLux);
                //#if USECUSTOMDATALXY                         //-------------------DOD
                // finalColor *= lerp(_Mask_var.r, lerp(0, _Lux, _isLux), _isLux) * lerp(
                //     1, i.customData1.w, _UseCustomData1xy);
                //#endif
                finalColor += (_Mask_var.r * _Lux * finalColor) * lerp(0, _Lux, _isLux);
                #endif

                #if  FX_DISTORT
                finalColor *= _Distort_tex_var;
                #endif
                return finalColor;
            }
            ENDHLSL
        }
    }
    CustomEditor "EffectMaterialGUI_Combine_Additive"
}