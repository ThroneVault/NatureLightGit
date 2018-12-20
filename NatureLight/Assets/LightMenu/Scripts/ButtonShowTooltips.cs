using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

public class ButtonShowTooltips : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string DisplayName;
    private GameObject ToolTipsUI;
  
    private Canvas canvas;//所在的画布
    private RectTransform rectTransform;//2d坐标
    private Vector2 pos;

    private int rate = 5;
    private bool isBestFit = false;

    void Start()
    {
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        rectTransform = canvas.transform as RectTransform;
        ToolTipsUI = canvas.transform.Find("Tips").gameObject;


        //得到这个UI的2d位置
       
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, transform.position, canvas.GetComponent<Camera>(), out pos))
        {
            Debug.Log(pos);
        }

    }

    //当鼠标悬浮在这个UI上触发
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("hover");
        if (ToolTipsUI != null)
        {
            ToolTipsUI.GetComponentInChildren<Text>().text = DisplayName;
           // Vector2 pos;

            //得到鼠标在这个Canvas的2d位置
            //if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, Input.mousePosition, canvas.worldCamera, out pos))
            //{
            //    //将2D位置赋值给这个UI 并显示
            //   // ToolTipsUI.GetComponent<RectTransform>().anchoredPosition = pos;
            //}
            ToolTipsUI.GetComponent<RectTransform>().anchoredPosition = pos;
            ToolTipsUI.SetActive(true);
        }
        else
        {
            Debug.LogWarning("TooltipsUI is null");
        }
       // image.sprite = sprites[0];
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("hover end");
        if (ToolTipsUI != null)
        {
            ToolTipsUI.SetActive(false);
        }
        //image.sprite = sprites[1];
    }

}
