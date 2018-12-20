using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonPressed : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IPointerEnterHandler
{
    public float Ping;
    private bool IsStart = false;
    private float LastTime = 0;

    void Update()
    {
        if (IsStart && Time.time - LastTime > Ping)
        {
            IsStart = false;
            Debug.Log("长按");
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        LongPress(true);
        Debug.Log("按下");
        transform.GetComponent<Toggle>().isOn = true;
    }
    public void OnPointerUp(PointerEventData eventData)
    {
      
            LongPress(false);
            Debug.Log("抬起");
        
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("离开");
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("进入");
    }
    public void LongPress(bool bStart)
    {
        IsStart = bStart;
        LastTime = Time.time;
    }
}
