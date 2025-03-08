Shader "Unlit/OutlineMaskShader"
{
    Properties
    {
        [Enum(UnityEngine.Rendering.CompareFunction)] _ZTest("ZTest", Float) = 0
    }
    SubShader
    {
        Tags { "Queue" = "Transparent+100" "RenderType" = "Transparent" }
        LOD 100

        Pass
        {
            ZTest[_ZTest]
            Cull Off
            ColorMask 0

            Stencil {
                Ref 1
                Pass Replace
            }
        }
    }
}
