Shader "Unlit/FontShader"{
	Properties{
		_MainTex("Font Texture", 2D) = "white" {}
		_Color("Text Color", Color) = (1,1,1,1)
		_Scale("Scale", Float) = 1

		_Smoothness("Smoothness", Float) = 1
		_Threshold("Threshold", Range(0, 1)) = 0.5
		_LowAlpha("Low Alpha", Range(0, 1)) = 0
		_HighAlpha("High Alpha", Range(0, 1)) = 1

		_OutlineWidth("Outline Width", Float) = 0.1
		_OutlineColor("Outline Color", Color) = (1, 1, 1, 1)
	}

	SubShader{

		Tags {
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
		}
		Lighting Off Cull Off ZTest Always ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile _ UNITY_SINGLE_PASS_STEREO STEREO_INSTANCING_ON STEREO_MULTIVIEW_ON
			#include "UnityCG.cginc"

			#define vec4 float4
			#define vec2 float2
			#define fract frac
			#define mix lerp

			struct appdata_t {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
				UNITY_VERTEX_OUTPUT_STEREO
			};

			sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			uniform fixed4 _Color;

			v2f vert(appdata_t v)
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.color = v.color * _Color;
				o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
				return o;
			}

			/*Perlin Noise*/
			vec4 mod289(vec4 x)
			{
				return x - floor(x * (1.0 / 289.0)) * 289.0;
			}

			vec4 mod(vec4 x, vec4 y)
			{
				return x - y * floor(x / y);
			}

			vec4 permute(vec4 x)
			{
				return mod289(((x*34.0) + 1.0)*x);
			}

			vec4 taylorInvSqrt(vec4 r)
			{
				return 1.79284291400159 - 0.85373472095314 * r;
			}

			vec2 fade(vec2 t) {
				return t * t*t*(t*(t*6.0 - 15.0) + 10.0);
			}

			// Classic Perlin noise
			float cnoise(vec2 P)
			{
				vec4 Pi = floor(P.xyxy) + vec4(0.0, 0.0, 1.0, 1.0);
				vec4 Pf = fract(P.xyxy) - vec4(0.0, 0.0, 1.0, 1.0);
				Pi = mod289(Pi); // To avoid truncation effects in permutation
				vec4 ix = Pi.xzxz;
				vec4 iy = Pi.yyww;
				vec4 fx = Pf.xzxz;
				vec4 fy = Pf.yyww;

				vec4 i = permute(permute(ix) + iy);

				vec4 gx = fract(i * (1.0 / 41.0)) * 2.0 - 1.0;
				vec4 gy = abs(gx) - 0.5;
				vec4 tx = floor(gx + 0.5);
				gx = gx - tx;

				vec2 g00 = vec2(gx.x, gy.x);
				vec2 g10 = vec2(gx.y, gy.y);
				vec2 g01 = vec2(gx.z, gy.z);
				vec2 g11 = vec2(gx.w, gy.w);

				vec4 norm = taylorInvSqrt(vec4(dot(g00, g00), dot(g01, g01), dot(g10, g10), dot(g11, g11)));
				g00 *= norm.x;
				g01 *= norm.y;
				g10 *= norm.z;
				g11 *= norm.w;

				float n00 = dot(g00, vec2(fx.x, fy.x));
				float n10 = dot(g10, vec2(fx.y, fy.y));
				float n01 = dot(g01, vec2(fx.z, fy.z));
				float n11 = dot(g11, vec2(fx.w, fy.w));

				vec2 fade_xy = fade(Pf.xy);
				vec2 n_x = mix(vec2(n00, n01), vec2(n10, n11), fade_xy.x);
				float n_xy = mix(n_x.x, n_x.y, fade_xy.y);
				return 2.3 * n_xy;
			}

			// Classic Perlin noise, periodic variant
			float pnoise(vec2 P, vec2 rep)
			{
				vec4 Pi = floor(P.xyxy) + vec4(0.0, 0.0, 1.0, 1.0);
				vec4 Pf = fract(P.xyxy) - vec4(0.0, 0.0, 1.0, 1.0);
				Pi = mod(Pi, rep.xyxy); // To create noise with explicit period
				Pi = mod289(Pi);        // To avoid truncation effects in permutation
				vec4 ix = Pi.xzxz;
				vec4 iy = Pi.yyww;
				vec4 fx = Pf.xzxz;
				vec4 fy = Pf.yyww;

				vec4 i = permute(permute(ix) + iy);

				vec4 gx = fract(i * (1.0 / 41.0)) * 2.0 - 1.0;
				vec4 gy = abs(gx) - 0.5;
				vec4 tx = floor(gx + 0.5);
				gx = gx - tx;

				vec2 g00 = vec2(gx.x, gy.x);
				vec2 g10 = vec2(gx.y, gy.y);
				vec2 g01 = vec2(gx.z, gy.z);
				vec2 g11 = vec2(gx.w, gy.w);

				vec4 norm = taylorInvSqrt(vec4(dot(g00, g00), dot(g01, g01), dot(g10, g10), dot(g11, g11)));
				g00 *= norm.x;
				g01 *= norm.y;
				g10 *= norm.z;
				g11 *= norm.w;

				float n00 = dot(g00, vec2(fx.x, fy.x));
				float n10 = dot(g10, vec2(fx.y, fy.y));
				float n01 = dot(g01, vec2(fx.z, fy.z));
				float n11 = dot(g11, vec2(fx.w, fy.w));

				vec2 fade_xy = fade(Pf.xy);
				vec2 n_x = mix(vec2(n00, n01), vec2(n10, n11), fade_xy.x);
				float n_xy = mix(n_x.x, n_x.y, fade_xy.y);
				return 2.3 * n_xy;
			}

			float _Scale;
			float _Threshold;
			float _Smoothness;
			float _LowAlpha;
			float _HighAlpha;
			float _OutlineWidth;
			vec4 _OutlineColor;

			#define E 2.71828182846

			float sigmoid(float x, float pos, float spread) {
				return 1 / (1 + pow(E, -spread * (x - pos)));
			}

			float outline(vec2 uv) {
				float left = tex2D(_MainTex, uv + vec2(-_OutlineWidth, 0)).a;
				float right = tex2D(_MainTex, uv + vec2(_OutlineWidth, 0)).a;
				float up = tex2D(_MainTex, uv + vec2(0, -_OutlineWidth)).a;
				float down = tex2D(_MainTex, uv + vec2(0, _OutlineWidth)).a;
				float me = tex2D(_MainTex, uv).a;

				return ((left > .5 || right > .5 || up > .5 || down > .5) && me < 0.5) ? 1 : 0;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col = i.color;
				col.a *= tex2D(_MainTex, i.texcoord).a;

				vec2 uv = i.texcoord * 2 - 1;
				uv.x *= _ScreenParams.y / _ScreenParams.x;

				float p1 = pnoise(i.texcoord*_Scale + _Time.yy, vec2(0, 0));
				float p2 = pnoise(i.texcoord*_Scale - _Time.yy, vec2(0, 0));
				float perlin = p1 * p2;

				col.a *= lerp(_LowAlpha, _HighAlpha, sigmoid(perlin, _Threshold, _Smoothness));

				if (outline(i.texcoord) > 0) {
					col = _OutlineColor;
				}

				return col;
			}
			ENDCG
		}
	}
}
