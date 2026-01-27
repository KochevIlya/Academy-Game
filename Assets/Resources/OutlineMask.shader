Shader "Custom/OutlineMask" {
    SubShader {
        Tags { "Queue"="Transparent+100" "RenderType"="Transparent" }
        Pass {
            ZTest Always
            ZWrite Off
            ColorMask 0
            Stencil {
                Ref 1
                Pass Replace
            }
        }
    }
}