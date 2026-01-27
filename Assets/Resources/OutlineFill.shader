Shader "Custom/OutlineFill" {
    Properties {
        _OutlineColor("Outline Color", Color) = (0, 1, 1, 1)
        _OutlineWidth("Outline Width", Range(0, 10)) = 2
    }
    SubShader {
        Tags { "Queue"="Transparent+110" "RenderType"="Transparent" }
        Pass {
            ZTest Always
            ZWrite Off
            Stencil {
                Ref 1
                Comp NotEqual
            }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            struct appdata { float4 vertex : POSITION; float3 normal : NORMAL; };
            struct v2f { float4 pos : SV_POSITION; };
            float _OutlineWidth;
            fixed4 _OutlineColor;
            v2f vert (appdata v) {
                v2f o;
                float3 norm = normalize(v.normal);
                v.vertex.xyz += norm * _OutlineWidth * 0.01;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }
            fixed4 frag (v2f i) : SV_Target { return _OutlineColor; }
            ENDCG
        }
    }
}