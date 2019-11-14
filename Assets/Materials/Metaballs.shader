Shader "Custom/Metaballs"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Threshold("Threshold", Float) = 0.5
    }
    SubShader
    {
        //Cull Off
        //Tags { "Queue"="Transparent" }
        //ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass{
            CGPROGRAM
            #pragma target 5.0
    
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

            float4 _Metaballs[256];
            float4 _MetaColors[256];
            float _MetaballsSize;

            float2 viewport;
            float2 cameraPosition;
            float cameraZoom;

            fixed4 fragmentFunc(v2f i) : COLOR{

                float4 color = float4(0.0, 0.0, 0.0, 0.0);

                float2 uv = i.uv * viewport;

                float influence = 0;
                float cont = 0;

                /* Esse cara funciona esquisito mas funciona
                for(int i = 0; i < _MetaballsSize; i ++){
                        float3 transformed = float3((_Metaballs[i].xy - cameraPosition) / cameraZoom + viewport/2.0, _Metaballs[i].z / cameraZoom);
                        float len = length(uv - transformed.xy) / viewport.x;

                        float intensity = clamp ( (transformed.z / pow(len, 1.0)) / 100.0, 0, 1 );
                        float coloraddup = -len / transformed.z + 1.0;

                        if(transformed.z > 0.0){
                            color += float4(_MetaColors[i].rgb * intensity, intensity);
                            influence += intensity;
                            cont += 1.0;
                        }
                }

                color.rgb = color.rgb / influence;
                color.a /= influence;
                if(influence < _Threshold){
                   color = float4(0.0, 0.0, 0.0, 0.0);
                }

                */

                for(int i = 0; i < _MetaballsSize; i ++){
                    float3 transformed = float3((_Metaballs[i].xy - cameraPosition) / cameraZoom + viewport/2.0, _Metaballs[i].z / cameraZoom);
                
                    float len = length(uv - transformed.xy) / viewport.x;
                    
                    // DESCOBRIR UMA FUNÇÃO DE FALLOFF MELHOR QUE ESSA (já tentai gaussiana, e umas parada maluca ai)
                    //rotciv do futuro aqui: essa função de falloff é a melhor que tem mesmo kkkk
                    
                    float addup = (transformed.z / pow(len, 2.0)) / 100.0; //essa é a função de falloff q define o alpha
                    float coloraddup = -len / transformed.z + 1.0;
                    
                    if(transformed.z > 0.0 && addup > 10){
                        color += float4(_MetaColors[i].rgb * coloraddup, 1.0);

                        influence += addup;
                        cont += 1.0;
                    }
                }
                
                color /= cont;
                color.a = 1.0;
                
                if(influence < _Threshold){
                    color = float4(0.0, 0.0, 0.0, 0.0);
                }



                return color;
            }

            ENDCG
        }
    }
}
