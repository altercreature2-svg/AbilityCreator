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
            DeveloperLogger.Log($"Starting to Save {nodescene.sceneName}");
            // Convert every SavedNode into an id 
            for (int i = 0; i < EveryNode.Length; i++)
            {
                EveryID = EveryID.AddItem(EveryNode[i].GetNodeInstanceID()).ToArray();
                int CurrentID = EveryNode[i].GetNodeInstanceID();
                // Add Blueprint
                savedNodeScene.blueprints.Add(CurrentID);
                savedNodeScene.blueprints2.Add(EveryNode[i].Blueprint.key);
                DeveloperLogger.Log($"Saved blueprint for {EveryNode[i].Blueprint.Name} ({CurrentID})");
                // Add Position
                savedNodeScene.Positions.Add(CurrentID);
                savedNodeScene.Positions2.Add(EveryNode[i].position);
                DeveloperLogger.Log($"Saved position for {EveryNode[i].Blueprint.Name} ({CurrentID})");
                // Add Connections
                if (EveryNode[i].connections != null)
                {
                    NodeConnections savedConnections = new NodeConnections()
                    {
                        connectionsTypes = new List<NodeBlueprint.ConnectionType>(),
                        otherIDs = new List<int>(),
                    };
                    DeveloperLogger.Log("Begining adding connections, connection count is" + EveryNode[i].connections.Count);
                    List<Node.Connection> fixedConnections = EveryNode[i].connections.ToList().Where(n => n.savedNode != null).ToList();
                    if (fixedConnections.Count != 0)
                    {
                        for (int connectionsIndex = 0; connectionsIndex < fixedConnections.Count; connectionsIndex++)
                        {
                            DeveloperLogger.Log($"Adding connection {fixedConnections[connectionsIndex].savedNode.Blueprint.Name}");
                            DeveloperLogger.Log($"With connectionType of {fixedConnections[connectionsIndex].connectionsType}");
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
                
                DeveloperLogger.Log($"Saved connections for {EveryNode[i].Blueprint.Name} ({CurrentID})");
                // Add Fields
                DeveloperLogger.Log("Commencing field saving");
                savedNodeScene.fields.Add(CurrentID);
                string fieldsCombined = "";
                for (int fieldIndex = 0; fieldIndex < EveryNode[i].fields.Count; fieldIndex++)
                {
                    fieldsCombined += EveryNode[i].fields[fieldIndex] + "###";
                }
                savedNodeScene.fields2.Add(fieldsCombined);
                DeveloperLogger.Log($"Saved fields for {EveryNode[i].Blueprint.Name} ({CurrentID})");


            }
            // Extra info
            savedNodeScene.id = nodescene.id;
            savedNodeScene.sceneDescription = nodescene.sceneDescription;
            savedNodeScene.sceneImage = nodescene.sceneImage;
            savedNodeScene.sceneName = nodescene.sceneName;
            savedNodeScene.zoom = nodescene.zoom;
            DeveloperLogger.Log($"Saved Extra Info");
            DeveloperLogger.Log($"Done saving {nodescene.sceneName}");
            DeveloperLogger.Log($"Done building nodescene! that took {System.DateTime.Now - dateTime}");
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
                DeveloperLogger.Log("starting with node:" + currentID);
                GameObject node = new GameObject("Saved Node");
                DeveloperLogger.Log("Node:" + node);
                Object.DontDestroyOnLoad(node);
                SavedNode savedNode = node.AddComponent<SavedNode>();
                SavedNodeSceneStorer.SavedNodes.Add(savedNode);
                //blueprint
                savedNode.blueprintName = blueprints2[i];
                DeveloperLogger.Log($"Converted blueprint for {blueprints2[i]}");
                //position
                savedNode.position = Positions2[i];
                DeveloperLogger.Log($"Converted position for {blueprints2[i]}");
                // fields
                savedNode.fields = fields2[i].Split(separator: new string[] { "###" }, System.StringSplitOptions.RemoveEmptyEntries).ToList();

                DeveloperLogger.Log($"Converted fields for {blueprints2[i]}");
                conversion.Add(currentID, savedNode);
                DeveloperLogger.Log($"Done converting for node:" + blueprints2[i]);
            }
            DeveloperLogger.Log("Done with baisic stuff!");
            // connections
            for (int i = 0; i < Positions.Count; i++)
            {
                SavedNode savedNode = conversion[Positions[i]];
                DeveloperLogger.Log("Starting Connection converting for:" + blueprints2[i]);
                List<Node.Connection> connections = new List<Node.Connection>();
                DeveloperLogger.Log($"Converting connections for {blueprints2[i]}");
                DeveloperLogger.Log($"Safety Check! {connections2[i].connectionsTypes.Count == connections2[i].otherIDs.Count}");
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
                DeveloperLogger.Log($"Converted connections for {blueprints2[i]}");
            }
            scene.everyNode = conversion.Values.ToArray();
            scene.id = id;
            scene.sceneDescription = sceneDescription;
            scene.sceneImage = sceneImage;
            scene.sceneName = sceneName;
            scene.zoom = zoom;
            DeveloperLogger.Log($"Converted extra info");
            DeveloperLogger.Log($"Done building nodescene! that took {System.DateTime.Now - dateTime}");
            return scene;

        }
    }
}