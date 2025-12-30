using UnityEngine;

namespace IDK
{
    [System.Serializable]
    public class NodeScene : MonoBehaviour
    {
        public SavedNode[] everyNode;
        public string sceneName = "";
        public string sceneDescription = "";
        public string sceneImage = "";
        public int id;
        public bool isFinal;
        public float zoom;
        public int NodeSceneIndex
        {
            get
            {
                try
                {
                    if (Main.nodeScenes.FindIndex(n => n == this) != -1)
                        return Main.nodeScenes.FindIndex(n => n == this);
                    throw new System.Exception();
                }
                catch (System.Exception)
                {
                    return Main.nodeScenes.Count;
                }

            }
        }
        public string Jsonify(Newtonsoft.Json.Formatting formatting = Newtonsoft.Json.Formatting.Indented)
        {
            SavedNodeScene savedNodeScene = SavedNodeScene.Instance(this);
            var settings = new Newtonsoft.Json.JsonSerializerSettings()
            {
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore,
            };
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(savedNodeScene, formatting, settings);
            return json;
        }
        public NodeScene CreateCopy()
        {
            GameObject nodeobj = new GameObject($"Node Scene ({sceneName})");
            var node = nodeobj.AddComponent<NodeScene>();
            node.everyNode = everyNode;
            node.id = id;
            node.sceneDescription = sceneDescription;
            node.sceneImage = sceneImage;
            node.sceneName = sceneName;
            return node;

        }
        void OnDestroy()
        {
            if (Main.nodeScenes.Contains(this))
                Main.nodeScenes.Remove(this);
            if (gameObject)
                Destroy(gameObject);
        }
        public void Awake()
        {
            if (gameObject)
                DontDestroyOnLoad(gameObject);
            
        }
        public void Start()
        {
            if (!Main.nodeScenes.Contains(this) && isFinal)
                Main.nodeScenes.Add(this);
        }
        
    }

}