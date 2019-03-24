Shader "Aurora" {
    Properties {
        _MainTex ("Particle Texture", 2D) = "white" {}
    }

    Category {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Back Lighting Off ZWrite Off 

        SubShader {
            
            LOD 100
            
            Pass {
                
                //Stencil {
                //    Ref 0
                //    Comp Equal
                //    Pass IncrWrap 
                //    Fail IncrWrap 
                //}
                
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                #include "UnityCG.cginc"
                
                sampler2D _MainTex;
                float4 _MainTex_ST;
                
                struct appdata
                {
                    float4 vertex : POSITION;
                };

                struct v2f
                {
                    float4 vertex : SV_POSITION;
                    float2 uv : TEXCOORD0;
                    half4 color : COLOR0;
                };

                v2f vert (appdata_full v)
                {
                    v2f o;
                    o.color = v.color;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                    return o;
                }

                fixed4 frag (v2f i) : SV_Target
                {
                    fixed4 texcol = tex2D(_MainTex, i.uv) * i.color;
                    return texcol;
                }
                
                ENDCG
            }
        }
    }
}