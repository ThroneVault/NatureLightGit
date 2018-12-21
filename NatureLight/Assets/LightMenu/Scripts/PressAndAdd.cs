using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CreateAndOperate
{
    //放在添加物体的Toggle上
    public class PressAndAdd : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [Header("CursorStyle")]
        [Tooltip("鼠标样式-可添加")]
        public Texture2D CursorTextureAdd;

        [Tooltip("鼠标样式-不可添加")]
        public Texture2D CursorTextureNoAdd;

        [Space(10)]
        [Header("AddObject")]
        [Tooltip("用于添加的prefab")]
        public GameObject OriginPrefab;

        [Tooltip("创建的物体将作为它的子物体")]
        public GameObject FatherObject;

        //缓存创建的实例
        private GameObject GameObjectAdd;


        private bool CanRayCast = false;

        private RaycastHit Hit = new RaycastHit();


        void Start()
        {
            if (FatherObject == null)
            {
                FatherObject = new GameObject(OriginPrefab.name + "_Farther");
            }
        }

        void Update()
        {
            //开始追踪
            if (CanRayCast)
            {

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                //追踪成功，创建物体实例并实时更改位置
                if (Physics.Raycast(ray, out Hit, 10000f, 1 << 9) && !EventSystem.current.IsPointerOverGameObject())
                {

                    if (GameObjectAdd == null)
                    {
                        GameObjectAdd = GameObject.Instantiate(OriginPrefab);

                        //加到一个父物体中保存
                        FatherObject.GetComponent<AllCreatedPrefabs>().AddChild(GameObjectAdd);

                        //将鼠标样式改为可加
                        Cursor.SetCursor(CursorTextureAdd, Vector2.zero, CursorMode.Auto);
                    }
                    else
                    {
                        GameObjectAdd.transform.position = Hit.point;
                        GameObjectAdd.transform.forward = Hit.normal;
                    }
                }
                else//追踪失败，销毁缓存的物体
                {
                    if (GameObjectAdd != null)
                    {
                        Destroy(GameObjectAdd);
                        //
                        FatherObject.GetComponent<AllCreatedPrefabs>().DestroyChild(GameObjectAdd);
                    }

                    //将鼠标样式改为不能加
                    Cursor.SetCursor(CursorTextureNoAdd, Vector2.zero, CursorMode.Auto);

                }

            }

        }

        public void OnPointerDown(PointerEventData eventData)
        {
            //按下时切换为开的状态，而不是点击才换
            transform.GetComponent<Toggle>().isOn = true;

            CanRayCast = true;
            GameObjectAdd = null;
            //将鼠标样式改为不能加
            Cursor.SetCursor(CursorTextureNoAdd, Vector2.zero, CursorMode.Auto);

        }

        public void OnPointerUp(PointerEventData eventData)
        {
            CanRayCast = false;

            //将鼠标样式改为正常
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

            if (GameObjectAdd != null)
            {
                FatherObject.GetComponent<AllCreatedPrefabs>().NewSeletedGameObject(GameObjectAdd);
            }
        }
    }

}

