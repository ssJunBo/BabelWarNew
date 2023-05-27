Shader "Shaders/Effect/effect_RR_a_double" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        [HDR]_TintColor ("Tint Color", Color) = (1,1,1,1)
        _UV_sudu ("UV_sudu", Vector) = (0,0,0,0)
        _RaodongTEX ("RaodongTEX", 2D) = "white" {}
        [MaterialToggle] _RD_main_sw ("RD_main_sw", Float ) = 0
        [MaterialToggle] _RD_rongjie_sw ("RD_rongjie_sw", Float ) = 0
        [MaterialToggle] _RD_mask_sw ("RD_mask_sw", Float ) = 0
        _Raodong_qiangdu ("Raodong_qiangdu", Range(0, 1)) = 0
        _RongjieTEX ("RongjieTEX", 2D) = "white" {}
        _Rongjie_td ("Rongjie_td", 2D) = "white" {}
        _Rongjie_qiangdu ("Rongjie_qiangdu", Range(0, 2)) = 0
        _Rongjie_bianyuan ("Rongjie_bianyuan", Range(1, 30)) = 30
        [MaterialToggle] _Rongjie_fanzhuang ("Rongjie_fanzhuang", Float ) = 0
        [MaterialToggle] _Rongjie_td_sw ("Rongjie_td_sw", Float ) = 0
        [MaterialToggle] _UV_rongjie_sw ("UV_rongjie_sw", Float ) = 0
        [MaterialToggle] _Rongjie_dh ("Rongjie_dh", Float ) = 1
        _Rongjie_cc ("Rongjie_cc", Vector) = (1,1,0,0)
        _MaskTEX ("MaskTEX", 2D) = "white" {}
        _Opacity ("Opacity", Range(0, 10)) = 1
        [MaterialToggle] _Mask_sw ("Mask_sw", Float ) = 0
        [MaterialToggle] _Rongjie_qh ("Rongjie_qh", Float ) = 0
       // [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha One
            Cull Off
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
         //   #pragma multi_compile_fwdbase
           // #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal d3d11_9x 
            #pragma target 3.0
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float _Raodong_qiangdu;
            uniform float4 _TintColor;
            uniform sampler2D _MaskTEX; uniform float4 _MaskTEX_ST;
            uniform float _Rongjie_qiangdu;
            uniform fixed _Rongjie_fanzhuang;
            uniform float _Opacity;
            uniform fixed _RD_rongjie_sw;
            uniform fixed _RD_main_sw;
            uniform sampler2D _RaodongTEX; uniform float4 _RaodongTEX_ST;
            uniform fixed _RD_mask_sw;
            uniform float4 _UV_sudu;
            uniform fixed _Rongjie_dh;
            uniform sampler2D _RongjieTEX; uniform float4 _RongjieTEX_ST;
            uniform float4 _Rongjie_cc;
            uniform sampler2D _Rongjie_td; uniform float4 _Rongjie_td_ST;
            uniform float _Rongjie_bianyuan;
            uniform fixed _Mask_sw;
            uniform fixed _Rongjie_td_sw;
            uniform fixed _UV_rongjie_sw;
            uniform fixed _Rongjie_qh;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 texcoord1 : TEXCOORD1;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 uv1 : TEXCOORD1;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.vertexColor = v.vertexColor;
                o.pos = UnityObjectToClipPos( v.vertex );
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                //float isFrontFace = ( facing >= 0 ? 1 : 0 );
                //float faceSign = ( facing >= 0 ? 1 : -1 );
////// Lighting:
////// Emissive:
                float2 _raodong_uv1 = (i.uv0+(_Time.g*_UV_sudu.b)*float2(1,0));
                float4 _niuqu_tex1 = tex2D(_RaodongTEX,TRANSFORM_TEX(_raodong_uv1, _RaodongTEX));
                float2 _raodong_uv2 = (i.uv0+(_Time.g*_UV_sudu.a)*float2(0,1));
                float4 _niuqu_tex2 = tex2D(_RaodongTEX,TRANSFORM_TEX(_raodong_uv2, _RaodongTEX));
                float _noise_uv = (_niuqu_tex1.r*_niuqu_tex2.g);
                float2 _offset_uv = ((float2(_noise_uv,_noise_uv)-0.5)*_Raodong_qiangdu*i.uv1.g);
                float2 _raodong_uv = (((i.uv0+(_UV_sudu.r*_Time.g)*float2(1,0))+(_UV_sudu.g*_Time.g)*float2(0,1))+_offset_uv);
                float2 _raodong_main_uv = lerp( i.uv0, _raodong_uv, _RD_main_sw );
                float4 _main_tex = tex2D(_MainTex,TRANSFORM_TEX(_raodong_main_uv, _MainTex));
                float3 finalColor = (_main_tex.rgb*_TintColor.rgb*i.vertexColor.rgb*4.0);
                float2 _main_offset_uv = (_offset_uv+i.uv0);
                float2 _mask_uv = lerp( i.uv0, _main_offset_uv, _RD_mask_sw );
                float4 _mask_tex = tex2D(_MaskTEX,TRANSFORM_TEX(_mask_uv, _MaskTEX));
                float2 _rongjie_uv1 = float2(((_Rongjie_cc.b*_Time.g)+(i.uv0.r*_Rongjie_cc.r)),((_Rongjie_cc.a*_Time.g)+(i.uv0.g*_Rongjie_cc.g)));
                float4 _rongjie_tex = tex2D(_RongjieTEX,TRANSFORM_TEX(_rongjie_uv1, _RongjieTEX));
                float2 _rongjie_uv2 = lerp( i.uv0, lerp( _main_offset_uv, _raodong_uv, _UV_rongjie_sw ), _RD_rongjie_sw );
                float4 _xiaosan_texture = tex2D(_RongjieTEX,TRANSFORM_TEX(_rongjie_uv2, _RongjieTEX));
                float4 _rongjie_uv = tex2D(_Rongjie_td,TRANSFORM_TEX(i.uv0, _Rongjie_td));
                float _intensity = lerp(1.0,(lerp( 1.0, _rongjie_tex.r, _Rongjie_dh )*_xiaosan_texture.r),lerp( _rongjie_uv.r, _rongjie_uv.a, _Rongjie_td_sw ));
                float rongjie = saturate(((lerp( _intensity, (1.0 - _intensity), _Rongjie_fanzhuang )-(_Rongjie_qiangdu*lerp( i.uv1.r, 1.0, _Rongjie_qh )))*_Rongjie_bianyuan));
                return fixed4(finalColor,(_main_tex.a*lerp( _mask_tex.r, _mask_tex.a, _Mask_sw )*_Opacity*rongjie*(_TintColor.a*i.vertexColor.a)));
            }
            ENDCG
        }
    }
  //  CustomEditor "ShaderForgeMaterialInspector"
}
