Shader "Sprites/Shadow"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
		_ShadowColor ("Shadow", Color) = (0,0,0,1)
		_ShadowScaler ("ShadowScaler", Vector) = (-1.0,-1.0,0,0)
		_ShadowOffset ("ShadowOffset", Vector) = (0,-0.1,0,0)
		_Shear ("Shear", Vector) = (1,0.0, 0.0, 0.0)
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		// draw shadow
		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile _ PIXELSNAP_ON
			#include "UnityCG.cginc"
			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				float2 texcoord  : TEXCOORD0;
			};
			
			fixed4 _ShadowColor;
			fixed4 _ShadowOffset;
			fixed4 _ShadowScaler;
			fixed4 _Shear;

			v2f vert(appdata_t IN)
			{
				v2f OUT;

				IN.vertex.x = IN.vertex.x - IN.vertex.y * tan(_Shear.x);
				IN.vertex.y = IN.vertex.y - IN.vertex.x * tan(_Shear.y);
				OUT.vertex = UnityObjectToClipPos(IN.vertex * _ShadowScaler + _ShadowOffset);

				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color *_ShadowColor;
				#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap (OUT.vertex);
				#endif

				return OUT;
			}

			sampler2D _MainTex;
			sampler2D _AlphaTex;
			float _AlphaSplitEnabled;

			fixed4 SampleSpriteTexture (float2 uv)
			{
				fixed4 color = tex2D (_MainTex, uv);
				color.rgb = _ShadowColor.rgb;

				#if UNITY_TEXTURE_ALPHASPLIT_ALLOWED
				if (_AlphaSplitEnabled)
					color.a = tex2D (_AlphaTex, uv).r;
				#endif //UNITY_TEXTURE_ALPHASPLIT_ALLOWED

				return color;
			}

			fixed4 frag(v2f IN) : SV_Target
			{
				// float2x2 scaleMat = {
				// 	_ShadowScaler.x, 0.0,
				// 	0.0, _ShadowScaler.y 
				// };
				
				//fixed4 c = (SampleSpriteTexture (scaleMat * IN.texcoord + _ShadowOffset) * IN.color);
				
				fixed4 c = SampleSpriteTexture(IN.texcoord) * IN.color;
				c.rgb *= c.a;
				return c;
			}
		ENDCG
		}
	}
}