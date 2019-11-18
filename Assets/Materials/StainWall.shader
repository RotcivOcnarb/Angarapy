Shader "Unlit/StainWall"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Threshold("Threshold", Float) = 0.5
        _Deviation("Deviation", Range(0, 10)) = 0.15
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
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

            float _Threshold;
            float _Deviation;

            float4 _Metaballs[1024];
            float4 _MetaColors[1024];
            float _MetaballsSize;

            float2 viewport;
            float2 cameraPosition;
            float cameraZoom;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            float lengthFunction(float len, float size){
                return (1/(size * 2.5)) * pow(2.71, -.5*(len/size)*(len/size)) * 0.05;
            }

            fixed4 frag(v2f i) : COLOR{
                // sample the texture
                fixed4 mask = tex2D(_MainTex, i.uv);

                if(mask.a == 0) return float4(0.0, 0.0, 0.0, 0.0);

                float4 color = float4(0.0, 0.0, 0.0, 0.0);

                if(_MetaballsSize == 0)
                    return mask;

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

                color.a *= mask.a;

                color.a *= paintAlpha; //transparencia

                float4 fn = float4(0, 0, 0, 0);
                fn.rgb = color.rgb * color.a + mask.rgb * (1 - color.a); //ALPHA BLENDING
                fn.a = mask.a;

                return fn;
            }
            ENDCG
        }
    }
}
