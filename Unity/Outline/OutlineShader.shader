Shader "Unlit/OutlineShader"
{
    Properties
    {
        [Header(Outline)]
        [Enum(UnityEngine.Rendering.CompareFunction)] _ZTest("ZTest", Float) = 0
        _OutlineColor ("Outline Color", Color) = (1, 1, 1, 1)
        _OutlineWidth ("Outline Width", Float) = 1
    }
    SubShader
    {
        Tags { "Queue"="Transparent+110" "RenderType"="Transparent" "DisableBatching" = "True" }

        Pass
        {
            ZTest [_ZTest]
            Cull Front
            Blend SrcAlpha OneMinusSrcAlpha

            Stencil {
                Ref 1
                Comp NotEqual
            }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 position : SV_POSITION;
                fixed4 color : COLOR;
            };

            uniform fixed4 _OutlineColor;
            uniform float _OutlineWidth;

            v2f vert (appdata v)
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                float3 normal = v.normal;
                float3 viewPosition = UnityObjectToViewPos(v.vertex);

                o.position = UnityObjectToClipPos(v.vertex * _OutlineWidth);
                o.color = _OutlineColor;
                
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return i.color;
            }
            ENDCG
        }
    }
}
