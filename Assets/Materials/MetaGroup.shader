Shader "Unlit/Outline"{

    Properties{
        _MainTex("Texture", 2D) = "white" {}
        _Color("Colour", Color) = (1, 1, 1, 1)
        _Threshold("Threshold", Float) = 0.5
    }

    SubShader {
        Cull Off
        //Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZTest Off

        Pass{
            CGPROGRAM

            #pragma vertex vertexFunc
            #pragma fragment fragmentFunc
            #include "UnityCG.cginc"

            sampler2D _MainTex;

            struct v2f{
                float4 pos: SV_POSITION;
                half2 uv: TEXCOORD0;
            };

            v2f vertexFunc(appdata_base v){
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                return o;
            }

            fixed4 _Color;
            float4 _MainTex_TexelSize;
            float _Threshold;

            fixed4 fragmentFunc(v2f i) : COLOR{
                half4 c = tex2D(_MainTex, i.uv);
                //c.rgb *= c.a;

                //if(c.a >= _Threshold) c.a = 1;
                //else c.a = 0;

                return c;
            }

            ENDCG
        }
    }
}
