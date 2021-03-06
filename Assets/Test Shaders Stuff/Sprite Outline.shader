Shader "Sprites/Sprite Outline"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
		_OutlineColor ("OutlineColor", Color) = (0,0,0,1)
		//_ShadowScaler ("ShadowScaler", Vector) = (-1.0,-1.0,0,0)
		_OutlineOffset ("OutlineOffset", Range(0.0, 1.0)) = 0.0 // Later rename to "OutlineThickness"
		//_Shear ("Shear", Vector) = (1,0.0, 0.0, 0.0)
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

		// draw outline
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
			
			//fixed4 _Color;
			//float4 _ShadowScaler;
			//float4 _Shear;
			fixed4 _OutlineColor;

			v2f vert(appdata_t IN)
			{
				v2f OUT;

				// old stuff for shadow
				// IN.vertex.x = IN.vertex.x - IN.vertex.y * tan(_Shear.x);
				// IN.vertex.y = IN.vertex.y - IN.vertex.x * tan(_Shear.y);
				// OUT.vertex = UnityObjectToClipPos(IN.vertex * _ShadowScaler + _OutlineOffset);

				// new stuff for outline
				OUT.vertex = UnityObjectToClipPos(IN.vertex);


				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * _OutlineColor;
				#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap (OUT.vertex);
				#endif

				return OUT;
			}

			sampler2D _MainTex;
			sampler2D _AlphaTex;
			float _AlphaSplitEnabled;

			float _OutlineOffset;

			fixed4 SampleSpriteTexture (float2 uv)
			{
				fixed4 color = tex2D (_MainTex, uv);
				color.rgb = _OutlineColor.rgb;

				#if UNITY_TEXTURE_ALPHASPLIT_ALLOWED
				if (_AlphaSplitEnabled)
					color.a = tex2D (_AlphaTex, uv).r;
				#endif //UNITY_TEXTURE_ALPHASPLIT_ALLOWED

				return color;
			}

			fixed4 frag(v2f IN) : SV_Target
			{
				fixed4 c = SampleSpriteTexture (IN.texcoord) * IN.color;

				// float2 uv_left = IN.texcoord + float2(-_OutlineOffset, 0.0f);
				// fixed2 uv_right = IN.texcoord + float2(_OutlineOffset, 0.0f);
				// fixed2 uv_upper = IN.texcoord + float2(0.0f, _OutlineOffset);
				// fixed2 uv_lower = IN.texcoord + float2(0.0f, -_OutlineOffset);

				// fixed4 c_left = SampleSpriteTexture  (uv_left) * IN.color;
				// fixed4 c_right = SampleSpriteTexture (uv_right) * IN.color;
				// fixed4 c_upper = SampleSpriteTexture (uv_upper) * IN.color;
				// fixed4 c_lower = SampleSpriteTexture (uv_lower) * IN.color;

				fixed4 mask_left = SampleSpriteTexture (IN.texcoord + float2(_OutlineOffset, 0.0)) * IN.color;
				fixed4 mask_right = SampleSpriteTexture (IN.texcoord + float2(-_OutlineOffset, 0.0)) * IN.color;
				fixed4 mask_upper = SampleSpriteTexture (IN.texcoord + float2(0.0, -_OutlineOffset)) * IN.color;
				fixed4 mask_lower = SampleSpriteTexture (IN.texcoord + float2(0.0, _OutlineOffset)) * IN.color;

				//fixed4 mask = mask_left + mask_right + mask_upper + mask_lower;
				c = mask_left + mask_right + mask_upper + mask_lower;
				//c = mask_left + mask_upper;

				c.rgb *= c.a;
				return c;
			}
		ENDCG
		}

		// draw real sprite
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
			
			fixed4 _Color;

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * _Color;
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

				#if UNITY_TEXTURE_ALPHASPLIT_ALLOWED
				if (_AlphaSplitEnabled)
					color.a = tex2D (_AlphaTex, uv).r;
				#endif //UNITY_TEXTURE_ALPHASPLIT_ALLOWED

				return color;
			}

			fixed4 frag(v2f IN) : SV_Target
			{
				fixed4 c = SampleSpriteTexture (IN.texcoord) * IN.color;
				c.rgb *= c.a;
				return c;
			}
		ENDCG
		}
	}
}


//{
//    Properties
//    {
//        _MainTex ("Texture", 2D) = "white" {}
//    }
//    SubShader
//    {
//        // No culling or depth
//        Cull Off 
//        ZWrite Off 
//        ZTest Always
//
//        Pass
//        {
//            CGPROGRAM
//            #pragma vertex vert
//            #pragma fragment frag
//
//            #include "UnityCG.cginc"
//
//            struct appdata
//            {
//                float4 vertex : POSITION;
//                float2 uv : TEXCOORD0;
//            };
//
//            struct v2f
//            {
//                float2 uv : TEXCOORD0;
//                float4 vertex : SV_POSITION;
//            };
//
//            v2f vert (appdata v)
//            {
//                v2f o;
//                o.vertex = UnityObjectToClipPos(v.vertex);
//                o.uv = v.uv;
//                return o;
//            }
//
//            sampler2D _MainTex;
//
//            fixed4 frag (v2f i) : SV_Target
//            {
//                fixed4 col = tex2D(_MainTex, i.uv);
//                // just invert the colors
//                //col.rgb = 1 - col.rgb;
//                return col;
//            }
//            ENDCG
//        }
//    }
//}
