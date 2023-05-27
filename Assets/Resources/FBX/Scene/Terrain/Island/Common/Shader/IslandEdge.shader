// Shader created with Shader Forge v1.38 
// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge

Shader "WWF/Edge" {
    Properties {
        _dis ("dis", 2D) = "white" {}
        _mask ("mask", 2D) = "white" {}
        _Color ("Color", Color) = (0.5,0.5,0.5,1)
        _Niuqu ("Niuqu", 2D) = "white" {}
        _Niuqu_U_copy ("Niuqu_U_copy", Float ) = -0.3
        _Niuqu_V_copy ("Niuqu_V_copy", Float ) = 0
        _Niuqu_U ("Niuqu_U", Float ) = 0
        _Niuqu_V ("Niuqu_V", Float ) = 0
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        LOD 100
        Pass {
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #pragma target 3.0

            uniform float4 _Color;
            uniform sampler2D _dis; uniform float4 _dis_ST;
            uniform sampler2D _mask; uniform float4 _mask_ST;
            uniform sampler2D _Niuqu; uniform float4 _Niuqu_ST;
            uniform float _Niuqu_U;
            uniform float _Niuqu_V;
            uniform float _Niuqu_U_copy;
            uniform float _Niuqu_V_copy;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = UnityObjectToClipPos( v.vertex );
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                float4 node_2309 = _Time;
                float2 node_4576 = (i.uv0+(node_2309.g*float2(_Niuqu_U_copy,_Niuqu_V_copy)));
                float4 _dis_var = tex2D(_dis,TRANSFORM_TEX(node_4576, _dis));
                float4 _mask_var = tex2D(_mask,TRANSFORM_TEX(i.uv0, _mask));
                clip(step((1.0 - dot(_dis_var.rgb,float3(0.3,0.59,0.11))),(1.0 - dot(_mask_var.rgb,float3(0.3,0.59,0.11)))) - 0.5);
////// Lighting:
////// Emissive:
                float4 node_5935 = _Time;
                float2 node_7753 = (i.uv0+(node_5935.g*float2(_Niuqu_U,_Niuqu_V)));
                float4 _Niuqu_var = tex2D(_Niuqu,TRANSFORM_TEX(node_7753, _Niuqu));
                float3 emissive = (_Niuqu_var.rgb*_Color.rgb);
                float3 finalColor = emissive;
                fixed4 finalRGBA = fixed4(finalColor,_Color.a);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
