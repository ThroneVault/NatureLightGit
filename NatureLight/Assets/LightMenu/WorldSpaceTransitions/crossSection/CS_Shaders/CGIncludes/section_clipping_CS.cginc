//section_clipping_CS.cginc

#ifndef PLANE_CLIPPING_INCLUDED
// Upgrade NOTE: excluded shader from DX11, OpenGL ES 2.0 because it uses unsized arrays
//#pragma exclude_renderers d3d11 gles
#define PLANE_CLIPPING_INCLUDED

//Plane clipping definitions

#if CLIP_PLANE || CLIP_TWO_PLANES || CLIP_SPHERE || CLIP_CUBE || CLIP_TUBES || CLIP_BOX || CLIP_CORNER || FADE_PLANE || FADE_SPHERE
	//PLANE_CLIPPING_ENABLED will be defined.
	//This makes it easier to check if this feature is available or not.
	#define PLANE_CLIPPING_ENABLED 1


	float randFromTexture(float3 co, sampler2D _noise, float _noiseScale)
	 {
		 co *= _noiseScale;
		 float x = frac(co.x);
		 float y = frac(co.y);
		 float z = frac(co.z);
		 float zy = 8*frac(8*z) - frac(8*frac(8*z));
		 float zx = 8*z - frac(8*z);
		 float3 col = tex2D(_noise, float2((zx + x)/8,(zy + y)/8));
		 return col.r;
	 }

#if CLIP_PLANE || CLIP_TWO_PLANES || CLIP_CUBE || CLIP_SPHERE || FADE_PLANE || FADE_SPHERE
	uniform float _SectionOffset = 0;
	uniform float3 _SectionPlane;
	uniform float3 _SectionPoint;

	#if CLIP_TWO_PLANES || CLIP_CUBE
	uniform float3 _SectionPlane2;
	#endif
	#if CLIP_SPHERE || CLIP_CUBE || FADE_SPHERE || FADE_PLANE
	uniform float _Radius = 0;
	#endif

	#if CLIP_CUBE
	static const float3 _SectionPlane3 = normalize(cross(_SectionPlane, _SectionPlane2));
	#endif
#endif

#if CLIP_PLANE || CLIP_CUBE || CLIP_SPHERE || FADE_PLANE || FADE_SPHERE || CLIP_BOX || CLIP_TUBES
	fixed _inverse;
#endif

#if FADE_SPHERE || FADE_PLANE 
	uniform sampler2D _Curves;
	uniform fixed _spread = 1;

	#if SCREENDISSOLVE || SCREENDISSOLVE_GLOW
		uniform sampler2D _ScreenNoise;
		float4 _ScreenNoise_TexelSize;
		uniform float _ScreenNoiseScale;
	#endif
	#if DISSOLVE || DISSOLVE_GLOW
		uniform float _Noise3dScale;
		uniform sampler2DArray _NoiseArray;
		uniform float _NoiseArraySliceRange;
	#endif
#endif

#if CLIP_TUBES
	//I couldn't get arrays to work here;
	/*uniform float4 _AxisDirs [2] = {float4(0.12,-0.6,0.8,0),float4(-0.2,0.65,-0.73,0)};
	uniform float4 _hitPoints [2] = {float4(0.12,2,0.2,0),float4(-0.6,2,-0.3,0)};
	uniform float _Radiuses [2] = {float(1),float(1)};*/

	uniform float3 _AxisDir0;
	uniform float3 _AxisDir1;
	uniform float3 _AxisDir2;
	uniform float3 _AxisDir3;
	uniform float3 _AxisDir4;

	uniform float3 _hitPoint0;
	uniform float3 _hitPoint1;
	uniform float3 _hitPoint2;
	uniform float3 _hitPoint3;
	uniform float3 _hitPoint4;

	uniform float _Rad0;
	uniform float _Rad1;
	uniform float _Rad2;
	uniform float _Rad3;
	uniform float _Rad4;
	
	uniform int _hitCount = 0; 
#endif

