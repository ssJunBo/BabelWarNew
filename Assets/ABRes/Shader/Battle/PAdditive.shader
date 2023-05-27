/* 纯色向纹理色渐变的 shader， 主要为了搭配FxPro Bloom*/
Shader "Shaders/Effect/PAdditive"
{
    Properties
	{
        _TintLerp ("TintLerp", Range(0.15, 1)) = 0
        _TintColor ("MainColor", Color) = (1,0,0,1)
        _MainTex ("MainTex", 2D) = "white" {}
        _Strength ("Strength", Float ) = 1.0
    }
    SubShader
	{
        Tags 
		{
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass 
		{
            Blend SrcAlpha One
            ZWrite Off
            Cull off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            uniform float4 _TintColor;
            uniform float _TintLerp;
            uniform sampler2D _MainTex; 
			uniform float4 _MainTex_ST;
			uniform float _Strength;

            struct VertexInput 
			{
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
				fixed4 vertexColor : COLOR;
            };

            struct VertexOutput
			{
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
				fixed4 vertexColor : COLOR;
            };

            VertexOutput vert (VertexInput v)
			{
                VertexOutput o;
				o.uv0 = TRANSFORM_TEX(v.texcoord0, _MainTex);
                o.pos = UnityObjectToClipPos( v.vertex );
				o.vertexColor = v.vertexColor;
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }

            float4 frag(VertexOutput i) : COLOR 
			{
				float4 _MainTex_var = tex2D(_MainTex, i.uv0)*_Strength;
				float3 emissive = lerp((_TintColor.rgb*i.vertexColor.rgb*0.07999998 + 0.92), (_TintColor.rgb*i.vertexColor.rgb*_MainTex_var.rgb), _TintLerp);
                float3 finalColor = emissive;
				fixed4 finalRGBA = fixed4(finalColor*finalColor*finalColor*finalColor, _TintColor.a*i.vertexColor.a*_MainTex_var.a);
                return finalRGBA;
            }
            ENDCG
        }
    }
}
