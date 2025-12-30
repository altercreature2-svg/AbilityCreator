using HarmonyLib;
using IDK.Node_Related_Scripts.connection_stuff;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace IDK
{
    [System.Serializable]
    public class SavedNodeScene
    {

        public List<int> connections = new List<int>();
        public List<NodeConnections> connections2 = new List<NodeConnections>();
        public List<int> Positions = new List<int>();
        public List<Vector3> Positions2 = new List<Vector3>();
        public List<int> fields = new List<int>();
        public List<string> fields2 = new List<string>();

        public List<int> blueprints = new List<int>();
        public List<string> blueprints2 = new List<string>();
        public float zoom;
        public string sceneName = "";
        public string sceneDescription = "";
        public string sceneImage = "";
        public int id = 0;
        public static SavedNodeScene Instance(NodeScene nodescene)
        {
            System.DateTime dateTime = System.DateTime.Now;
            SavedNodeScene savedNodeScene = new SavedNodeScene();
            SavedNodeSceneStorer.savedNodeScenes.Add(savedNodeScene);
            SavedNode[] EveryNode = nodescene.everyNode;
            int[] EveryID = new int[0];
            Debug.Log($"Starting to Save {nodescene.sceneName}");
            // Convert every SavedNode into an id 
            for (int i = 0; i < EveryNode.Length; i++)
            {
                EveryID = EveryID.AddItem(EveryNode[i].GetNodeInstanceID()).ToArray();
                int CurrentID = EveryNode[i].GetNodeInstanceID();
                // Add Blueprint
                savedNodeScene.blueprints.Add(CurrentID);
                savedNodeScene.blueprints2.Add(EveryNode[i].blueprint.key);
                Debug.Log($"Saved blueprint for {EveryNode[i].blueprint.Name} ({CurrentID})");
                // Add Position
                savedNodeScene.Positions.Add(CurrentID);
                savedNodeScene.Positions2.Add(EveryNode[i].position);
                Debug.Log($"Saved position for {EveryNode[i].blueprint.Name} ({CurrentID})");
                // Add Connections
                if (EveryNode[i].connections != null)
                {
                    NodeConnections savedConnections = new NodeConnections()
                    {
                        connectionsTypes = new List<NodeBlueprint.ConnectionType>(),
                        otherIDs = new List<int>(),
                    };
                    Debug.Log("Begining adding connections, connection count is" + EveryNode[i].connections.Count);
                    List<Node.Connection> fixedConnections = EveryNode[i].connections.ToList().Where(n => n.savedNode != null).ToList();
                    if (fixedConnections.Count != 0)
                    {
                        for (int connectionsIndex = 0; connectionsIndex < fixedConnections.Count; connectionsIndex++)
                        {
                            Debug.Log($"Adding connection {fixedConnections[connectionsIndex].savedNode.blueprint.Name}");
                            Debug.Log($"With connectionType of {fixedConnections[connectionsIndex].connectionsType}");
                            if (fixedConnections[connectionsIndex].savedNode == null)
                            {
                                savedConnections.connectionsTypes.Add(fixedConnections[connectionsIndex].connectionsType);
                                savedConnections.otherIDs.Add(999);
                            }
                            else
                            {

                                savedConnections.connectionsTypes.Add(fixedConnections[connectionsIndex].connectionsType);
                                savedConnections.otherIDs.Add(fixedConnections[connectionsIndex].savedNode.GetNodeInstanceID());
                            }
                        }
                    }
                    savedNodeScene.connections.Add(CurrentID);
                    savedNodeScene.connections2.Add(savedConnections);
                }
                
                Debug.Log($"Saved connections for {EveryNode[i].blueprint.Name} ({CurrentID})");
                // Add Fields
                Debug.Log("Commencing field saving");
                savedNodeScene.fields.Add(CurrentID);
                string fieldsCombined = "";
                for (int fieldIndex = 0; fieldIndex < EveryNode[i].fields.Count; fieldIndex++)
                {
                    fieldsCombined += EveryNode[i].fields[fieldIndex] + "###";
                }
                savedNodeScene.fields2.Add(fieldsCombined);
                Debug.Log($"Saved fields for {EveryNode[i].blueprint.Name} ({CurrentID})");


            }
            // Extra info
            savedNodeScene.id = nodescene.id;
            savedNodeScene.sceneDescription = nodescene.sceneDescription;
            savedNodeScene.sceneImage = nodescene.sceneImage;
            savedNodeScene.sceneName = nodescene.sceneName;
            savedNodeScene.zoom = nodescene.zoom;
            Debug.Log($"Saved Extra Info");
            Debug.Log($"Done saving {nodescene.sceneName}");
            Debug.Log($"Done building nodescene! that took {System.DateTime.Now - dateTime}");
            return savedNodeScene;

        }
        public NodeScene SavedNodeSceneToNodeScene()
        {
            System.DateTime dateTime = System.DateTime.Now;
            GameObject nodeobj = new GameObject($"Node Scene ({sceneName})");
            var scene = nodeobj.AddComponent<NodeScene>();

            Dictionary<int, SavedNode> conversion = new Dictionary<int, SavedNode>();
            for (int i = 0; i < Positions.Count; i++)
            {
                int currentID = Positions[i];
                Debug.Log("starting with node:" + currentID);
                GameObject node = new GameObject("Saved Node");
                Debug.Log("Node:" + node);
                Object.DontDestroyOnLoad(node);
                SavedNode savedNode = node.AddComponent<SavedNode>();
                SavedNodeSceneStorer.SavedNodes.Add(savedNode);
                //blueprint
                savedNode.blueprint = Main.nodeDatabase[blueprints2[i]];
                Debug.Log($"Converted blueprint for {savedNode.blueprint.Name}");
                //position
                savedNode.position = Positions2[i];
                Debug.Log($"Converted position for {savedNode.blueprint.Name}");
                // fields
                savedNode.fields = fields2[i].Split(separator: new string[] { "###" }, System.StringSplitOptions.RemoveEmptyEntries).ToList();

                Debug.Log($"Converted fields for {savedNode.blueprint.Name}");
                conversion.Add(currentID, savedNode);
                Debug.Log($"Done converting for node:" + savedNode.blueprint.Name);
            }
            Debug.Log("Done with baisic stuff!");
            // connections
            for (int i = 0; i < Positions.Count; i++)
            {
                SavedNode savedNode = conversion[Positions[i]];
                Debug.Log("Starting Connection converting for:" + savedNode?.blueprint?.Name);
                List<Node.Connection> connections = new List<Node.Connection>();
                Debug.Log($"Converting connections for {savedNode?.blueprint?.Name}");
                Debug.Log($"Safety Check! {connections2[i].connectionsTypes.Count == connections2[i].otherIDs.Count}");
                for (int i2 = 0; i2 < connections2[i].connectionsTypes.Count; i2++)
                {
                    if (connections2[i].otherIDs[i2] == 999)
                        continue;
                    connections.Add(new Node.Connection()
                    {
                        connectionsType = connections2[i].connectionsTypes[i2],
                        savedNode = conversion[connections2[i].otherIDs[i2]],
                    }); ;
                }

                savedNode.connections = connections;
                Debug.Log($"Converted connections for {savedNode.blueprint.Name}");
            }
            scene.everyNode = conversion.Values.ToArray();
            scene.id = id;
            scene.sceneDescription = sceneDescription;
            scene.sceneImage = sceneImage;
            scene.sceneName = sceneName;
            scene.zoom = zoom;
            Debug.Log($"Converted extra info");
            Debug.Log($"Done building nodescene! that took {System.DateTime.Now - dateTime}");
            return scene;

        }
    }
}