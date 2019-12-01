Shader "Unlit/StainShader2"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        
        _WallTex("Wall Tex", 2D) = "white" {}
        _StainTex("Stain Tex", 2D) = "white" {}
        _Threshold("Threshold", Float) = .5

        _HUE("Hue", Int) = 1
        _Color("Color", Color) = (1, 1, 1, 1)

        _GlowColor("Glow Color", Color) = (1, 1, 1, 1)
        _GlowIntensity("Glow Intensity", Float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            sampler2D _WallTex;
            sampler2D _StainTex;

            float _Threshold;
            int _HUE;
            float4 _Color;

            float4 _GlowColor;
            float _GlowIntensity;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            float3 hsv2rgb(float3 c)
            {
                float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
                float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
                return c.z * lerp(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
            }


            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 wall = tex2D(_WallTex, i.uv);
                fixed4 stain = tex2D(_StainTex, i.uv);
                float4 col = wall;
                
                if(stain.a > _Threshold)
                    stain.a = 1.0;
                else
                    stain.a = 0;

                if(_HUE == 0)
                    stain.rgb = _Color;
                else
                    stain.rgb = hsv2rgb(float3(i.uv.x + _Time.y, 1, 1));

                float3 alphablending = stain.rgb  * stain.a + wall.rgb * (1 - stain.a); //Alpha blending
                float3 additive = wall.rgb + stain.rgb * stain.a * wall.rgb;
                col.rgb = lerp(alphablending, additive, .5);
                col.a = wall.a;
                
                //Glow
                if(wall.a < 0.1){
                    float sml = 1;
                    float4 ccc = float4(0, 0, 0, 0);
                    for(float id = 0.05; id >= 0; id -= 0.001){
                        fixed4 wallUp = tex2D(_WallTex, i.uv + fixed2(0, id));
                        fixed4 wallDown = tex2D(_WallTex, i.uv + fixed2(0, -id));
                        fixed4 wallLeft = tex2D(_WallTex, i.uv + fixed2(-id, 0));
                        fixed4 wallRight = tex2D(_WallTex, i.uv + fixed2(id, 0));


                        
                        if(wallUp.a > 0.9){ sml = id; ccc = wallUp;}
                        if(wallDown.a > 0.9){ sml = id; ccc = wallDown;}
                        if(wallLeft.a > 0.9){ sml = id; ccc = wallLeft;}
                        if(wallRight.a > 0.9){ sml = id; ccc = wallRight;}
                        
                    }

                    if(sml < 1){
                        col.rgb = _GlowColor.rgb;
                        col.a = 1/(sml*1000) * _GlowIntensity;
                    }
                }

                return col;
            }
            ENDCG
        }
    }
}
