// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "FX/Scroll_Layer_Blend" 
{
	Properties {
		_Color ("Tint Color", Color) = (1,1,1,1)
		_MainTex ("Main Texture", 2D) = "white" {}
		_FXTex ("FX Texture", 2D) = "white" {}
		_ScrollX ("Main Speed X", float) = 1.0
		_ScrollY ("Main Speed Y", float) = 0.0
		_Scroll2X ("FX Speed X", float) = 1.0
		_Scroll2Y ("FX Speed Y", float) = 0.0
		_Power ("FX Power", float) = 1.0
		_Brightness ("Brightness", float) = 1.0
	}
	SubShader {
		Tags { "Queue"="Transparent+10" "IgnoreProjector"="True" "RenderType"="Transparent" }
		LOD 100
		
		Pass {
			Cull Off
			Blend SrcAlpha OneMinusSrcAlpha
			Zwrite Off
		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			sampler2D _MainTex;
			sampler2D _FXTex;
			
			float4 _MainTex_ST;
			float4 _FXTex_ST;
			
			float _ScrollX;
			float _ScrollY;
			float _Scroll2X;
			float _Scroll2Y;
			
			fixed4 _Color;
			fixed _Brightness;
			float _Power;

			struct appdata {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				fixed4 color : COLOR;
			};

			struct v2f {
				float4 pos : SV_POSITION;
				float4 uv : TEXCOORD0;
				fixed4 color : COLOR0;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos (v.vertex);		
				o.uv.xy = TRANSFORM_TEX(v.texcoord.xy, _MainTex) + frac(float2(_ScrollX, _ScrollY) * _Time.x);
				o.uv.zw = TRANSFORM_TEX(v.texcoord.xy, _FXTex) + frac(float2(_Scroll2X, _Scroll2Y) * _Time.x);
				
				o.color = v.color * _Color;
				return o;
			}

			fixed4 frag (v2f i) : COLOR
			{
				fixed4 texcol = tex2D(_MainTex, i.uv.xy);
				half2 fxuv = texcol.r * _Power;
				fixed4 fx = tex2D(_FXTex, i.uv.zw + fxuv);
				fixed4 ret = texcol * fx * i.color * _Brightness;
				return ret;
			}

			ENDCG
		}
	} 
	FallBack Off
}
