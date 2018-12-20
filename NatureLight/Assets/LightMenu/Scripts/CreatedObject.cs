using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CreateAndOperate
{
   //父类，提供统一的接口，子类添加在prefab上
    public abstract class CreatedObject : MonoBehaviour
    {

        //区分不同的状态
        public enum State { Preview, Normal, Translation, Rotation, Scaling };

        //物体的类型
        public enum Type { Light, Window };

        //当前的状态
        public State CurrentState = State.Normal;

        //物体的类型
        public Type ObjectType = Type.Light;


        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        //改变不同的状态
        public abstract State SwitchState(State NewState);

        //当物体被选中时
        //public abstract void OnMouseDown();

  
    }
}