#if CLIP_BOX ||  CLIP_CORNER

	#if CLIP_CORNER
	fixed4 _SectionCentre;
	fixed4 _SectionDirX;
	fixed4 _SectionDirY;
	fixed4 _SectionDirZ;
	#endif

	#if CLIP_BOX
	fixed4 _SectionCentre[10];
	fixed4 _SectionDirX[10];
	fixed4 _SectionDirY[10];
	fixed4 _SectionDirZ[10];
	fixed4 _SectionScale[10];

	fixed  _Count;//数组遍历

		static const float dotCamX = dot(_WorldSpaceCameraPos - _SectionCentre[0] - _SectionDirX[0] *0.5*_SectionScale[0].x, _SectionDirX[0]);
		static const float dotCamXback = dot(_WorldSpaceCameraPos - _SectionCentre[0] + _SectionDirX[0] *0.5*_SectionScale[0].x, -_SectionDirX[0]);
		static const float dotCamY = dot(_WorldSpaceCameraPos - _SectionCentre[0] - _SectionDirY[0] *0.5*_SectionScale[0].y, _SectionDirY[0]);
		static const float dotCamYback = dot(_WorldSpaceCameraPos - _SectionCentre[0] + _SectionDirY[0] *0.5*_SectionScale[0].y, -_SectionDirY[0]);
		static const float dotCamZ = dot(_WorldSpaceCameraPos - _SectionCentre[0] - _SectionDirZ[0] *0.5*_SectionScale[0].z, _SectionDirZ[0]);
		static const float dotCamZback = dot(_WorldSpaceCameraPos - _SectionCentre[0] + _SectionDirZ[0] *0.5*_SectionScale[0].z, -_SectionDirZ[0]);
	#endif
