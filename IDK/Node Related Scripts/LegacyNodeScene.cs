using UnityEngine;

namespace AC
{
    [System.Serializable]
    [System.Obsolete]
    public class LegacyNodeScene : MonoBehaviour
    {
        public LegacySavedNode[] everyNode;
        public string sceneName = "";
        public string sceneDescription = "";
        public string sceneImage = "";
        public int id;
        public bool isFinal;
        public float zoom;
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
        
    }

}