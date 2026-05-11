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

        [Header(Foam Settings)]
        _FoamColor("Foam Color", Color) = (1, 1, 1, 1)
        _FoamAmount("Foam Amount", Range(0, 1)) = 0.624

        [Header(Waves Geometry)]
        _WaveSpeed("Wave Speed", Float) = 0.5
        _WaveHeight("Wave Height", Float) = 0.05
        _WaveFrequency("Wave Frequency", Float) = 1.0
        _WaveScale("Wave Scale", Float) = 1.0

        [Header(Specular Sun)]
        _SpecularColor("Specular Color", Color) = (1, 1, 1, 1)
        _Gloss("Glossiness", Range(8, 256)) = 256
    }
    SubShader
    {
        Tags { "Queue" = "Transparent+1" "RenderType" = "Transparent" "LightMode" = "ForwardBase" }

        GrabPass { "_BackgroundTexture" }

        Pass
        {
            ZWrite Off
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

            float4 _WaterColor, _DeepColor;
            float _DepthFactor, _Distortion, _FresnelPower;

            float4 _FoamColor;
            float _FoamAmount;
            
            float _WaveSpeed, _WaveHeight, _WaveFrequency, _WaveScale;

            float4 _SpecularColor;
            float _Gloss;

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
                float3 viewDir : TEXCOORD3;
                float eyeDepth : TEXCOORD4;
                float3 lightDir : TEXCOORD5;
            };

            v2f vert(appdata v)
            {
                v2f o;

                float3 pos = v.vertex.xyz;
                float time = _Time.y * _WaveSpeed;
                float freq = _WaveScale * _WaveFrequency;

                float wave = (sin(pos.x * freq + time) + cos(pos.z * (freq * 0.8) + time * 1.5)) * _WaveHeight;
                v.vertex.y += wave;

                o.pos = UnityObjectToClipPos(v.vertex);
                o.screenPos = ComputeScreenPos(o.pos);
                o.uv = TRANSFORM_TEX(v.uv, _NormalMap);

                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.viewDir = normalize(_WorldSpaceCameraPos - worldPos);
                o.lightDir = normalize(_WorldSpaceLightPos0.xyz);
                COMPUTE_EYEDEPTH(o.eyeDepth);

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float4 screenPos = i.screenPos;
                float waterZ = i.eyeDepth;

                float2 distortUV = i.uv * 5.0 + _Time.x * _WaveSpeed;
                float2 distort = float2(sin(distortUV.x), cos(distortUV.y)) * _Distortion;

                float4 distScreenPos = screenPos;
                distScreenPos.xy += distort * screenPos.w;

                float sceneZDist = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(distScreenPos)));
                if (sceneZDist < waterZ) distScreenPos = screenPos;

                float sceneZClean = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(screenPos)));
                float depthDiff = sceneZClean - waterZ;

                fixed4 bg = tex2Dproj(_BackgroundTexture, distScreenPos);
                float depthExp = saturate(depthDiff / _DepthFactor);
                float4 waterColor = lerp(_WaterColor, _DeepColor, depthExp);
                float4 final = lerp(bg, waterColor, waterColor.a);

                float foamMask = 0;
                if (depthDiff > 0.01 && depthDiff < _FoamAmount)
                {
                    foamMask = 1.0 - saturate(depthDiff / _FoamAmount);
                    float noise = frac(sin(dot(i.uv ,float2(12.9898,78.233))) * 43758.5453) * 0.02;
                    foamMask = pow(saturate(foamMask - noise), 3.0);
                }
                final.rgb = lerp(final.rgb, _FoamColor.rgb, foamMask * _FoamColor.a);

                float3 viewDir = normalize(i.viewDir);
                float fresnel = pow(1.0 - saturate(dot(float3(0,1,0), viewDir)), _FresnelPower);
                final.a = saturate(waterColor.a + fresnel);

                float3 waveNormal = normalize(float3(sin(i.uv.x * 50 + _Time.y), 10, cos(i.uv.y * 50 + _Time.y)));
                float3 halfDir = normalize(i.lightDir + viewDir);
                float spec = pow(saturate(dot(waveNormal, halfDir)), _Gloss);
                final.rgb += spec * _SpecularColor.rgb * _LightColor0.rgb;

                return final;
            }
            ENDCG
        }
    }
    FallBack Off
}