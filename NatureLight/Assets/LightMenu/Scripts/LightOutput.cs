using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightOutput : MonoBehaviour {


    public Vector3 PointPosition;
    public Vector3 WindowPosition;
    public float WindowWidth; 
    public float WindowHeight;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        GetIllumination(PointPosition, WindowPosition,  WindowWidth, WindowHeight);

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
    public float GetIllumination(Vector3 PointPosition, Vector3 WindowPosition, float WindowWidth, float WindowHeight)
    {

        float ElevationAngle_1 = Mathf.Atan(((WindowPosition.y + 0.5f * WindowHeight) - PointPosition.y) / (WindowPosition.z - PointPosition.z));

        float ElevationAngle_2 = Mathf.Atan(((WindowPosition.y - 0.5f * WindowHeight) - PointPosition.y) / (WindowPosition.z - PointPosition.z));


        float DirectionAngle_1 = Mathf.Atan(((WindowPosition.x + 0.5f * WindowWidth) - PointPosition.x) / (WindowPosition.z - PointPosition.z));

        float DirectionAngle_2 = Mathf.Atan(((WindowPosition.x - 0.5f * WindowWidth) - PointPosition.x) / (WindowPosition.z - PointPosition.z));


        float Sub = 4f * Mathf.Pow(Mathf.Sin(ElevationAngle_2), 3) - 3f * Mathf.Pow(Mathf.Cos(ElevationAngle_2), 2) - 4f * Mathf.Pow(Mathf.Sin(ElevationAngle_1), 3) + 3f * Mathf.Pow(Mathf.Cos(ElevationAngle_1), 2);
        //采光系数
        float DaylightFactor = (DirectionAngle_2-DirectionAngle_1)*Sub/(14f* Mathf.PI);

        Debug.Log(DaylightFactor);
   
        return DaylightFactor;
    }



    //得到平均采光系数
    public float GetDaylightFactorAV()
    {
        return 1f;
    }

    //采光均匀度

}
