using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonToggle : MonoBehaviour {

    //All Buttons with toggle
    [Tooltip("只有一个按钮会选上")]
    public List<GameObject> Buttons;

    public bool ifActivePlane;
   
    public List<GameObject> Planes;
  
	// Use this for initialization
	void Start () {
        //绑定事件到这些按钮的点击事件上
        foreach (GameObject item in Buttons)
        {
            item.GetComponent<Button>().onClick.AddListener(delegate
            {
                OnButtonClick(item);
            });
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    //效果是只选中了这个按钮
    public void OnButtonClick(GameObject button) {
        for(int i=0;i<Buttons.Count;i++)
        {
            if (Buttons[i] == button)
            {
                Buttons[i].GetComponent<Button>().interactable = false;

                //使对应的Plane菜单出现
                if (ifActivePlane)
                {
                    Planes[i].SetActive(true);
                }
            }
            else
            {
                Buttons[i].GetComponent<Button>().interactable = true;

                //触发消失
                if (ifActivePlane)
                {
                    Planes[i].SetActive(false);
                }
            }
        }
    }
}
