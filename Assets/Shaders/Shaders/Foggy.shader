// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Foggy"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_FogTex("Fog Texture", 2D) = "white" {}
		_Motion("Effect", 2D) = "black"{}
		_DistTex("Distortion Texture", 2D) = "grey" {}
		_MotionSpeed("Motion Speed", float) = 0
		_EffectsLayer2Foreground("Blend Foreground", float) = 0
			//_InvertBlur("Invert", float) = 1
			//_Position("Blur Position", float3 xyz) 
			//_DisplaceTex("Displacement Texture", 2D) = "white" {}
			//_Magnitude("Magnitude", Range(0,0.1)) = 1
	}
		SubShader
		{
			Tags
		{
			"Queue" = "Transparent"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
		}
			Cull Off ZWrite Off ZTest Always

		Pass
	{
		//Blend SrcAlpha OneMinusSrcAlpha
		Blend DstColor OneMinusDstAlpha
		
		//Blend One One
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"
	struct appdata
	{
		float4 vertex : POSITION;
		float2 uv : TEXCOORD0;
	};

	struct v2f
	{
		float4 vertex : SV_POSITION;
		float2 uv : TEXCOORD0;

	};


	v2f vert(appdata v)
	{
		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.uv = v.uv;
		return o;
	}

	sampler2D _MainTex;
	sampler2D _FogTex;
	sampler2D _Motion;
	sampler2D _DistTex;
	float _EffectsLayer2Foreground;
	float _MotionSpeed;
	//float _InvertBlur;
	//sampler2D _DisplaceTex;
	//float _Magnitude;
	//float4 _MainTex_TexelSize;


	/*float4 box(sampler2D tex, float2 uv, float4 size)
	{

		float4 c = tex2D(tex, uv + float2(-size.x, size.y)) + tex2D(tex, uv + float2(0, size.y)) + tex2D(tex, uv + float2(size.x, size.y)) +
			tex2D(tex, uv + float2(-size.x, 0)) + tex2D(tex, uv + float2(0, 0)) + tex2D(tex, uv + float2(size.x, 0)) +
			tex2D(tex, uv + float2(-size.x, -size.y)) + tex2D(tex, uv + float2(0, -size.y)) + tex2D(tex, uv + float2(size.x, -size.y));

		return c / 9;

	}*/

	float4 frag(v2f i) : SV_Target
	{
		fixed4 main = tex2D(_MainTex, i.uv);
		
	float4 color1 = tex2D(_MainTex, i.uv);// *tex2D(_FogTex, i.uv).x * 0;
		color1 = lerp(main, color1, 0.05);
		if (tex2D(_FogTex, i.uv).x <= 0.5)
		{
			color1 = main;
		}

		float2 distScroll = float2(_Time.x/10, _Time.x);
		fixed2 dist = (tex2D(_DistTex, i.uv + distScroll).rg - 2) * 2;
		fixed4 col = tex2D(_FogTex, i.uv + dist * 0.025) * color1;
		col.a = 0.5;
		//col = lerp(col, color1, 0.05);
		fixed bg = col.a;

		fixed4 motion2 = tex2D(_Motion, i.uv);

		if (_MotionSpeed)
			motion2.y -= _Time.x * _MotionSpeed;
		else
			motion2 = fixed4(i.uv.rg, motion2.b, motion2.a);

		fixed4 effect2 = tex2D(_FogTex, motion2.rg) * motion2.a;
		//effect2 *= _EffectsLayer2Color;

		col += effect2 *effect2.a * max(bg, _EffectsLayer2Foreground);
		
		return col;
		}

			ENDCG
		}
	}
	
}
