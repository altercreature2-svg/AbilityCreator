using IDK;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;


// Token: 0x02000002 RID: 2
public class NodeWindow : MonoBehaviour, IDragHandler, IEventSystemHandler
{
    public RectTransform dragRectTransform;
    public NodeManager NodeManager
    {
        get
        {
            return FindObjectOfType<NodeManager>();
        }
    }
    private void Awake()
    {
    }
    // Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
    public void OnDrag(PointerEventData eventData)
    {
        if (NodeManager.CanMove == false)
        {
            return;
        }
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (NodeManager.SelectedNodes.Contains(GetComponent<Node>()))
            {
                for (int i = 0; i < NodeManager.SelectedNodes.Length; i++)
                {
                    try
                    {
                        NodeManager.SelectedNodes[i].GetComponent<NodeWindow>().dragRectTransform.anchoredPosition += eventData.delta / 2; ;
                    }
                    catch (Exception)
                    {

                    }
                    
                }
                
            }
            else
            {
                this.dragRectTransform.anchoredPosition += eventData.delta / 2;
            }
        }
        

    }

    // Token: 0x06000002 RID: 2 RVA: 0x0000206E File Offset: 0x0000026E
    public NodeWindow()
    {
    }

    // Token: 0x04000001 RID: 1
 
}
