using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Artngame.GISIM
{
    public class GISimulate : MonoBehaviour
    {

        [Header("Bounce Lights Setting")]
        [Tooltip("更新的时间周期")]
        public float UpdatePeriod = 0.1f;

        [Tooltip("最终强度 = BounceIntensityRatio*Light Source 的强度*")]
        [Range(0, 1)]
        public float BounceIntensityRatio = 0.2f;

        [Tooltip("BounceLight的半径")]
        public float BounceRange = 5f;

        [Tooltip("BounceLight到投射点和 光源到投射点的比值")]
        [Range(0, 1)]
        public float OffSetRatio = 0.2f;
        [Tooltip("在法线方向偏移")]
        public bool OffSetOnNormal;

        [Tooltip("动态更新颜色")]
        public bool DynamicUpdate = true;

        public bool Debug_on = false;

        [Tooltip("在摄像机规定范围内才生效")]
        public float InCameraRange = 30f;

        [Tooltip("二次反射")]
        public bool SecondBounce = false;

        //使得光照改变时比较平滑
        private float Degrade_speed = 0.6f; //speed that uneeded lights go out
        private float Appear_speed = 2f;



        //保存每一条射线 的各种信息
        struct RayCastStruct
        {
            public bool AllowAdd;
            public Transform BounceLight;
            public Transform LightSource;
            public int LightLevel;
            public RaycastHit HitResult;
            public int Index;

            //  public  Color LightsDestinationColor;
            //  public Vector3 RayDestPostion;//
        }

        //用来保存每个Bounce_Lights的信息
        private List<RayCastStruct> RayCasts = new List<RayCastStruct>();

        //用来保存每次追踪的信息
        private List<RaycastHit> HitResults;

        //缓存所有的BounceLights
        private GameObject LightPool;

        //从一点多个方向的进行射线追踪，定义这些方向(用于点光源)
        private Vector3[] SphereDirections;

        //追踪的数量
        public int DirectionAmount = 10;

        //定向光源
        //从多个点往同一方向射线追踪(用于定向光源)
        private Vector3[] CastOriginPositions;

        //正方形网格的边长个数
        public int OriginPositionAmount = 10;

        //网格的间距
        public float Distance = 1;


        // for UpdatePeriod
        private float CurrentTime;

        private bool HaveCreatLight;


        void Start()
        {
            //创建一个空物体用来缓存BounceLights
            LightPool = new GameObject("LightPool");

            //初始化点光源射线追踪的所有角度
            if (transform.GetComponent<Light>().type == LightType.Point || SecondBounce)
            {
                SphereDirections = GetSphereDirections(DirectionAmount);
            }

            //初始化定向光源射线追踪的所有起始位置
            if (transform.GetComponent<Light>().type == LightType.Directional)
            {
                CastOriginPositions = new Vector3[OriginPositionAmount * OriginPositionAmount];
                for (int i = 0; i < OriginPositionAmount; i++)
                {
                    for (int j = 0; i < OriginPositionAmount; i++)
                    {
                        CastOriginPositions[i * OriginPositionAmount + j] = transform.position + (Vector3.forward * i * Distance) + (Vector3.right * j * Distance);
                    }
                }
            }

        }

        void Update()
        {
            //有一定的间隔，用于节省性能
            if (Time.fixedTime - CurrentTime > UpdatePeriod)
            {
                CurrentTime = Time.fixedTime;

                //动态创建BounceLights
                //动态更新已有的BounceLights
                //动态销毁BouceLights
                UpdateBounceLights();


            }
        }

        void UpdateBounceLights()
        {
            //聚光源
            if (transform.GetComponent<Light>().type == LightType.Spot)
            {
 
                EachRayCast(transform.position, transform.forward, transform, 1, 0);

            }
            //定向光源
            else if (transform.GetComponent<Light>().type == LightType.Directional)
            {
                //矩形范围的追踪，

                for (int i = 0; i < CastOriginPositions.Length; i++)
                {

                    EachRayCast(CastOriginPositions[i], transform.forward, transform, 1, i);

                }

            }
            // 点光源
            else if (this.GetComponent<Light>().type == LightType.Point)
            {
                for (int i = 0; i < SphereDirections.Length; i++)
                {

                    EachRayCast(transform.position, SphereDirections[i], transform, 1, i);

                }
            }

            else
            {
                Debug.Log("This is no light, Please add this Script to a light");
            }

            // 第二次反弹光，按点光源来
            if (SecondBounce)
            {
                for (int i = 0; i < RayCasts.Count; i++)
                {
                    if (RayCasts[i].LightLevel == 1)
                    {
                        for (int j = 0; j < SphereDirections.Length; j++)
                        {
                            EachRayCast(RayCasts[i].BounceLight.position, SphereDirections[j], RayCasts[i].BounceLight, 2, i* SphereDirections.Length + j);

                        }
                    }
                }
            }
        }



        //第Index号的射线追踪，如果存在BounceLight，则更新颜色和位置，如果不存在，则创建并加入数组
        void EachRayCast(Vector3 OriginPosition, Vector3 Direction, Transform LightSource, int LightLevel, int Index)
        {

            Vector3 Offseted_hit_point;

            //第Index号的射线追踪，如果存在BounceLight，则更新颜色和位置，如果不存在，则创建并加入数组
            RaycastHit HitResult = new RaycastHit();

            bool HaveExist = false;
            RayCastStruct ExistRayCast = new RayCastStruct();

            for(int i = 0; i < RayCasts.Count; i++)
            {
                if(RayCasts[i].LightLevel == LightLevel && RayCasts[i].Index == Index)
                {
                    HaveExist = true;
                    ExistRayCast = RayCasts[i];
                    break;
                }
            }

            if (Physics.Raycast(OriginPosition, Direction, out HitResult, Mathf.Infinity))
            {
   
                //不存在
                if (!HaveExist)
                {
                    GameObject BounceLight;
                    if (LightLevel == 1)
                    {
                        BounceLight = new GameObject("Bounce Light");
                    }
                    else
                    {
                        BounceLight = new GameObject("Bounce Light 2");
                    }
                    BounceLight.transform.parent = LightPool.transform;

                    // Add the light component
                    BounceLight.AddComponent(typeof(Light));
                    // Set color and position
                    BounceLight.GetComponent<Light>().color = LightSource.GetComponent<Light>().color;
                    BounceLight.GetComponent<Light>().intensity = BounceIntensityRatio * LightSource.GetComponent<Light>().intensity;
                    BounceLight.GetComponent<Light>().range = BounceRange;
                    //偏移的方式是不是法线方向
                    if (OffSetOnNormal)
                    {
                        Offseted_hit_point = HitResult.point + (HitResult.normal).normalized * OffSetRatio * Vector3.Distance(transform.position, HitResult.point);
                    }
                    else
                    {
                        Offseted_hit_point = HitResult.point + (transform.position - HitResult.point).normalized * OffSetRatio * Vector3.Distance(transform.position, HitResult.point);
                    }
                    BounceLight.transform.position = Offseted_hit_point;

                    //将新的BounceLight保存到数组中
                    RayCastStruct newRayCast = new RayCastStruct();
                    newRayCast.BounceLight = BounceLight.transform;
                    newRayCast.LightSource = LightSource;
                    newRayCast.LightLevel = LightLevel;
                    newRayCast.Index = Index;
                    RayCasts.Add(newRayCast);
                }
                else//存在BounceLight
                {

                    Transform BounceLight = ExistRayCast.BounceLight;
                    BounceLight.GetComponent<Light>().color = GetColor(LightSource, HitResult, 0.8f);
                    BounceLight.GetComponent<Light>().intensity = BounceIntensityRatio * LightSource.GetComponent<Light>().intensity;
                    BounceLight.GetComponent<Light>().range = BounceRange;
                    if (OffSetOnNormal)
                    {
                        Offseted_hit_point = HitResult.point + (HitResult.normal).normalized * OffSetRatio * Vector3.Distance(transform.position, HitResult.point);
                    }
                    else
                    {
                        Offseted_hit_point = HitResult.point + (transform.position - HitResult.point).normalized * OffSetRatio * Vector3.Distance(transform.position, HitResult.point);
                    }
                    BounceLight.position = Offseted_hit_point;

                }
                if (Debug_on)
                {
                    Debug.DrawLine(OriginPosition, Offseted_hit_point, Color.magenta);
                }

            }
            else
            {
                Debug.Log("射线追踪失效，可能没有碰撞");
                //将存在的BounceLights摧毁
            }

        }

        //得到一组放射性的方向
        private Vector3[] GetSphereDirections(int numDirections)
        {
            Vector3[] pts = new Vector3[numDirections];
            float inc = Mathf.PI * (3 - Mathf.Sqrt(5));
            float off = 2f / numDirections;


            for (int k = 0; k < numDirections; k++)
            {
                var y = k * off - 1 + (off / 2);
                var r = Mathf.Sqrt(1 - y * y);
                var phi = k * inc;
                var x = (float)(Mathf.Cos(phi) * r);
                var z = (float)(Mathf.Sin(phi) * r);
                pts[k] = new Vector3(x, y, z);
            }


            return pts;
        }

        //得到动态地颜色
        private Color GetColor(Transform LightSource, RaycastHit Hit, float ColorRatio)
        {

            Color Out_color = LightSource.GetComponent<Light>().color;

            if (Hit.transform.GetComponent<Renderer>() != null)
            {
                Out_color = Color.Lerp(LightSource.GetComponent<Light>().color, Color.Lerp(Hit.transform.GetComponent<Renderer>().material.color, GetComponent<Light>().color, 0.5f), ColorRatio);
            }

            return Out_color;
        }
    }
}
