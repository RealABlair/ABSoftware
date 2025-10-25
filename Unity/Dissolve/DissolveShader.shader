Shader "Custom/DissolveShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex("Texture", 2D) = "white" {}
        _DissolveThreshold("Dissolve Threshold", Range(0,1)) = 0.5
        [HDR]_EdgeColor("Edge Color", Color) = (1,1,1,1)
        _EdgeWidth("Edge Width", Range(0,0.2)) = 0.05
        _NoiseScale("Noise Scale", Float) = 5.0
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" "DisableBatching" = "True" "LightMode" = "ShadowCaster" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;
        float4 _Color;
        float _DissolveThreshold;
        float4 _EdgeColor;
        float _EdgeWidth;
        float _NoiseScale;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        float rand(float2 n)
        {
            return frac(sin(dot(n, float2(12.9898, 78.233))) * 43758.5453);
        }

        float fade(float t)
        {
            return t * t * t * (t * (t * 6 - 15) + 10);
        }

        float perlinNoise(float2 p)
        {
            float2 i = floor(p);
            float2 f = frac(p);

            float a = rand(i);
            float b = rand(i + float2(1.0, 0.0));
            float c = rand(i + float2(0.0, 1.0));
            float d = rand(i + float2(1.0, 1.0));

            float u = fade(f.x);
            float v = fade(f.y);

            float x1 = lerp(a, b, u);
            float x2 = lerp(c, d, u);
            return lerp(x1, x2, v);
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;

            float noiseValue = perlinNoise(IN.uv_MainTex * _NoiseScale);
            noiseValue = noiseValue * 0.5 + 0.5;
            float maskValue = noiseValue;

            float edgeLower = _DissolveThreshold - _EdgeWidth;
            float edgeUpper = _DissolveThreshold + _EdgeWidth;

            float alpha = smoothstep(edgeLower, edgeUpper, maskValue);

            /*if (maskValue < edgeLower)
            {
                discard;
            }
            else if (maskValue > edgeUpper)
            {
                col.a *= alpha;
            }
            else
            {
                col.rgb = _EdgeColor.rgb;
                col.a = alpha;
            }*/

            if (noiseValue < _DissolveThreshold)
                clip(-1);
            else if (noiseValue < _DissolveThreshold + _EdgeWidth)
                o.Albedo = _EdgeColor;

            o.Alpha = alpha;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
