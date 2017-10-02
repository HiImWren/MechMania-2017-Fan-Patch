Shader "Sci-Fi Space Station/PBR Basic" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,2)) = 1.0
		
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_MetallicGlossMap("Metallic (RGB) Gloss (A)", 2D) = "black" {}
	
		_OcclusionStrength("Occlusion Strength", Range(0.0, 1.0)) = 1.0

		_EmissionColor("Emission Color", Color) = (1,1,1,1)
		_EmissionMap("Emissive", 2D) = "black" {}
		_EmissionFactor("Emissive Factor", Float) = 1.0

		_BumpScale("Normal Factor", Float) = 1.0
		_BumpMap("Normal Map", 2D) = "bump" {}

		//Colormask Values
		_ColorMask("Color Mask  (RGB)", 2D) = "black" {}
		_MaskedColorA ("Masked Color 1 (R)", Color) = (1,0,0,1)
		_MaskedColorB ("Masked Color 2 (G)", Color) = (0,1,0,1)
		_MaskedColorC ("Masked Color 3 (B)", Color) = (0,0,1,1)
		
	}

	

	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex, _MetallicGlossMap,  _EmissionMap, _BumpMap, _Rough; //_OcclusionMap
	
		struct Input {
			float2 uv_MainTex;
		};

		half _Glossiness, _Metallic, _EmissionFactor, _BumpScale, _OcclusionStrength, _RoughLerp;
	
		fixed4 _Color, _EmissionColor;
		
		//Color Mask 
		sampler2D _ColorMask;
		fixed4 _MaskedColorA, _MaskedColorB, _MaskedColorC;

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			
			half2 RescaledUV = IN.uv_MainTex;
					
			fixed4 c = tex2D (_MainTex, RescaledUV) * _Color;
			
			fixed4 colorMask = tex2D(_ColorMask, RescaledUV);
			fixed3 FinalMaskedColors = (_MaskedColorA * colorMask.r) + (_MaskedColorB * colorMask.g) + (c.rgb *(_MaskedColorC * colorMask.b));

			c.rgb = lerp(c.rgb, FinalMaskedColors, colorMask.r + colorMask.g +  colorMask.b);

			o.Albedo = c.rgb;

			float MetallicMap = tex2D(_MetallicGlossMap, RescaledUV).r;
			float GlossMap = tex2D(_MetallicGlossMap, RescaledUV).a;
		
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic * MetallicMap;

			o.Smoothness = _Glossiness * GlossMap;

			o.Normal = UnpackScaleNormal(tex2D(_BumpMap, RescaledUV), _BumpScale);
		
			half occ = tex2D(_MetallicGlossMap, RescaledUV).b;
			o.Occlusion = LerpOneTo(occ , _OcclusionStrength );
			o.Occlusion = 1.0;
			o.Alpha = c.a;

			o.Emission = tex2D(_EmissionMap, RescaledUV) * _EmissionColor * _EmissionFactor;
		}
		ENDCG
	} 
	FallBack "Specular"
}
