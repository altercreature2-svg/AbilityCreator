using Mono.Cecil.Pdb;
using ProGrids;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace IDK.Node_Related_Scripts.connection_stuff
{

    public class NodeConnector : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IEventSystemHandler
    {
        public float curveHeight = .5f;
        public Vector3 vec = Vector3.left;
        public int segmentCount = 32;
        public bool isDragging;
        private Transform m_copy;
        public static Dictionary<NodeBlueprint.ConnectionType, Dictionary<NodeBlueprint.ConnectionType, string>> convertTable = new Dictionary<NodeBlueprint.ConnectionType, Dictionary<NodeBlueprint.ConnectionType, string>>()
        {
            {NodeBlueprint.ConnectionType.GiveUnit, new Dictionary<NodeBlueprint.ConnectionType, string>
                {
                    {NodeBlueprint.ConnectionType.ReciveAnything, "CONVERTUNIT" },
                    {NodeBlueprint.ConnectionType.ReciveGameObject, "GAMEOBJ" },
                }
            },
            {NodeBlueprint.ConnectionType.GiveGameObject, new Dictionary<NodeBlueprint.ConnectionType, string>
                {
                    {NodeBlueprint.ConnectionType.ReciveAnything, "CONVERTOBJ" },
                    {NodeBlueprint.ConnectionType.ReciveUnit, "GETUNIT" },
                    {NodeBlueprint.ConnectionType.ReciveComponent, "GETCOMP" },
                }
            },
            {NodeBlueprint.ConnectionType.GiveComponent, new Dictionary<NodeBlueprint.ConnectionType, string>
                {
                    {NodeBlueprint.ConnectionType.ReciveAnything, "CONVERTCOMP" },
                }
            },
            {NodeBlueprint.ConnectionType.GiveObjectVariable, new Dictionary<NodeBlueprint.ConnectionType, string>
                {
                    {NodeBlueprint.ConnectionType.ReciveAnything, "OBJECTVARIABLEVALUE" },
                }
            },
            {NodeBlueprint.ConnectionType.GiveAnything, new Dictionary<NodeBlueprint.ConnectionType, string>
                {
                    {NodeBlueprint.ConnectionType.ReciveUnit, "CONVERTUNIT2" },
                    {NodeBlueprint.ConnectionType.ReciveGameObject, "CONVERTOBJ2" },
                    {NodeBlueprint.ConnectionType.ReciveComponent, "CONVERTCOMP2" },
                }
            },
        };
        public static NodeBlueprint.ConnectionType FlipConnection(NodeBlueprint.ConnectionType connectionType)
        {
            if (connectionType is NodeBlueprint.ConnectionType.ReciveAnything)
                return NodeBlueprint.ConnectionType.GiveAnything;
            else if (connectionType is NodeBlueprint.ConnectionType.GiveAnything)
                return NodeBlueprint.ConnectionType.ReciveAnything;

            else if (connectionType is NodeBlueprint.ConnectionType.ReciveComponent)
                return NodeBlueprint.ConnectionType.GiveComponent;
            else if (connectionType is NodeBlueprint.ConnectionType.GiveComponent)
                return NodeBlueprint.ConnectionType.ReciveComponent;

            else if (connectionType is NodeBlueprint.ConnectionType.ReciveGameObject)
                return NodeBlueprint.ConnectionType.GiveGameObject;
            else if (connectionType is NodeBlueprint.ConnectionType.GiveGameObject)
                return NodeBlueprint.ConnectionType.ReciveGameObject;

            else if (connectionType is NodeBlueprint.ConnectionType.ReciveUnit)
                return NodeBlueprint.ConnectionType.GiveUnit;
            else if (connectionType is NodeBlueprint.ConnectionType.GiveUnit)
                return NodeBlueprint.ConnectionType.ReciveUnit;

            else if (connectionType is NodeBlueprint.ConnectionType.ReciveVariable)
                return NodeBlueprint.ConnectionType.GiveVariable;
            else if (connectionType is NodeBlueprint.ConnectionType.GiveVariable)
                return NodeBlueprint.ConnectionType.ReciveVariable;
            else if (connectionType is NodeBlueprint.ConnectionType.ReciveObjectVariable)
                return NodeBlueprint.ConnectionType.GiveObjectVariable;
            else if (connectionType is NodeBlueprint.ConnectionType.GiveObjectVariable)
                return NodeBlueprint.ConnectionType.ReciveObjectVariable;
            else if (connectionType is NodeBlueprint.ConnectionType.Trigger)
                return NodeBlueprint.ConnectionType.Triggered;
            else if (connectionType is NodeBlueprint.ConnectionType.Triggered)
                return NodeBlueprint.ConnectionType.Trigger;

            return NodeBlueprint.ConnectionType.Triggered;
        }
        public Transform Copy
        {
            get
            {
                if (m_copy == null)
                {
                    m_copy = new GameObject("Copy of:" + transform.name).transform;

                }
                m_copy.position = transform.position;
                return m_copy;
            }
        }
        public NodeConnector other;
        public LineRenderer m_lineRenderer;
        public LineRenderer LineRenderer
        {
            get
            {
                if (m_lineRenderer == null)
                {
                    m_lineRenderer = gameObject.AddComponent<LineRenderer>();
                    m_lineRenderer.widthMultiplier = .05f;
                    m_lineRenderer.positionCount = 2;
                    m_lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
                    Image renderer = GetComponent<Image>();
                    LineRenderer.startColor = renderer.color;
                    LineRenderer.endColor = renderer.color;
                    LineRenderer.material.color = renderer.color;
                }

                return m_lineRenderer;
            }
        }
        public NodeBlueprint.ConnectionType connectionType;
        void IDragHandler.OnDrag(PointerEventData eventData)
        {

            Vector3 mousePositon = Input.mousePosition;
            mousePositon.z = 0;
            if (eventData.button != PointerEventData.InputButton.Left)
                return;
            DrawCubicBezier(WorldPosition(transform.position), WorldPosition(mousePositon));

            //GetComponent<RectTransform>().anchoredPosition = (Vector2)Camera.current.ScreenToWorldPoint(mousePositon, Camera.MonoOrStereoscopicEye.Mono);
            if (other != null)
                other.other = null;

        }
        void IBeginDragHandler.OnBeginDrag(UnityEngine.EventSystems.PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            isDragging = true;
        }
        public Vector3 WorldPosition(Vector3 orignal)
        {
            Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(null, orignal);
            screenPos.z = 5;
            Vector3 vector3 = FindObjectOfType<Camera>().ScreenToWorldPoint(screenPos);
            return vector3;
        }
        void IEndDragHandler.OnEndDrag(UnityEngine.EventSystems.PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;
            isDragging = false;
            bool found = false;
            NodeConnector[] all = FindObjectsOfType<NodeConnector>();
            for (int i = 0; i < all.Length; i++)
            {

                if (all[i] == this)
                    continue;

                if (Vector2.Distance((Vector2)Input.mousePosition, all[i].transform.position) < 25)
                {
                    found = true;
                    Connect(all[i], out _);
                    break;
                }

            }
            if (!found)
            {
                if (other != null)
                {
                    other.other = null;
                    other = null;
                }
            }

        }
        public void RemoveAllConnections()
        {
            if (other != null)
            {
                other.other = null;
                other = null;
            }
        }
        public bool CanConnect(NodeConnector other)
        {
            return connectionType == FlipConnection(other.connectionType);
        }
        public void Connect(NodeConnector nodeConnector, out bool createdNewNodes)
        {
            bool canConnect = CanConnect(nodeConnector);
            if (canConnect)
            {
                other = nodeConnector;
                nodeConnector.other = this;
                createdNewNodes = false;
            }
            else
            {
                if (convertTable.ContainsKey(connectionType))
                {
                    if (convertTable[connectionType].ContainsKey(nodeConnector.connectionType))
                    {
                        Node node = Main.nodeDatabase[convertTable[connectionType][nodeConnector.connectionType]].Spawn();
                        node.transform.position = (transform.position + nodeConnector.transform.position) / 2;
                        Connect(node.GetComponentsInChildren<NodeConnector>().First(n => n.connectionType == FlipConnection(connectionType)), out _);
                        nodeConnector.Connect(node.GetComponentsInChildren<NodeConnector>().First(n => n.connectionType == FlipConnection(nodeConnector.connectionType)), out _);
                        createdNewNodes = true;
                        Debug.Log("Created new node!");
                    }
                }
                else if (convertTable.ContainsKey(nodeConnector.connectionType))
                {
                    if (convertTable[nodeConnector.connectionType].ContainsKey(connectionType))
                    {
                        Node node = Main.nodeDatabase[convertTable[nodeConnector.connectionType][connectionType]].Spawn();
                        node.transform.position = (transform.position + nodeConnector.transform.position) / 2;
                        Connect(node.GetComponentsInChildren<NodeConnector>().First(n => n.connectionType == FlipConnection(connectionType)), out _);
                        nodeConnector.Connect(node.GetComponentsInChildren<NodeConnector>().First(n => n.connectionType == FlipConnection(nodeConnector.connectionType)), out _);
                        createdNewNodes = true;
                        Debug.Log("Created new node!");
                    }
                }
            }
            createdNewNodes = false;
        }
        void Update()
        {
            float multi = transform.lossyScale.Sum() / 3;
            LineRenderer.widthMultiplier = 0.05f * multi;
            if (isDragging)
            {
                return;
            }
            if (other == null)
            {
                DrawCubicBezier(Vector3.zero, Vector3.zero);
            }
            else
            {
                DrawCubicBezier(WorldPosition(transform.position), WorldPosition(other.transform.position));
            }
        }
        void DrawCubicBezier(Vector3 startPoint, Vector3 endPoint)
        {
            if (startPoint == null || endPoint == null)
                return;

            // Auto-generate control points
            Vector3 mid = (startPoint + endPoint) / 2f;
            Vector3 dir = (endPoint - startPoint).normalized;

            // Controls are offset upwards for a smooth arc
            float ch = curveHeight;
            if (Vector3.Distance(startPoint, endPoint) < curveHeight)
            {
                ch = Vector3.Distance(startPoint, endPoint);
            }
            if ((startPoint.x) > (endPoint.x))
                ch = -ch;
            Vector3 controlPoint1 = startPoint + Vector3.right * ch;
            Vector3 controlPoint2 = endPoint + Vector3.right * -ch;

            Vector3[] positions = new Vector3[segmentCount + 1];

            for (int i = 0; i <= segmentCount; i++)
            {
                float t = i / (float)segmentCount;
                positions[i] = GetCubicBezierPoint(t, startPoint, controlPoint1, controlPoint2, endPoint);
            }

            LineRenderer.positionCount = positions.Length;
            LineRenderer.SetPositions(positions);
        }

        Vector3 GetCubicBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            // Formula: B(t) = (1-t)^3 * p0 + 3(1-t)^2t * p1 + 3(1-t)t^2 * p2 + t^3 * p3
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;
            float uuu = uu * u;
            float ttt = tt * t;

            Vector3 p = uuu * p0; // (1-t)^3 * p0
            p += 3 * uu * t * p1; // 3(1-t)^2 * t * p1
            p += 3 * u * tt * p2; // 3(1-t) * t^2 * p2
            p += ttt * p3;        // t^3 * p3

            return p;
        }
    }
}