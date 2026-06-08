using UnityEngine;

namespace IDK
{
    [System.Serializable]
    public class LegacyNodeScene : MonoBehaviour
    {
        public LegacySavedNode[] everyNode;
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
                    if (AbilityCreator.nodeScenes.FindIndex(n => n == this) != -1)
                        return AbilityCreator.nodeScenes.FindIndex(n => n == this);
                    throw new System.Exception();
                }
                catch (System.Exception)
                {
                    return AbilityCreator.nodeScenes.Count;
                }

            }
        }
        public string Jsonify(Newtonsoft.Json.Formatting formatting = Newtonsoft.Json.Formatting.Indented)
        {
            LegacySavedNodeScene savedNodeScene = LegacySavedNodeScene.Instance(this);
            var settings = new Newtonsoft.Json.JsonSerializerSettings()
            {
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore,
            };
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(savedNodeScene, formatting, settings);
            return json;
        }
        public LegacyNodeScene CreateCopy()
        {
            GameObject nodeobj = new GameObject($"Node Scene ({sceneName})");
            var node = nodeobj.AddComponent<LegacyNodeScene>();
            node.everyNode = everyNode;
            node.id = id;
            node.sceneDescription = sceneDescription;
            node.sceneImage = sceneImage;
            node.sceneName = sceneName;
            return node;

        }
        void OnDestroy()
        {
            if (AbilityCreator.nodeScenes.Contains(this))
                AbilityCreator.nodeScenes.Remove(this);
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
            if (!AbilityCreator.nodeScenes.Contains(this) && isFinal)
                AbilityCreator.nodeScenes.Add(this);
        }
        
    }

}