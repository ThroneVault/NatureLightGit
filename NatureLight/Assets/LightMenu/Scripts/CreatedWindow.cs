using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AdvancedGizmo;

namespace CreateAndOperate
{
    //放在window 的prefab下
    public class CreatedWindow : CreatedObject
    {

        //区分不同的状态

        public GameObject Gizmo;
        public GameObject CrossSectionBox;
        public GameObject Window;


        // Use this for initialization
        void Start()
        {
 
        }

        // Update is called once per frame
        void Update()
        {
            transform.position = Gizmo.transform.position;
            transform.rotation = Gizmo.transform.rotation;

        }

        void OnMouseDown()
        {

            print("被点击消失");

            // Destroy(this.gameObject);

        }

        //切换状态
        public override State SwitchState(State NewState) 
        {
            CurrentState = NewState;
            switch (NewState)
            {
                case State.Preview:
                    break;
                case State.Normal:
                    Gizmo.SetActive(false);
                    CrossSectionBox.SetActive(false);
                    break;
                case State.Translation:
                    Gizmo.SetActive(true);
                    if (Gizmo.GetComponent<Gizmo>().i == 1)
                    {
                        Gizmo.GetComponent<Gizmo>().ChangeMode();
                    }
                    CrossSectionBox.SetActive(false);
                    break;
                case State.Rotation:
                    Gizmo.SetActive(true);
                    if (Gizmo.GetComponent<Gizmo>().i == 0)
                    {
                        Gizmo.GetComponent<Gizmo>().ChangeMode();
                    }
                    CrossSectionBox.SetActive(false);
                    break;
                case State.Scaling:
                    Gizmo.SetActive(false);
                    CrossSectionBox.SetActive(true);
                    break;

                default:
                    break;
            }
            return NewState;
        }
    }

}

