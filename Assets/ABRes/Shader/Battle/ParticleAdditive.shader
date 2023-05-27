Shader "Shaders/Effect/ParticleAdditive" 
{
    Properties 
	{
        [HDR]_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,1)
        _MainTex ("Main Texture", 2D) = "white" {}
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
            Cull Off
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            uniform sampler2D _MainTex; 
			uniform float4 _MainTex_ST;
            uniform float4 _TintColor;

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
            };
            VertexOutput vert (VertexInput v) 
			{
                VertexOutput o = (VertexOutput)0;
				o.uv0 = TRANSFORM_TEX(v.texcoord0, _MainTex);
                o.vertexColor = v.vertexColor;
                o.pos = UnityObjectToClipPos( v.vertex );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR 
			{
                float4 _MainTex_var = tex2D(_MainTex,i.uv0);
                float3 finalColor = (_MainTex_var.rgb*i.vertexColor.rgb*_TintColor.rgb*2.0);
                fixed4 finalRGBA = fixed4(finalColor,(_MainTex_var.a*i.vertexColor.a*_TintColor.a));
                return finalRGBA;
            }
            ENDCG
        }
    }
}
