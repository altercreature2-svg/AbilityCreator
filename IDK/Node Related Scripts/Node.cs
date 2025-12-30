
using HarmonyLib;
using IDK.Node_Related_Scripts.connection_stuff;
using Mono.Security.X509.Extensions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


namespace IDK
{
    public class Node : MonoBehaviour
    {
        public class Connection
        {
            public NodeBlueprint.ConnectionType connectionsType;
            public SavedNode savedNode;
            public override string ToString()
            {
                return $"{connectionsType} > {savedNode?.blueprint?.Name}";
            }
        }
        public LineRenderer line;
        public GameObject background;
        public bool GetValueAtRuntime;
        public NodeBlueprint blueprint;
        public Button button;
        public SavedNode corispondingNode;
       
        public Dictionary<NodeBlueprint.ConnectionType, NodeConnector> Connections
        {
            get
            {
                return GetComponentsInChildren<NodeConnector>().ToDictionary(n => n.connectionType);
            }
        }


        private void Awake()
        {
            button = GetComponentInChildren<Button>();
            button.onClick.AddListener(OnPointerClick);
            background = transform.Find("Background").gameObject;
            if (line == null)
            {
                line = gameObject.AddComponent<LineRenderer>();
                line.material = new Material(Shader.Find("Sprites/Default"));
                line.widthMultiplier *= 3;
                line.positionCount = 2;
            }
            if (FindObjectOfType<NodeManager>())
            {
                Node[] nodes = FindObjectsOfType<Node>()?.Where(n => n != this).ToArray();
                if (nodes.Length != 0)
                {
                    transform.localScale = nodes[0].transform.localScale;
                }
                else
                {
                    transform.localScale = Vector3.one;
                }
            }
            FindObjectOfType<NodeManager>()?.nodes.Add(this);   
        }
        private void Update()
        {


            if (line == null)
            {
                line = gameObject.AddComponent<LineRenderer>();
                line.material = new Material(Shader.Find("Sprites/Default"));
                line.widthMultiplier *= 3;

            }
            /*
            line.positionCount = connectionsOrdered.Count * 2;
            int index = 0;
            for (int i = 0; i < connectionsOrdered.Count; i++)
            {
                line.SetPosition(
                    index,
                    transform.position
                );
                line.SetPosition(
                    index + 1,
                    connectionsOrdered[i].transform.position
                );
                index += 2;
            }
            if (Input.GetKey(KeyCode.LeftControl))
            {

                float maxDistance = line.widthMultiplier;
                Vector2 mouseworldpos = Camera.current.ScreenToWorldPoint(Input.mousePosition);
                for (int i = 0; i < line.positionCount - 1; i++)
                {
                    Vector2 p1 = line.GetPosition(i);
                    Vector2 p2 = line.GetPosition(i + 1);
                    float distance = DistanceFromPointToLineSegment(mouseworldpos, p1, p2);
                    if (distance < maxDistance)
                    {

                        if (Input.GetMouseButton(1))
                        {
                            Node connectionToRemove = connectionsOrdered[(int)Math.Round((i / 2) - 0.1)];
                            Connections.Remove(connectionToRemove);
                            connectionsOrdered.Remove(connectionToRemove);
                        }

                    }
                }

            }*/




        }

        private float DistanceFromPointToLineSegment(Vector2 point, Vector2 a, Vector2 b)
        {
            Vector2 ap = point - a;
            Vector2 ab = b - a;
            float MagnitudeAB = ab.sqrMagnitude;
            float abDotAp = Vector2.Dot(ap, ab);
            float t = Mathf.Clamp01(abDotAp / MagnitudeAB);
            Vector2 closest = a + ab * t;
            return Vector2.Distance(point, closest);
        }

        public int GetNodeInstanceID()
        {
            var objects = FindObjectsOfType<Node>();
            for (int i = 0; i < objects.Length; i++)
            {
                if (objects[i] == this)
                {
                    return i + 1;
                }
            }
            return 0;
        }
        public void OnPointerClick()
        {
            if (FindObjectOfType<NodeManager>() != null)
            {
                var manager = FindObjectOfType<NodeManager>();
                if (!Input.GetKey(KeyCode.LeftShift))
                {
                    if (!manager.SelectedNodes.Contains(this))
                    {
                        
                        if (manager.SelectedNodes != null && manager.SelectedNodes.Length != 0)
                        {
                            for (int i = 0; i < manager.SelectedNodes.Length; i++)
                            {
                                manager.SelectedNodes[i].RemoveOutline();
                            }
                        }
                        AddOutline();
                    }
                    else
                    {
                        RemoveOutline();               
                    }
                    transform.SetAsLastSibling();
                    transform.SetSiblingIndex(transform.GetSiblingIndex() - 1);
                }
                else
                {
                    if (manager.SelectedNodes.Contains(this))
                    {
                        RemoveOutline();
                    }
                    else
                    {
                        AddOutline();
                       
                    }
                }
            }
        }
        public void AddOutline()
        {
            if (!background.GetComponent<UnityEngine.UI.Outline>())
            {
                background.AddComponent<UnityEngine.UI.Outline>().effectColor = Color.white;
                background.GetComponent<UnityEngine.UI.Outline>().effectDistance = Vector2.one * 2.5f;
            }
        }
        public void RemoveOutline()
        {
            if (background.GetComponent<UnityEngine.UI.Outline>())
            {
                Destroy(background.GetComponent<UnityEngine.UI.Outline>());
            }
        }
        private void OnDestroy()
        {
            FindObjectOfType<NodeManager>().nodes.Remove(this);
            Destroy(line);
        }
    }
}