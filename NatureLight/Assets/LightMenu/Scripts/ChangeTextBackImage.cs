using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

//make the back image fit the Text
public class ChangeTextBackImage : BaseMeshEffect

{

    public GameObject backImage;//层级不重要，在改变大小的时候重新指定了父节点
    private Rect rect;

    protected override void Start()
    {

    }

    public override void ModifyMesh(VertexHelper vh)
    {

        if (!IsActive())
        {
            return;
        }

        float topX = 1000000;
        float topY = 1000000;
        float bottomX = -1000000;
        float bottomY = -1000000;

        var stream = new List<UIVertex>();
        vh.GetUIVertexStream(stream);

        foreach (var item in stream)
        {
            var pos = item.position;
            if (topX > pos.x)
            {
                topX = pos.x;
            }
            if (topY > pos.y)
            {
                topY = pos.y;
            }
            if (bottomX < pos.x)
            {
                bottomX = pos.x;
            }
            if (bottomY < pos.y)
            {
                bottomY = pos.y;
            }
        }

        rect = new Rect(topX, topY, bottomX - topX, bottomY - topY);

        var rt = GetComponent<RectTransform>();

        //计算相对于左下角的位置 如果能够保证rt.pivot为0,0则可以不用此步
        rect.x += rt.pivot.x * rt.rect.size.x;
        rect.y += rt.pivot.y * rt.rect.size.y;

        //计算相对于父节点的位置
        var parentSize = transform.parent.gameObject.GetComponent<RectTransform>().rect.size;
        rect.x += rt.offsetMin.x + rt.anchorMin.x * parentSize.x;
        rect.y += rt.offsetMin.y + rt.anchorMin.y * parentSize.y;

    }

    void OnGUI()
    {
        var backImageRect = backImage.GetComponent<RectTransform>();
        backImageRect.SetParent(transform.parent);
        backImageRect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, rect.x, rect.width);
        backImageRect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, rect.y, rect.height);
        backImage.SetActive(true);
    }

}
