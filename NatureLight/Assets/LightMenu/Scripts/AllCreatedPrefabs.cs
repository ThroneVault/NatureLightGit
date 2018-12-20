using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CreateAndOperate
{
    //保存所有物体，保存选中的物体，放在创建物体的父亲节点上
    public class AllCreatedPrefabs : MonoBehaviour
    {

        public GameObject Wall;
        public Material ClipMaterial;
        //所有物体
        [HideInInspector]
        public List<GameObject> AllCreatedGameObject;

        //所有选择的物体
        [HideInInspector]
        public List<GameObject> SeletedGameObject;


        public Vector4[] SectionDirX = new Vector4[10];
        public Vector4[] SectionDirY = new Vector4[10];
        public Vector4[] SectionDirZ = new Vector4[10];
        public Vector4[] SectionCentre = new Vector4[10];
        public Vector4[] SectionScale = new Vector4[10];

        private int index = 0;
        // Use this for initialization
        void Start()
        {
            ClipMaterial = Wall.GetComponent<Renderer>().material;
           
        }

        // Update is called once per frame
        void Update()
        {
            Shader.SetGlobalVectorArray("_SectionDirX", SectionDirX);
            Shader.SetGlobalVectorArray("_SectionDirY", SectionDirY);
            Shader.SetGlobalVectorArray("_SectionDirZ", SectionDirZ);
            Shader.SetGlobalVectorArray("_SectionCentre", SectionCentre);
            Shader.SetGlobalVectorArray("_SectionScale", SectionScale);


            Debug.Log(index);
        }


        public void AddChild(GameObject child)
        {
            child.transform.parent = transform;
            AllCreatedGameObject.Add(child);
            NewSeletedGameObject(child);

            //设置变量用来 挖洞
            child.GetComponentInChildren<CappedSectionFollow>().FatherObject = gameObject;
            child.GetComponentInChildren<CappedSectionFollow>().index = index;
            index++;
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

