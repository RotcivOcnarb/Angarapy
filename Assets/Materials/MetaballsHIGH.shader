Shader "Custom/Metaballs"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Threshold("Threshold", Float) = 0.5
        _Deviation("Deviation", Range(0, 10)) = 0.15
    }
    SubShader
    {
        //Cull Off
        Tags { "Queue"="Transparent" }
        ZWrite Off
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
            float _Deviation;

            float4 _Metaballs[1024];
            float4 _MetaColors[1024];
            float _MetaballsSize;

            float2 viewport;
            float2 cameraPosition;
            float cameraZoom;
            
            float lengthFunction(float len, float size){
                return (1/(size * 2.5)) * pow(2.71, -.5*(len/size)*(len/size)) * 0.05;
            }

            fixed4 fragmentFunc(v2f i) : COLOR{
                float4 color = float4(0.0, 0.0, 0.0, 0.0);

                if(_MetaballsSize == 0)
                    return float4(0, 0, 0, 0);

                float weight = 0;
                float2 uv = i.uv * viewport;

                for(int i = 0; i < _MetaballsSize; i ++){
                    if(_Metaballs[i].z > 0){
                        float3 transformed = float3((_Metaballs[i].xy - cameraPosition) / cameraZoom + viewport/2.0, _Metaballs[i].z / cameraZoom);
                        float len = length(uv - transformed.xy) / viewport.x;
                        
                        float intensity = transformed.z * lengthFunction(len, _Deviation * transformed.z);
                        weight += intensity;

                        color += _MetaColors[i] * intensity;
                    }
                }

                float paintAlpha = 1;

                if(weight > 0){
                    color /= weight;
                    paintAlpha = color.a;
                }
                color.a = clamp(weight * 10, 0, 1);

                
                if(color.a > _Threshold)
                    color.a = 1.0;
                else
                    color.a = 0.0;

                color.a *= paintAlpha; //transparencia original da cor
                

                return color;
            }

            ENDCG
        }
    }
}
