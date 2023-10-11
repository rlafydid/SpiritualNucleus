Shader "ORII/Role/Unlit/Role_Unlit_Body"
{
    Properties
    {
        [HDR]_MetalBrightColor ("MetalBrightColor", Color) = (1,1,1,1)
        [HDR]_MetalDarkColor ("MetalDarkColor", Color) = (0,0,0,0)
        [HDR]_HandBrightColor ("HandBrightColor", Color) = (1,1,1,1)
        [HDR]_HandDarkColor ("HandDarkColor", Color) = (0,0,0,0)
        [HDR]_SoftBrightColor ("SoftBrightColor", Color) = (1,1,1,1)
        [HDR]_SoftDarkColor ("SoftDarkColor", Color) = (0,0,0,0)
        [HDR]_SilkBrightColor ("SilkBrightColor", Color) = (1,1,1,1)
        [HDR]_SilkDarkColor ("SilkDarkColor", Color) = (0,0,0,0)
        [HDR]_SkinBrightColor ("SkinBrightColor", Color) = (1,1,1,1)
        [HDR]_SkinDarkColor ("SkinDarkColor", Color) = (0,0,0,0)
        [HDR] _EmissionColor1("Emission Color (Aplha=1)", Color) = (0,0,0)
        [HDR] _EmissionColor2("Emission Color (Aplha=0.5)", Color) = (0,0,0)
        _MainTex ("MainTex", 2D) = "white" {}
        _DarkExcessive ("DarkExcessive", Range(0,1)) = 0.5
        _DarkStrength ("DarkStrength", Range(0,1)) = 0.5
        [HDR]_SpecularColor ("SpecularColor", Color) = (1,1,1,1)
        _SpecularExcessive ("SpecularExcessive", Range(0,1)) = 0.5
        _SpecularStrength ("SpecularStrength", Range(0,1)) = 0.5
        _CubeMap ("CubeMap", CUBE) = "" {}
        _CubeMapStrength ("CubeMapStrength", Range(0,10)) = 1
        _LightMap ("LightMap", 2D) = "white" {}
        [HDR]_FresnelColor ("FresnelColor", Color) = (1,1,1,1)
        _Fresnel ("Fresnel", Range(0,10)) = 1
        _FresnelExcessive ("FresnelExcessive", Range(0,1)) = 0.5
        _FresnelStrength ("FresnelStrength", Range(0,1)) = 0.5
        [HDR]_OutLineColor ("OutLineColor", Color) = (0,0,0,0)
        _OutLineStrength ("OutLineStrength", float) = 0.002
        _ShiftX ("ShiftX", float) = 0
        _ShiftY ("ShiftY", float) = 0
        [Enum]_Cull ("Cull", Int) = 2
        _LightDir ("LighrDir", Vector) = (0.5,0.5,0,0)
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline" "RenderType"="Opaque" "Queue"="Geometry-20"
        }
        LOD 100
        Cull [_Cull]

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
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 worldvertex : TEXCOORD1;
                float3 worldnormaldir : TEXCOORD2;
            };

            CBUFFER_START(UnityPerMaterial)
            float4 _MainTex_ST;
            half3 _LightDir;
            half3 _MetalBrightColor;
            half3 _MetalDarkColor;
            half3 _HandBrightColor;
            half3 _HandDarkColor;
            half3 _SoftBrightColor;
            half3 _SoftDarkColor;
            half3 _SilkBrightColor;
            half3 _SilkDarkColor;
            half3 _SkinBrightColor;
            half3 _SkinDarkColor;
            half3 _SpecularColor;
            half3 _FresnelColor;
            half3 _OutLineColor;
            half3 _EmissionColor1;
            half3 _EmissionColor2;
            half _DarkExcessive;
            half _DarkStrength;
            half _SpecularExcessive;
            half _SpecularStrength;
            half _CubeMapStrength;
            half _Fresnel;
            half _FresnelExcessive;
            half _FresnelStrength;
            half _ShiftX;
            half _ShiftY;
            half _OutLineStrength;
            CBUFFER_END

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_LightMap);
            TEXTURECUBE(_CubeMap);
            SAMPLER(sampler_CubeMap);

            v2f vert(appdata v)
            {
                v2f o;
                half4 viewvertex = mul(UNITY_MATRIX_MV, v.vertex);
                half4x4 PMatrix = UNITY_MATRIX_P;
                PMatrix[0][2] = _ShiftX;
                PMatrix[1][2] = _ShiftY;
                o.vertex = mul(PMatrix, viewvertex);
                o.uv = v.uv * _MainTex_ST.xy + _MainTex_ST.zw;
                o.worldvertex = TransformObjectToWorld(v.vertex);
                o.worldnormaldir = normalize(TransformObjectToWorldDir(v.normal));
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                float4 maintex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                half4 lightmaptex = SAMPLE_TEXTURE2D(_LightMap, sampler_MainTex, i.uv);
                half3 BrightColor;
                half3 DarkColor;
                if (lightmaptex.a >= 0 && lightmaptex.a <= 0.15)
                {
                    BrightColor = _MetalBrightColor;
                    DarkColor = _MetalDarkColor;
                }
                if (lightmaptex.a >= 0.2 && lightmaptex.a <= 0.35)
                {
                    BrightColor = _HandBrightColor;
                    DarkColor = _HandDarkColor;
                }
                if (lightmaptex.a >= 0.4 && lightmaptex.a <= 0.55)
                {
                    BrightColor = _SoftBrightColor;
                    DarkColor = _SoftDarkColor;
                }
                if (lightmaptex.a >= 0.6 && lightmaptex.a <= 0.75)
                {
                    BrightColor = _SilkBrightColor;
                    DarkColor = _SilkDarkColor;
                }
                if (lightmaptex.a >= 0.8 && lightmaptex.a <= 1)
                {
                    BrightColor = _SkinBrightColor;
                    DarkColor = _SkinDarkColor;
                }
                half3 worldlightdir = normalize(_LightDir);
                // Light light = GetMainLight();
                // half3 worldlightdir = normalize(light.direction);
                half NdotL = smoothstep(_DarkStrength - _DarkExcessive, _DarkStrength + _DarkExcessive, (max(dot(i.worldnormaldir, worldlightdir), 0) * 0.5 + 0.5) + lightmaptex.g);
                half3 lambert = lerp(DarkColor, BrightColor, NdotL) * maintex.rgb;
                half3 worldviewdir = normalize(_WorldSpaceCameraPos.xyz - i.worldvertex);
                half4 cubemap = SAMPLE_TEXTURECUBE(_CubeMap, sampler_CubeMap, reflect(-worldviewdir, i.worldnormaldir)) * _CubeMapStrength;
                half NdotH = max(dot(i.worldnormaldir, normalize(worldlightdir + worldviewdir)), 0);
                half3 specular = smoothstep(_SpecularStrength - _SpecularExcessive, _SpecularStrength + _SpecularExcessive,
                                            (NdotH + max(dot(i.worldnormaldir.xz, worldlightdir.xz), 0)) * lightmaptex.b * lightmaptex.r) * _SpecularColor * cubemap.xyz;
                half NdotV = pow(1 - max(dot(i.worldnormaldir, worldviewdir), 0), _Fresnel);
                half3 fresnel = smoothstep(_FresnelStrength - _FresnelExcessive, _FresnelStrength + _FresnelExcessive, NdotV) * _FresnelColor * lambert;

                half setpEnisson = (1 - step(maintex.a, 0.55));
                half3 _Emission1 = setpEnisson * _EmissionColor1;
                half3 _Emission2 = ((step(0.45, maintex.a)) - setpEnisson) * _EmissionColor2;
                half3 finalcolor = lambert + specular + fresnel + _Emission1 + _Emission2;
                return half4(finalcolor, 1);
            }
            ENDHLSL
        }

        Pass
        {
            Name "Outline"
            Tags
            {
                "LightMode"="Outline"
            }
            Cull Front
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float4 texcoord0 : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
            };

            CBUFFER_START(UnityPerMaterial)
            float4 _MainTex_ST;
            half3 _LightDir;
            half3 _MetalBrightColor;
            half3 _MetalDarkColor;
            half3 _HandBrightColor;
            half3 _HandDarkColor;
            half3 _SoftBrightColor;
            half3 _SoftDarkColor;
            half3 _SilkBrightColor;
            half3 _SilkDarkColor;
            half3 _SkinBrightColor;
            half3 _SkinDarkColor;
            half3 _SpecularColor;
            half3 _FresnelColor;
            half3 _OutLineColor;
            half3 _EmissionColor1;
            half3 _EmissionColor2;
            half _DarkExcessive;
            half _DarkStrength;
            half _SpecularExcessive;
            half _SpecularStrength;
            half _CubeMapStrength;
            half _Fresnel;
            half _FresnelExcessive;
            half _FresnelStrength;
            half _ShiftX;
            half _ShiftY;
            half _OutLineStrength;
            CBUFFER_END

            half3 decode(half3 enc)
            {
                half scale = 1.7777;
                half3 nn = enc.xyz * half3(2 * scale, 2 * scale, 0) + half3(-scale, -scale, 1);
                half g = 2.0 / dot(nn.xy, nn.xy);
                half3 n;
                n.xy = g * nn.xy;
                n.z = g - 1;
                return n;
            }

            v2f vert(appdata v)
            {
                v2f o;
                // float3 normal = (v.tangent.xyz*2) - 1;

                // v.vertex = v.vertex + float4(normal.xyz,0) * _OutLineStrength  * 0.01;
                // v.vertex.xyz += normal.xyz * _OutLineStrength;

                half3 n;
                //unpack uvs
                v.texcoord0.z = v.texcoord0.z * 255.0 / 16.0;
                n.x = floor(v.texcoord0.z) / 15.0;
                n.y = frac(v.texcoord0.z) * 16.0 / 15.0;
                //- get z
                n.z = v.texcoord0.w;
                //- transform
                n = n * 2 - 1;

                half3 vertNormal = n;

                //从顶点颜色中读取法线信息，并将其值范围从0~1还原为-1~1
                // float3 vertNormal = v.vertexColor.rgb * 2 - 1;
                //使用法线与切线叉乘计算副切线用于构建切线→模型空间转换矩阵
                half3 bitangent = cross(v.normal, v.tangent.xyz) * v.tangent.w * unity_WorldTransformParams.w;
                //构建切线→模型空间转换矩阵
                half3x3 TtoO = half3x3(v.tangent.x, bitangent.x, v.normal.x,
                                       v.tangent.y, bitangent.y, v.normal.y,
                                       v.tangent.z, bitangent.z, v.normal.z);
                //将法线转换到模型空间下
                vertNormal = mul(TtoO, vertNormal);
                //模型坐标 + 法线 * 自定义粗细值 * 顶点颜色A通道 = 轮廓线模型					
                o.vertex = TransformObjectToHClip(v.vertex.xyz + vertNormal * _OutLineStrength);
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                return half4(_OutLineColor, 1);
            }
            ENDHLSL
        }
    }
    FallBack "Universal Render Pipeline/Lit"
}