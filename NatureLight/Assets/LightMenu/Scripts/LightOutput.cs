using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightOutput : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    //窗地比
    public float GetWindowGroundRatio(float WindowArea, float GroundArea)
    {
        float Ratio = WindowArea / GroundArea;

        //保留两位小数
        return Mathf.Round(100* Ratio) /100f;
    }

    //采光有效进深, 地面宽度除以窗高？
    public float GetDepthRatio( float GroundWidth, float WindowHeight)
    {
        float Ratio = GroundWidth / WindowHeight;
        return Mathf.Round(100 * Ratio) / 100f;
    }

    //采光系数
    public float GetDaylightFactor(float Illumination , float IlluminationOut)
    {
        float Factor = Illumination / IlluminationOut;
        return Mathf.Round(100 * Factor) / 100f;
    }

    //室内某一点的天然光照度（侧窗）
    public float GetIllumination(Vector3 PointPosition, GameObject Window)
    {


     //   float Angle_1 = Mathf.Atan(Point);

        float Angle_2 = 2;
 

        //立体角
        float SolidAngle=0;

        //仰角(高度角)
        float ElevationAngle=0;

        //天顶的亮度 cd/cm2
        float LuminanceTop = 0;

        //天空在 仰角上的亮度 
        float Luminance = 0.3f * (1 + 2 * Mathf.Sin(ElevationAngle)) * LuminanceTop;

        //这个点的照度
        float Illumination = SolidAngle * Mathf.Cos(ElevationAngle);


        return Illumination;
    }



    //得到平均采光系数
    public float GetDaylightFactorAV()
    {
        return 1f;
    }

    //采光均匀度

}
