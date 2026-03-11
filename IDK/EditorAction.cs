using IDK.Node_Related_Scripts.connection_stuff;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace IDK
{
    public class StoredNode
    {
        public string key;
        public Vector3 position;
        public NodeConnector[] connections;
        public string[] fields;
        public StoredNode(Node node)
        {
            connections = node.Connections.Values.Select(n => n.other).ToArray();
            key = node.nodeBlueprint.key;
            position = node.transform.position;
            fields = node.GetComponentsInChildren<NodeField>().Select(n => n.Value).ToArray();
        } 
    }
    public abstract class EditorAction
    {
        public Node relatedNode;
        public StoredNode storedNode;
        public List<string> extraInfo;
        public abstract void Undo();
    }
    public class CreateNodeAction : EditorAction
    {
        public override void Undo()
        {
            relatedNode.Remove();
        }
        public CreateNodeAction(Node node)
        {
            this.relatedNode = node;
        }
    }
    public class DeleteNodeAction : EditorAction
    {
        public override void Undo()
        {
            Node node = Main.nodeDatabase[storedNode.key].Spawn();
            node.transform.position = storedNode.position;
            NodeConnector[] nodeConnectors = node.GetComponentsInChildren<NodeConnector>();
            for (int i = 0; i < nodeConnectors.Length; i++)
            {
                NodeConnector nodeConnector = storedNode.connections[i];
                if (!(nodeConnector && nodeConnectors[i].CanConnect(nodeConnector)))
                    return;
                nodeConnectors[i].Connect(nodeConnector, out _);
            }
            node.SetFields(storedNode.fields);
        }
        public DeleteNodeAction(Node node)
        {
            storedNode = new StoredNode(node);
        }
    }
}