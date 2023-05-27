Shader "Unlit/OverlapShadow_stencil"
{
    Properties
    {
        _Color("Color", Color) = (0, 0, 0, 0.5)
        _MainTex("Texture", 2D) = "white" {}
    }
    
    SubShader
    {    
        Tags {"Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True"}
        Pass
        {
            ZWrite Off
            // Blend DstAlpha OneMinusDstAlpha
            Blend SrcAlpha OneMinusSrcAlpha
            // BlendOp Min
            Fog { Mode off }

            Stencil {
                Ref 0
                Comp Equal
                Pass IncrSat 
                Fail IncrSat 
            }
            
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #pragma fragmentoption ARB_precision_hint_fastest
            #include "UnityCG.cginc"    
            
            uniform float4 _Color;
            uniform sampler2D _MainTex;
            
            fixed4 frag (v2f_img i) : SV_Target
            {  
                return _Color * tex2D(_MainTex, i.uv);
            }
            ENDCG
        }
    }
}