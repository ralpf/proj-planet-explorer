Shader "Planet/Tectonic Plates Debug"
{
    Properties
    {
        _Cubemap ("Plate Map", Cube) = "" {}
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Geometry"
        }

        Pass
        {
            Name "PlateOwnershipDebug"

            HLSLPROGRAM

            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURECUBE(_Cubemap);
            SAMPLER(sampler_Cubemap);

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 directionOS : TEXCOORD0;
            };

            Varyings Vert(Attributes input)
            {
                Varyings output;

                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.directionOS = normalize(input.positionOS.xyz);

                return output;
            }

            half4 Frag(Varyings input) : SV_Target
            {
                float3 direction = normalize(input.directionOS);
                half4 color = SAMPLE_TEXTURECUBE(_Cubemap, sampler_Cubemap, direction);

                return color;
            }

            ENDHLSL
        }
    }
}