using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CreateAndOperate
{
    //保存所有物体，保存选中的物体，将这个脚本放在父亲节点上
    public class AllCreatedPrefabs : MonoBehaviour
    {

     //   public GameObject Wall;
      //  public Material ClipMaterial;

        //所有物体
        [HideInInspector]
        public List<GameObject> AllCreatedGameObject;

        //所有选择的物体
        [HideInInspector]
        public List<GameObject> SeletedGameObject;


        //窗户的参数，用于挖洞
        [HideInInspector]
        public Vector4[] SectionDirX = new Vector4[10];
        [HideInInspector]
        public Vector4[] SectionDirY = new Vector4[10];
        [HideInInspector]
        public Vector4[] SectionDirZ = new Vector4[10];
        [HideInInspector]
        public Vector4[] SectionCentre = new Vector4[10];
        [HideInInspector]
        public Vector4[] SectionScale = new Vector4[10];

        private int index = 0;
        // Use this for initialization
        void Start()
        {
         //   ClipMaterial = Wall.GetComponent<Renderer>().material;
           
        }

        // Update is called once per frame
        void Update()
        {


            //将窗户参数组传入shader中
            Shader.SetGlobalVectorArray("_SectionDirX", SectionDirX);
            Shader.SetGlobalVectorArray("_SectionDirY", SectionDirY);
            Shader.SetGlobalVectorArray("_SectionDirZ", SectionDirZ);
            Shader.SetGlobalVectorArray("_SectionCentre", SectionCentre);
            Shader.SetGlobalVectorArray("_SectionScale", SectionScale);


        }


        //添加物体
        public void AddChild(GameObject Child)
        {
            Child.transform.parent = transform;
            AllCreatedGameObject.Add(Child);
            NewSeletedGameObject(Child);

            //设置变量用来 挖洞
            Child.GetComponentInChildren<CappedSectionFollow>().FatherScript = this;
            UpdateArray();


        }

        //删掉物体
        public void DestroyChild(GameObject Child)
        {
            AllCreatedGameObject.Remove(Child);
            UpdateArray();
        }



        //根据窗户数组更新参数组，用于挖洞
        void UpdateArray()
        {
            index = 0;
            for (int i = 0; i < AllCreatedGameObject.Count; i++)
            {
                if (AllCreatedGameObject[i].GetComponent<CreatedObject>().ObjectType == CreatedObject.Type.Window)
                {
                    AllCreatedGameObject[i].GetComponentInChildren<CappedSectionFollow>().index = index;
                    index++;
                }
            }
            Shader.SetGlobalInt("_Count", index);
        }


        //加到以选中的物体上
        public void AddSeletedGameObject(GameObject NewGameObject)
        {
            SeletedGameObject.Add(NewGameObject);
        }

        //重新选中物体
        public void NewSeletedGameObject(GameObject NewGameObject)
        {
            SeletedGameObject.Clear();
            SeletedGameObject.Add(NewGameObject);
        }


        //用于在Toggle上调用，改为平移状态
        public void ChangeTranslation(bool Toggle)
        {
            if (Toggle)
            {
                for (int i = 0; i < SeletedGameObject.Count; i++)
                {
                    SeletedGameObject[i].GetComponent<CreatedObject>().SwitchState(CreatedObject.State.Translation);
                }
            }
        }

        //改为旋转状态
        public void ChangeRotaion(bool Toggle)
        {
            if (Toggle)
            {
                for (int i = 0; i < SeletedGameObject.Count; i++)
                {
                    SeletedGameObject[i].GetComponent<CreatedObject>().SwitchState(CreatedObject.State.Rotation);
                }
            }
        }

        //改为放缩状态
        public void ChangeScaling(bool Toggle)
        {
            if (Toggle)
            {
                for (int i = 0; i < SeletedGameObject.Count; i++)
                {
                    SeletedGameObject[i].GetComponent<CreatedObject>().SwitchState(CreatedObject.State.Scaling);
                }
            }
        }
    }
}

