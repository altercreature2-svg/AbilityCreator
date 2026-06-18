using AC;
using System;
using UnityEngine;
using UnityEngine.EventSystems;


// Token: 0x02000002 RID: 2
public class NodeSceneMovement : MonoBehaviour, IDragHandler, IEventSystemHandler
{
    public RectTransform dragRectTransform;
    public NodeManager nodeman;
    private void Awake()
    {
        nodeman = FindObjectOfType<NodeManager>();
    }
    // Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
    public void OnDrag(PointerEventData eventData)
    {
        if (nodeman.CanMove == false)
        {
            return;
        }
        if (eventData.button == PointerEventData.InputButton.Middle)
        {
            var node = FindObjectsOfType<NodeComponent>();
            for (int i = 0; i < node.Length; i++)
            {
                node[i].GetComponent<RectTransform>().anchoredPosition += eventData.delta/2;
            }
        }
        //this.dragRectTransform.anchoredPosition += eventData.delta / 2;

    }

    // Token: 0x06000002 RID: 2 RVA: 0x0000206E File Offset: 0x0000026E
    public NodeSceneMovement()
    {
    }

    // Token: 0x04000001 RID: 1
 
}