#endif


	//discard drawing of a point in the world if it is behind any one of the planes.
	void PlaneClip(float3 posWorld) {
		#if CLIP_TWO_PLANES
		float3 vcross = cross(_SectionPlane,_SectionPlane2);
		if(vcross.y>=0){//<180
			bool _clip = false;
			_clip = _clip||(- dot((posWorld - _SectionPoint),_SectionPlane)<0);
			_clip = _clip||(- dot((posWorld - _SectionPoint),_SectionPlane2)<0);
			if(_clip) discard;

		}
		if(vcross.y<0){//>180
			if((_SectionOffset - dot((posWorld - _SectionPoint),_SectionPlane)<0)&&(- dot((posWorld - _SectionPoint),_SectionPlane2)<0)) discard;
		}
		//#else //
		#endif
		#if CLIP_PLANE
			#if DISSOLVE
			float dist = -dot((posWorld - _SectionPoint),_SectionPlane)*(1-2*_inverse);
			float transparency = saturate(1/_spread*dist + 0.5);
			#if FADE_PLANE
				float4 col = tex2D(_Curves, float2(transparency,1));
				transparency = col.r;
			#endif
			if(transparency<1)
				{
					if(randFromTexture(posWorld, _Noise, _NoiseScale)>transparency||transparency==0) discard;
				}
			#else
			if((_SectionOffset - dot((posWorld - _SectionPoint),_SectionPlane))*(1-2*_inverse)<0) discard;
			//if(((_SectionOffset - dot((posWorld - _SectionPoint + _SectionPlane*0.07),_SectionPlane))>0)||((_SectionOffset - dot((posWorld - _SectionPoint - _SectionPlane*0.07),_SectionPlane))<0)) discard;//two paralell planes test
			#endif
		#endif
		#if CLIP_SPHERE
			#if DISSOLVE
			float dist = length(posWorld - _SectionPoint);
			float transparency = (1-2*_inverse)*saturate(dist/_spread + 0.5 - _Radius/_spread);
			#if FADE_SPHERE
				float4 col = tex2D(_Curves, float2(transparency,1));
				transparency = col.r;
			#endif
			if(transparency<1)
				{
					if(randFromTexture(posWorld, _Noise, _NoiseScale)>transparency||transparency==0) discard;
				}

			#else
			if((1-2*_inverse)*(dot((posWorld - _SectionPoint),(posWorld - _SectionPoint)) - _Radius*_Radius)<0) discard; //_inverse = 1 : negative to clip the outside of the sphere
			#endif
		#endif

		#if CLIP_CUBE
		//if(_SectionOffset - dot((posWorld - _SectionPoint),_SectionPlane)<0) discard;
		//if(frac((posWorld - _SectionPoint),_SectionPlane) - 0.5>0) discard;
		fixed _sign = 1-2*_inverse;
		if((_SectionOffset - dot((posWorld - _SectionPoint -_Radius*_SectionPlane),-_SectionPlane)*_sign<0)&&(_SectionOffset - dot((posWorld - _SectionPoint +_Radius*_SectionPlane),-_SectionPlane)*_sign>0)
		&&(_SectionOffset - dot((posWorld - _SectionPoint -_Radius*_SectionPlane2),-_SectionPlane2)*_sign<0)&&(_SectionOffset - dot((posWorld - _SectionPoint +_Radius*_SectionPlane2),-_SectionPlane2)*_sign>0) 
		&&(_SectionOffset - dot((posWorld - _SectionPoint -_Radius*_SectionPlane3),-_SectionPlane3)*_sign<0)&&(_SectionOffset - dot((posWorld - _SectionPoint +_Radius*_SectionPlane3),-_SectionPlane3)*_sign>0))
		discard;
		//if((_SectionOffset - dot((posWorld - _SectionPoint -_Radius*_SectionPlane2),-_SectionPlane2)<0)&&(_SectionOffset - dot((posWorld - _SectionPoint +_Radius*_SectionPlane2),-_SectionPlane2)>0)) discard;
		#endif


		#if CLIP_TUBES
		//float3 posRel = posWorld - _SectionPoint;
		//float3 posCylinderRel = posRel - _AxisDir * dot(_AxisDir, posRel);
		//if ((dot(posCylinderRel,posCylinderRel) - _Radius*_Radius)<0) discard;
		bool _clip = false;

		if(_hitCount>0) _clip = _clip || ((dot(posWorld - _hitPoint0 - _AxisDir0 * dot(_AxisDir0, posWorld - _hitPoint0),posWorld - _hitPoint0 - _AxisDir0 * dot(_AxisDir0, posWorld - _hitPoint0)) - _Rad0*_Rad0)<0) ;
		if(_hitCount>1) _clip = _clip || ((dot(posWorld - _hitPoint1 - _AxisDir1 * dot(_AxisDir1, posWorld - _hitPoint1),posWorld - _hitPoint1 - _AxisDir1 * dot(_AxisDir1, posWorld - _hitPoint1)) - _Rad1*_Rad1)<0) ;
		if(_hitCount>2) _clip = _clip || ((dot(posWorld - _hitPoint2 - _AxisDir2 * dot(_AxisDir2, posWorld - _hitPoint2),posWorld - _hitPoint2 - _AxisDir2 * dot(_AxisDir2, posWorld - _hitPoint2)) - _Rad2*_Rad2)<0) ;
		if(_hitCount>3) _clip = _clip || ((dot(posWorld - _hitPoint3 - _AxisDir3 * dot(_AxisDir3, posWorld - _hitPoint3),posWorld - _hitPoint3 - _AxisDir3 * dot(_AxisDir3, posWorld - _hitPoint3)) - _Rad3*_Rad3)<0) ;
		if(_hitCount>4) _clip = _clip || ((dot(posWorld - _hitPoint4 - _AxisDir4 * dot(_AxisDir4, posWorld - _hitPoint4),posWorld - _hitPoint4 - _AxisDir4 * dot(_AxisDir4, posWorld - _hitPoint3)) - _Rad4*_Rad4)<0) ;
		//}
		if(_inverse==0)
		{
			if(_clip) discard;
		}
		else
		{
			if(!_clip) discard;
		}
		#endif

		#if CLIP_BOX

		//对每个Box进行遍历，查看是否在内部
		bool _clip = false;
		for (fixed index = 0; index < _Count; ++index)
		{
			float dotProdX = dot(posWorld - _SectionCentre[index] - _SectionDirX[index] * 0.5*_SectionScale[index].x, _SectionDirX[index]);
			float dotProdXback = dot(posWorld - _SectionCentre[index] + _SectionDirX[index] * 0.5*_SectionScale[index].x, -_SectionDirX[index]);
			float dotProdY = dot(posWorld - _SectionCentre[index] - _SectionDirY[index] * 0.5*_SectionScale[index].y, _SectionDirY[index]);
			float dotProdYback = dot(posWorld - _SectionCentre[index] + _SectionDirY[index] * 0.5*_SectionScale[index].y, -_SectionDirY[index]);
			float dotProdZ = dot(posWorld - _SectionCentre[index] - _SectionDirZ[index] * 0.5*_SectionScale[index].z, _SectionDirZ[index]);
			float dotProdZback = dot(posWorld - _SectionCentre[index] + _SectionDirZ[index] * 0.5*_SectionScale[index].z, -_SectionDirZ[index]);
			_clip = dotProdX < 0 && dotProdXback < 0 && dotProdY < 0 && dotProdYback < 0 && dotProdZ < 0 && dotProdZback < 0;
			if (_clip) break;
		}
		if (_clip) discard;

		#endif

		#if CLIP_CORNER
		float dotProdX = dot(posWorld - _SectionCentre, _SectionDirX);
		float dotProdY = dot(posWorld - _SectionCentre, _SectionDirY);
		float dotProdZ = dot(posWorld - _SectionCentre, _SectionDirZ);
		bool _clip = dotProdX > 0 && dotProdY > 0 && dotProdZ > 0;
		if(_clip) discard;
		#endif

	}
	#if CLIP_BOX
	void PlaneClipWithCaps(float3 posWorld) {
		
		float dotProdX = dot(posWorld - _SectionCentre[0] - _SectionDirX[0] *0.5*_SectionScale[0].x, _SectionDirX[0]);
		float dotProdXback = dot(posWorld - _SectionCentre[0] + _SectionDirX[0] *0.5*_SectionScale[0].x, -_SectionDirX[0]);
		float dotProdY = dot(posWorld - _SectionCentre[0] - _SectionDirY[0] *0.5*_SectionScale[0].y, _SectionDirY[0]);
		float dotProdYback = dot(posWorld - _SectionCentre[0] + _SectionDirY[0] *0.5*_SectionScale[0].y, -_SectionDirY[0]);
		float dotProdZ = dot(posWorld - _SectionCentre[0] - _SectionDirZ[0] *0.5*_SectionScale[0].z, _SectionDirZ[0]);
		float dotProdZback = dot(posWorld - _SectionCentre[0] + _SectionDirZ[0] *0.5*_SectionScale[0].z, -_SectionDirZ[0]);

		bool _clip = (dotProdX > 0 && dotCamX > 0)||(dotProdXback > 0 && dotCamXback > 0)||(dotProdY > 0 && dotCamY > 0)||(dotProdYback > 0 && dotCamYback > 0)||(dotProdZ > 0 && dotCamZ > 0)||(dotProdZback > 0 && dotCamZback > 0);
		if(_inverse==1) _clip = dotProdX <= 0 && dotProdXback <= 0 && dotProdY <= 0 && dotProdYback <= 0 && dotProdZ <= 0 && dotProdZback <= 0;
		if(_clip) discard;
	}

	#define PLANE_CLIPWITHCAPS(posWorld) PlaneClipWithCaps(posWorld); //preprocessor macro that will produce an empty block if no clipping planes are used.
	#endif

	#if FADE_PLANE || FADE_SPHERE
		inline float4 fadeTransition(float3 posWorld)
		{
			#if FADE_PLANE
			float dist = -dot((posWorld - _SectionPoint),_SectionPlane)*(1-2*_inverse);
			float transparency = saturate(dist/_spread + 0.5);
			float4 col = tex2D(_Curves, float2(transparency,1));
			float4 rgbcol = tex2D(_Curves, float2(transparency,0.5));
			rgbcol.a = col.r;
			return rgbcol;
			#endif
			#if FADE_SPHERE
			float dist = length(posWorld - _SectionPoint);
			float transparency = (1-2*_inverse)*saturate(dist/_spread + 0.5 - _Radius/_spread);
			float4 col = tex2D(_Curves, float2(transparency,1));
			float4 rgbcol = tex2D(_Curves, float2(transparency,0.5));
			rgbcol.a = col.r;
			return rgbcol;
			#endif
		}

		inline float fadeTransparency(float3 posWorld)
		{
			float transparency = 0;
			#if FADE_PLANE
			float dist = -dot((posWorld - _SectionPoint),_SectionPlane);//*(1-2*_inverse);
			transparency = (dist/_spread + 0.5);
			#endif
			#if FADE_SPHERE
			float dist = length(posWorld - _SectionPoint);
			transparency = (dist/_spread + 0.5 - _Radius/_spread);//*(1-2*_inverse);
			#endif
			return transparency;
		}
		#define TRANSFADE(posWorld) fadeTransparency(posWorld);
		#define PLANE_FADE(posWorld) fadeTransition(posWorld);
	#endif

//preprocessor macro that will produce an empty block if no clipping planes are used.
#define PLANE_CLIP(posWorld) PlaneClip(posWorld);
    
#else
//empty definition
#define PLANE_CLIP(s)
#define PLANE_CLIPWITHCAPS(s) //empty definition
//#define PLANE_FADE(s)
#endif


#endif // PLANE_CLIPPING_INCLUDED