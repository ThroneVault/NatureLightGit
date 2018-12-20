Shader "CrossSection/Others/StencilledTexture"
{
	Properties
	{
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_StencilMask("Stencil Mask", Range(0, 255)) = 255
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue" = "Transparent" }

		LOD 100
		
		Stencil{
			Ref [_StencilMask]
			Comp Equal
			Pass Zero
		}

		CGPROGRAM
		#pragma surface surf Lambert
		#include "UnityCG.cginc"

		fixed4 _Color;
		sampler2D _MainTex;
		float4 _MainTex_ST;
		fixed4 _SectionDirX;
		fixed4 _SectionDirY;
		fixed4 _SectionDirZ;

		static const float3x3 projMatrix = float3x3(_SectionDirX.xyz, _SectionDirY.xyz, _SectionDirZ.xyz);
	      
		struct Input {
			float3 worldNormal;
			float3 worldPos;
		};

		void surf (Input IN, inout SurfaceOutput o) {
	        
			float2 UV;
			fixed4 c;
			float3 projPos = mul(projMatrix, IN.worldPos);
			float3 projNorm = mul(projMatrix, IN.worldNormal);

			if(abs(projNorm.x)>0.5)
			{
				UV = projPos.zy; // side
				c = tex2D(_MainTex, float2(UV.x*_MainTex_ST.x, UV.y*_MainTex_ST.y)); // use WALLSIDE texture
			}
			else if(abs(projNorm.z)>0.5)
			{
				UV = projPos.xy; // front
				c = tex2D(_MainTex, float2(UV.x*_MainTex_ST.x, UV.y*_MainTex_ST.y)); // use WALL texture
			}
			else
			{
				UV = projPos.xz; // top
				c = tex2D(_MainTex, float2(UV.x*_MainTex_ST.x, UV.y*_MainTex_ST.y)); // use FLR texture
			}
			o.Albedo = c.rgb * _Color.rgb;
			o.Alpha = c.a * _Color.a;
			
		}
		ENDCG
	}
	Fallback "Legacy Shaders/Transparent/VertexLit"
}
