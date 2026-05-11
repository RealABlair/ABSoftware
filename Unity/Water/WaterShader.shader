Shader "Custom/WaterShader"
{
    Properties
    {
        [Header(Base Color)]
        _WaterColor("Water Color", Color) = (0, 0.5, 1, 0.5)
        _DeepColor("Deep Water Color", Color) = (0, 0.2, 0.7, 0.66)
        _DepthFactor("Depth Factor", Range(0.1, 5.0)) = 3.0

        [Header(Visual Effects)]
        _Distortion("Distortion Strength", Range(0, 0.1)) = 0.006
        _FresnelPower("Fresnel Power", Range(0.1, 5)) = 1.0
        _NormalMap("Normal Map (Optional)", 2D) = "bump" {}
        _NormalMapSharpness("Normal Map Sharpness", Float) = 0.8
        _FlowSpeed("Flow Speed", Float) = 1.0

        [Header(Foam Settings)]
        _FoamColor("Foam Color", Color) = (1, 1, 1, 1)
        _FoamAmount("Foam Amount", Range(0, 1)) = 0.624

        [Header(Waves Geometry)]
        _WaveSpeed("Wave Speed", Float) = 0.5
        _WaveHeight("Wave Height", Float) = 0.05
        _WaveFrequency("Wave Frequency", Float) = 1.0

        [Header(Specular Sun)]
        _SpecularColor("Specular Color", Color) = (1, 1, 1, 1)
        _Gloss("Glossiness", Range(8, 256)) = 256

        [Header(Crest Foam Settings)]
        _CrestThreshold("Crest Start", Range(0, 1)) = 0.8
        _CrestSoftness("Crest Softness", Range(0.01, 0.5)) = 0.1
    }
    SubShader
    {
        Tags { "Queue" = "Transparent+1" "RenderType" = "Transparent" "LightMode" = "ForwardBase" }

        GrabPass { "_BackgroundTexture" }

        Pass
        {
            Cull Back
            //ZWrite Off
            ZWrite On
            ZTest LEqual
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            sampler2D _CameraDepthTexture;
            sampler2D _BackgroundTexture;
            sampler2D _NormalMap;
            float4 _NormalMap_ST;
            float _FlowSpeed;
            float _NormalMapSharpness;

            float4 _WaterColor, _DeepColor;
            float _DepthFactor, _Distortion, _FresnelPower;

            float4 _FoamColor;
            float _FoamAmount;
            
            float _WaveSpeed, _WaveHeight, _WaveFrequency;

            float4 _SpecularColor;
            float _Gloss;

            float _CrestThreshold;
            float _CrestSoftness;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float4 screenPos : TEXCOORD0;
                float2 uv : TEXCOORD1;
                float eyeDepth : TEXCOORD4;
                float3 lightDir : TEXCOORD5;
                float waveHeightFactor : TEXCOORD6;
                float3 worldNormal : TEXCOORD7;
                float3 worldPos : TEXCOORD8;
            };

            v2f vert(appdata v)
            {
                v2f o;
                float3 pos = v.vertex.xyz;
                float time = _Time.y * _WaveSpeed;
                float freq = _WaveFrequency;

                float w1 = sin(pos.x * freq + time);
                float w2 = sin((pos.x + pos.z) * freq * 0.7 + time * 1.3);
                float w3 = cos((pos.z - pos.x) * freq * 2.5 + time * 2.0);

                float wave = (w1 * 0.5 + w2 * 0.3 + w3 * 0.2) * _WaveHeight;

                float f1 = freq;
                float f2 = freq * 0.7;
                float f3 = freq * 2.5;

                float dx = (cos(pos.x * freq + time) * freq * 0.5) +
                    (cos((pos.x + pos.z) * (freq * 0.7) + time * 1.3) * (freq * 0.7) * 0.3) +
                    (sin((pos.z - pos.x) * (freq * 2.5) + time * 2.0) * (freq * 2.5) * 0.2);

                float dz = (cos((pos.x + pos.z) * (freq * 0.7) + time * 1.3) * (freq * 0.7) * 0.3) -
                    (sin((pos.z - pos.x) * (freq * 2.5) + time * 2.0) * (freq * 2.5) * 0.2);


                float3 normal = normalize(float3(-dx * _WaveHeight, 1.0, -dz * _WaveHeight));

                o.worldNormal = UnityObjectToWorldNormal(normal);

                v.vertex.y += wave;

                float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.pos = mul(UNITY_MATRIX_VP, worldPos);
                o.worldPos = worldPos.xyz;

                o.screenPos = ComputeScreenPos(o.pos);
                o.uv = TRANSFORM_TEX(v.uv, _NormalMap);
                o.lightDir = normalize(_WorldSpaceLightPos0.xyz);
                o.eyeDepth = -mul(UNITY_MATRIX_V, worldPos).z;

                o.waveHeightFactor = (wave / _WaveHeight) * 0.5 + 0.5;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float4 screenPos = i.screenPos;
                float waterZ = i.eyeDepth;
                float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                float3 halfDir = normalize(lightDir + viewDir);

                float2 wUV = i.worldPos.xz * 0.2;
                float2 flow = _Time.x * _WaveSpeed * _FlowSpeed;

                float3 n1 = UnpackNormal(tex2Dbias(_NormalMap, float4(wUV + flow, 0, -1.0)));
                float3 n2 = UnpackNormal(tex2Dbias(_NormalMap, float4(wUV * 1.2 - flow * 0.8, 0, -1.0)));
                float3 detailNormal = normalize(n1 + n2);

                float3 baseNormal = normalize(i.worldNormal);
                float3 combinedNormal = normalize(float3(
                    baseNormal.x + detailNormal.x * _NormalMapSharpness,
                    baseNormal.y,
                    baseNormal.z + detailNormal.y * _NormalMapSharpness
                    ));

                float4 distScreenPos = screenPos;
                distScreenPos.xy += (detailNormal.xy * _Distortion) * screenPos.w;

                float sceneZDist = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(distScreenPos)));
                if (sceneZDist < waterZ) distScreenPos = screenPos;

                float sceneZClean = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(screenPos)));
                float depthDiff = sceneZClean - waterZ;

                fixed4 bg = tex2Dproj(_BackgroundTexture, distScreenPos);
                float depthExp = saturate(depthDiff / _DepthFactor);
                float4 baseColor = lerp(_WaterColor, _DeepColor, depthExp);

                float waveRelief = smoothstep(0.0, 0.7, i.waveHeightFactor);
                float3 waveTint = lerp(_DeepColor.rgb, _WaterColor.rgb, waveRelief);

                float4 finalWaterColor = baseColor;
                finalWaterColor.rgb = waveTint;

                float ao = saturate(dot(combinedNormal, float3(0, 1, 0)));
                finalWaterColor.rgb *= (0.75 + 0.25 * ao);

                float finalAlpha = saturate(baseColor.a + (depthExp * 0.5));
                float4 final = lerp(bg, finalWaterColor, finalAlpha);

                float shoreFoam = 0;
                if (depthDiff > 0.01 && depthDiff < _FoamAmount)
                {
                    shoreFoam = 1.0 - saturate(depthDiff / _FoamAmount);
                    float noise = frac(sin(dot(i.worldPos.xz * 50, float2(12.9898, 78.233))) * 43758.5453) * 0.02;
                    shoreFoam = pow(saturate(shoreFoam - noise), 3.0);
                }

                float crestMask = smoothstep(_CrestThreshold, _CrestThreshold + _CrestSoftness, i.waveHeightFactor);
                float crestNoise = frac(sin(dot(i.worldPos.xz * 20, float2(12.9898, 78.233))) * 43758.5453);
                crestMask = saturate(crestMask - crestNoise * 0.2);

                float combinedFoam = saturate(shoreFoam + crestMask);
                final.rgb = lerp(final.rgb, _FoamColor.rgb, combinedFoam * _FoamColor.a);

                float specPower = pow(saturate(dot(combinedNormal, halfDir)), _Gloss);
                float specWide = pow(saturate(dot(combinedNormal, halfDir)), 8.0) * 0.3;
                float spec = (specPower + specWide);

                float fresnel = pow(1.0 - saturate(dot(combinedNormal, viewDir)), _FresnelPower);

                final.rgb = lerp(final.rgb, _SpecularColor.rgb * _LightColor0.rgb, fresnel * 0.4);
                final.rgb += spec * _SpecularColor.rgb * _LightColor0.rgb;

                final.a = saturate(finalWaterColor.a + fresnel);

                float3 skyFake = lerp(_WaterColor.rgb, _SpecularColor.rgb, saturate(combinedNormal.y));

                final.rgb = lerp(final.rgb, skyFake, fresnel * 0.5);

                return final;
            }
            ENDCG
        }
    }
    FallBack Off
}