using Steamworks;
using System.Collections;
using UnityEngine;

namespace AC
{
    public class QuickLine : MonoBehaviour
    {
        public LineRenderer LineRenderer
        {
            get
            {
                LineRenderer lineRenderer = GetComponent<LineRenderer>();
                if (lineRenderer != null)
                    return lineRenderer;
                else
                {
                    lineRenderer = gameObject.AddComponent<LineRenderer>();
                    lineRenderer.positionCount = 2;
                    lineRenderer.widthMultiplier = 2f;
                    lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
                    return lineRenderer;
                }
            }
        }
        public Transform other;
        public void Update()
        {
            LineRenderer.SetPosition(0, (Vector2)transform.position);
            LineRenderer.SetPosition(1, (Vector2)other.transform.position);
        }
    }
}