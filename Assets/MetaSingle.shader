Shader "Unlit/MetaSingle"{

    Properties{
        _Intensity("Intensity", Float) = 0.1
    }

    SubShader {
        Cull Off
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
       // Blend SrcAlpha OneMinusSrcAlpha, One One//SrcAlpha OneMinusSrcAlpha
        
        Blend SrcAlpha One

        //pass 1
        Pass{
            CGPROGRAM

            #pragma vertex vertexFunc
            #pragma fragment fragmentFunc
            #include "UnityCG.cginc"

            sampler2D _MainTex;

            struct v2f{
                float4 pos: SV_POSITION;
                half2 uv: TEXCOORD0;
                float4 color: COLOR;
            };

            v2f vertexFunc(appdata_full v){
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                o.color = v.color;
                return o;
            }

            float _Intensity;

            fixed4 fragmentFunc(v2f i) : COLOR{
                half4 c = i.color;
                float dist = length(i.uv - half2(.5, .5))*2;
                c.a = clamp((1 / dist) * _Intensity, 0, 1);

                return c;
            }

            ENDCG
        }
    }
}
