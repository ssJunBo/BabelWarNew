Shader "Shaders/Effect/ModelUVMoveAdditive"
{
Properties
	{
		_TintColor("Tint Color", Color) = (0.5, 0.5, 0.5, 1)
		_MainTex("Main Texture", 2D) = "white" {}
		_MaskTex("Mask Texture： R chanel replace Alpha", 2D) = "white" {}
		_XSpeed("XSpeed", Float) = 0
		_YSpeed("YSpeed", Float) = 0
	}
	SubShader
	{
		Tags
		{ 
			"Queue" = "Transparent" 
			"RenderType" = "Transparent" 
			"IgnoreProject" = "True"
		}
		Pass
		{
			Blend SrcAlpha One
			Cull Off
			ZWrite Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			sampler2D _MaskTex;
			float4 _MainTex_ST;
			float4 _MaskTex_ST;
			uniform float4 _TintColor;

			float _XSpeed;
			float _YSpeed;

			struct VertexInput
			{
				float4 vertex : POSITION;
				float2 texcoord0 : TEXCOORD0;
				float4 vertexColor : COLOR;		
			};

			struct VertexOutput
			{
				float4 pos : SV_POSITION;
				float2 uv0 : TEXCOORD0;
				float4 vertexColor : COLOR;
				float2 uv_mask : TEXCOORD1;
			};

			VertexOutput vert(VertexInput v)
			{
				VertexOutput o = (VertexOutput)0;
				o.uv0 = TRANSFORM_TEX(v.texcoord0, _MainTex) + float2(_Time.x * _XSpeed, _Time.x * _YSpeed);
				o.vertexColor = v.vertexColor;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv_mask = TRANSFORM_TEX(v.texcoord0, _MaskTex);
				return o;
			}

			float4 frag(VertexOutput i) : COLOR
			{
				float4 col = tex2D(_MainTex, i.uv0);
				//rgb
				col.rgb *= i.vertexColor.rgb*_TintColor.rgb*2.0;
				//a
				col.a *= i.vertexColor.a*_TintColor.a*tex2D(_MaskTex, i.uv_mask).r;
				return col;
			}
			ENDCG
		}
	}
}
