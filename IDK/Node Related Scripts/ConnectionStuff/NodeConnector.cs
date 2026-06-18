using AC.Node_Related_Scripts.ConnectionStuff;
using Newtonsoft.Json.Linq;
using ProGrids;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AC.Node_Related_Scripts.connection_stuff
{

    public class NodeConnector : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IEventSystemHandler
    {
        [Header("Curve")]
        public float curveHeight = .5f;
        public Vector3 vec = Vector3.left;
        public int segmentCount = 32;
        [Header("Control")]
        public bool isDragging;
        private Transform m_copy;
        [Header("Info")]
        public NodeComponent papa;
        public int index;
        public NodeConnector connected;
        public LineRenderer m_lineRenderer;
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

        public static Dictionary<NodeBlueprint.ConnectionClass, Dictionary<NodeBlueprint.ConnectionClass, string>> convertTable = new Dictionary<NodeBlueprint.ConnectionClass, Dictionary<NodeBlueprint.ConnectionClass, string>>()
        {
            {NodeBlueprint.ConnectionClass.GiveUnit, new Dictionary<NodeBlueprint.ConnectionClass, string>
                {
                    {NodeBlueprint.ConnectionClass.ReciveAnything, "CONVERTUNIT" },
                    {NodeBlueprint.ConnectionClass.ReciveGameObject, "GAMEOBJ" },
                }
            },
            {NodeBlueprint.ConnectionClass.GiveGameObject, new Dictionary<NodeBlueprint.ConnectionClass, string>
                {
                    {NodeBlueprint.ConnectionClass.ReciveAnything, "CONVERTOBJ" },
                    {NodeBlueprint.ConnectionClass.ReciveUnit, "GETUNIT" },
                    {NodeBlueprint.ConnectionClass.ReciveComponent, "GETCOMP" },
                }
            },
            {NodeBlueprint.ConnectionClass.GiveComponent, new Dictionary<NodeBlueprint.ConnectionClass, string>
                {
                    {NodeBlueprint.ConnectionClass.ReciveAnything, "CONVERTCOMP" },
                }
            },
            {NodeBlueprint.ConnectionClass.GiveObjectVariable, new Dictionary<NodeBlueprint.ConnectionClass, string>
                {
                    {NodeBlueprint.ConnectionClass.ReciveAnything, "OBJECTVARIABLEVALUE" },
                }
            },
            {NodeBlueprint.ConnectionClass.GiveAnything, new Dictionary<NodeBlueprint.ConnectionClass, string>
                {
                    {NodeBlueprint.ConnectionClass.ReciveUnit, "CONVERTUNIT2" },
                    {NodeBlueprint.ConnectionClass.ReciveGameObject, "CONVERTOBJ2" },
                    {NodeBlueprint.ConnectionClass.ReciveComponent, "CONVERTCOMP2" },
                }
            },
        };
        public static NodeBlueprint.ConnectionClass FlipConnection(NodeBlueprint.ConnectionClass connectionType)
        {
            int value = (int)connectionType;
            NodeBlueprint.ConnectionClass result = (NodeBlueprint.ConnectionClass)(IsReciver(connectionType) ? (value << 1) : (value >> 1));
            return result;
        }
        public static bool IsReciver(NodeBlueprint.ConnectionClass connectionType)
        {
            bool result = (NodeBlueprint.ConnectionClass.AllRecivers & connectionType) != 0;
            return result;
        }

        public ConnectionType connectionType;
        void Start()
        {
            papa = transform.GetComponentInParent<NodeComponent>();
            NodeConnector[] neighbours = transform.root.GetComponentsInChildren<NodeConnector>().Where(n => n.connectionType.connectionClass == connectionType.connectionClass).ToArray();
            index = Array.FindIndex(neighbours, n => n == this);
        }
        void IDragHandler.OnDrag(PointerEventData eventData)
        {

            Vector3 mousePositon = Input.mousePosition;
            mousePositon.z = 0;
            if (eventData.button != PointerEventData.InputButton.Left)
                return;
            DrawCubicBezier(WorldPosition(transform.position), WorldPosition(mousePositon));

            //GetComponent<RectTransform>().anchoredPosition = (Vector2)Camera.current.ScreenToWorldPoint(mousePositon, Camera.MonoOrStereoscopicEye.Mono);
            if (connected != null)
                connected.connected = null;

        }
        void IBeginDragHandler.OnBeginDrag(UnityEngine.EventSystems.PointerEventData eventData)
        {
            if ((eventData.button & PointerEventData.InputButton.Left) != PointerEventData.InputButton.Left)
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
                if (connected != null)
                {
                    connected.connected = null;
                    connected = null;
                }
            }

            UpdateConnectionType();
        }
        public void RemoveAllConnections()
        {
            if (connected != null)
            {
                connected.connected = null;
                connected = null;
            }
        }
        public static bool CanConnect(NodeBlueprint.ConnectionClass connectionType, NodeBlueprint.ConnectionClass other)
        {
            return connectionType == FlipConnection(other);
        }
        public bool CanConnect(NodeConnector other)
        {
            return connectionType.connectionClass == FlipConnection(other.connectionType.connectionClass);
        }
        public void Connect(NodeConnector nodeConnector, out bool createdNewNodes)
        {
            bool canConnect = CanConnect(nodeConnector);
            if (canConnect)
            {
                connected = nodeConnector;
                nodeConnector.connected = this;
                createdNewNodes = false;
            }
            else
            {
                if (convertTable.ContainsKey(connectionType.connectionClass))
                {
                    if (convertTable[connectionType.connectionClass].ContainsKey(nodeConnector.connectionType.connectionClass))
                    {
                        NodeComponent node = AbilityCreator.nodeDatabase[convertTable[connectionType.connectionClass][nodeConnector.connectionType.connectionClass]].Spawn();
                        node.transform.position = (transform.position + nodeConnector.transform.position) / 2;
                        Connect(node.GetComponentsInChildren<NodeConnector>().First(n => n.connectionType.connectionClass == FlipConnection(connectionType.connectionClass)), out _);
                        nodeConnector.Connect(node.GetComponentsInChildren<NodeConnector>().First(n => n.connectionType.connectionClass == FlipConnection(nodeConnector.connectionType.connectionClass)), out _);
                        createdNewNodes = true;
                        Debug.Log("Created new node!");
                    }
                }
                else if (convertTable.ContainsKey(nodeConnector.connectionType.connectionClass))
                {
                    if (convertTable[nodeConnector.connectionType.connectionClass].ContainsKey(connectionType.connectionClass))
                    {
                        NodeComponent node = AbilityCreator.nodeDatabase[convertTable[nodeConnector.connectionType.connectionClass][connectionType.connectionClass]].Spawn();
                        node.transform.position = (transform.position + nodeConnector.transform.position) / 2;
                        Connect(node.GetComponentsInChildren<NodeConnector>().First(n => n.connectionType.connectionClass == FlipConnection(connectionType.connectionClass)), out _);
                        nodeConnector.Connect(node.GetComponentsInChildren<NodeConnector>().First(n => n.connectionType.connectionClass == FlipConnection(nodeConnector.connectionType.connectionClass)), out _);
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
            if (connected == null)
            {
                DrawCubicBezier(Vector3.zero, Vector3.zero);
            }
            else
            {
                DrawCubicBezier(WorldPosition(transform.position), WorldPosition(connected.transform.position));
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
        void UpdateConnectionType()
        {
            if (connectionType.first == null)
                connectionType.first = new ConnectionType.VirtualPort();
            if (connectionType.second == null && connected)
                connectionType.second = new ConnectionType.VirtualPort();
            else if (!connected)
                connectionType.second = null;
            if (connectionType.first != null)
            {
                connectionType.first.connectedNode = papa.GetNodeInstanceID();
                connectionType.first.portIndex = index;
            }
            if (connectionType.second != null)
            {
                connectionType.second.connectedNode = connected.papa.GetNodeInstanceID();
                connectionType.second.portIndex = connected.index;
            }
        }
    }
}