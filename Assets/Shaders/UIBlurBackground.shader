Shader "UI/BlurBackground"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _Size ("Blur Size", Range(0, 8)) = 2
        _Iterations ("Blur Iterations (quality)", Range(1, 3)) = 2

        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            Name "UIBlurBackground"

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            // Текстура сцены после непрозрачных объектов.
            // Требует включённой опции "Opaque Texture" в URP Asset (Renderer)!
            TEXTURE2D(_CameraOpaqueTexture);
            SAMPLER(sampler_CameraOpaqueTexture);

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 positionHCS : SV_POSITION;
                half4 color       : COLOR;
                float2 texcoord    : TEXCOORD0;
            };

            CBUFFER_START(UnityPerMaterial)
                half4 _Color;
                float _Size;
                float _Iterations;
            CBUFFER_END

            v2f vert(appdata_t v)
            {
                v2f OUT;
                OUT.positionHCS = TransformObjectToHClip(v.vertex.xyz);
                OUT.color = v.color * _Color;
                OUT.texcoord = v.texcoord;
                return OUT;
            }

            // Приводим позицию из SV_POSITION (пиксели экрана) к нормализованным UV [0..1]
            float2 GetScreenUV(float4 positionHCS)
            {
                float2 uv = positionHCS.xy / _ScreenParams.xy;
                #if UNITY_UV_STARTS_AT_TOP
                if (_ProjectionParams.x < 0)
                    uv.y = 1 - uv.y;
                #endif
                return uv;
            }

            // Размытие по кругу, несколько итераций для более сильного эффекта
            half4 Blur(float2 screenUV, float2 texel)
            {
                half4 sum = half4(0,0,0,0);
                const int SAMPLES = 12;
                const float PI2 = 6.28318530718;
                int totalSamples = 0;

                for (int it = 1; it <= (int)_Iterations; it++)
                {
                    float radius = it;
                    for (int i = 0; i < SAMPLES; i++)
                    {
                        float angle = (PI2 / SAMPLES) * i;
                        float2 offset = float2(cos(angle), sin(angle)) * texel * radius;
                        sum += SAMPLE_TEXTURE2D(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, screenUV + offset);
                        totalSamples++;
                    }
                }

                // центральный сэмпл
                sum += SAMPLE_TEXTURE2D(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, screenUV);
                totalSamples++;

                return sum / totalSamples;
            }

            half4 frag(v2f IN) : SV_Target
            {
                float2 screenUV = GetScreenUV(IN.positionHCS);
                float2 texel = (_ScreenParams.zw - 1.0) * _Size; // 1/width, 1/height * Size

                half4 blurred = Blur(screenUV, texel);

                blurred.a *= IN.color.a;
                blurred.rgb *= IN.color.rgb;

                return blurred;
            }
            ENDHLSL
        }
    }
}
