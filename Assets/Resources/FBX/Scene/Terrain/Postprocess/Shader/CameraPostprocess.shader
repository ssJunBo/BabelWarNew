Shader "Unlit/CameraPostprocess"
{
	Properties
	{
		[HideInInspector] _MainTex("Texture", 2D) = "white" {}
		_Overlap("Overlap", 2D) = "black" {}
		_OverlapDark("OverlapDark", 2D) = "black" {}
		_Param("Param", float) = 1
		_ParamDark("Param Dark", float) = 1
		[Toggle(_LINEAR_LIGHT_ON)] _LINEAR_LIGHT_ON("Linear Light On", Float) = 0
		[Toggle(_ADDITIVE_ON)] _ADDITIVE_ON("单纯叠加", Float) = 0
		[Toggle(_BRIGHTER_ON)] _BRIGHTER_ON("叠加", Float) = 0
		[Toggle(_OVERLAP_ON)] _OVERLAP_ON("Overlap On", Float) = 0
	}
		SubShader
		{
			Cull Off ZWrite On ZTest Always

			Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma shader_feature_local _ADDITIVE_ON
				#pragma shader_feature_local _LINEAR_LIGHT_ON
				#pragma shader_feature_local _BRIGHTER_ON
				#pragma shader_feature_local _OVERLAP_ON

				#include "UnityCG.cginc"

				struct appdata
				{
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
				};

				struct v2f
				{
					float2 uv : TEXCOORD0;
					float4 vertex : SV_POSITION;
				};

				sampler2D _MainTex, _Overlap, _OverlapDark;
				float4 _MainTex_ST;

				float _Param, _ParamDark;

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					fixed4 col = tex2D(_MainTex, i.uv);

					fixed4 overlap_col = tex2D(_Overlap, i.uv);
					fixed4 overlap_dark_col = tex2D(_OverlapDark, i.uv);

					#ifdef _OVERLAP_ON
						col.rgb = lerp(col.rgb, col.rgb * overlap_dark_col.rgb, overlap_dark_col.a * _ParamDark);
					#endif

					#ifdef _ADDITIVE_ON
						col.rgb += overlap_col.rgb * overlap_col.a * _Param;
						col.rgb = saturate(col.rgb);
					#endif

					#ifdef _LINEAR_LIGHT_ON
						col.rgb = lerp(col.rgb, col.rgb + (overlap_col.rgb * 2 - 1) * _Param, overlap_col.a);
					#endif

					#ifdef _BRIGHTER_ON
						fixed3 tex1 = overlap_dark_col.rgb * overlap_dark_col.a;
						fixed3 comp = lerp(col.rgb * tex1 * 2, 1 - (1 - col.rgb) * (1 - tex1) * 2, step(0.5, tex1));
						col.rgb = lerp(col.rgb, comp, comp.r);
					#endif


					return col;
				}
				ENDCG
			}
		}
}
