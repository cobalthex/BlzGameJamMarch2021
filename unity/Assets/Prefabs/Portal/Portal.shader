// based on https://medium.com/@limdingwen_66715/multiple-recursive-portals-and-ai-in-unity-part-1-basic-portal-rendering-7c3d957f656c

Shader "Portal Viewthrough"
{
    Properties 
    {
        _Color ("Tint", Color) = (1, 1, 1, 1)
        _MainTex ("Texture", 2D) = "white" {}
        _IsMirror ("IsMirror", Int) = 0
    }

    SubShader
    {
        Tags
        { 
            "RenderType" = "Opaque" 
            "Queue"      = "Geometry"
        }

        Pass
        {
            CGPROGRAM

            #include "UnityCG.cginc"
        
            // #pragma surface surf Standard fullforwardshadows
            #pragma vertex VertexFn
            #pragma fragment FragmentFn

            sampler2D _MainTex;
            float4 _MainTex_ST;

            fixed4 _Color; // tint

            int _IsMirror = 0; // 1 or 0

            struct VertexOutput
            {
                float4 position       : SV_POSITION;
                float4 screenPosition : TEXCOORD0;
            };

            //the vertex shader
            VertexOutput VertexFn(float4 vertex : POSITION)
            {
                VertexOutput output;
                //convert the vertex positions from object space to clip space so they can be rendered
                output.position = UnityObjectToClipPos(vertex);
                output.screenPosition = ComputeScreenPos(output.position);
                return output;
            }

            //the fragment shader
            fixed4 FragmentFn(VertexOutput input) : SV_TARGET
            {
                float2 texcoord = input.screenPosition.xy / input.screenPosition.w;

                fixed4 col = tex2D(_MainTex, _IsMirror ? float2(1 - texcoord.x, texcoord.y) : texcoord);
                col *= _Color;
                
                return col;
            }

            ENDCG
        }
    }
}