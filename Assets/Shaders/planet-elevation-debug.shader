Shader "Planet/Elevation-Debug"
{
    Properties
    {
        _SeaRadius("Sea Radius", Float) = 637
        _MinElevation("Min Elevation", Float) = -5
        _MaxElevation("Max Elevation", Float) = 20

        _OceanColor("Ocean Color", Color) = (0.05, 0.18, 0.55, 1)
        _LowColor("Low Land Color", Color) = (0.1, 0.45, 0.12, 1)
        _MidColor("Mid Land Color", Color) = (0.55, 0.42, 0.18, 1)
        _HighColor("High Land Color", Color) = (0.95, 0.95, 0.9, 1)

        _ContourColor("Contour Color", Color) = (0, 0, 0, 1)
        _ContourStep("Contour Step", Float) = 2
        _ContourStrength("Contour Strength", Range(0, 1)) = 0.25
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Opaque"
            "Queue" = "Geometry"
        }

        Pass
        {
            Name "ElevationDebug"

            HLSLPROGRAM

            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 positionOS : TEXCOORD0;
            };

            CBUFFER_START(UnityPerMaterial)

            float _SeaRadius;
            float _MinElevation;
            float _MaxElevation;

            float4 _OceanColor;
            float4 _LowColor;
            float4 _MidColor;
            float4 _HighColor;

            float4 _ContourColor;
            float _ContourStep;
            float _ContourStrength;

            CBUFFER_END

            Varyings Vert(Attributes input)
            {
                Varyings output;

                output.positionHCS = TransformObjectToHClip(input.positionOS.xyz);
                output.positionOS = input.positionOS.xyz;

                return output;
            }

            half4 Frag(Varyings input) : SV_Target
            {
                float radius = length(input.positionOS);
                float elevation = radius - _SeaRadius;

                float elevationRange = max(0.0001, _MaxElevation - _MinElevation);
                float t = saturate((elevation - _MinElevation) / elevationRange);

                float4 lowToMid = lerp(_LowColor, _MidColor, saturate(t * 2.0));
                float4 midToHigh = lerp(_MidColor, _HighColor, saturate((t - 0.5) * 2.0));
                float4 landColor = lerp(lowToMid, midToHigh, step(0.5, t));

                float4 color = elevation < 0.0 ? _OceanColor : landColor;

                float contourStep = max(0.0001, _ContourStep);
                float contourValue = abs(frac(elevation / contourStep) - 0.5);
                float contour = 1.0 - smoothstep(0.0, 0.04, contourValue);

                color.rgb = lerp(color.rgb, _ContourColor.rgb, contour * _ContourStrength);

                return color;
            }

            ENDHLSL
        }
    }
}