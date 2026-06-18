using AC.Node_Related_Scripts.connection_stuff;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AC
{
    public class StoredNode
    {
        public string key;
        public Vector3 position;
        public NodeConnector[] connections;
        public string[] fields;
        public StoredNode(NodeComponent node)
        {
            connections = node.Connections.Values.Select(n => n.connected).ToArray();
            key = node.nodeBlueprint.nodeKey;
            position = node.transform.position;
            fields = node.GetComponentsInChildren<NodeField>().Select(n => n.Value).ToArray();
        } 
    }
    public abstract class EditorAction
    {
        public NodeComponent relatedNode;
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
        public CreateNodeAction(NodeComponent node)
        {
            this.relatedNode = node;
        }
    }
    public class DeleteNodeAction : EditorAction
    {
        public override void Undo()
        {
            NodeComponent node = AbilityCreator.nodeDatabase[storedNode.key].Spawn();
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
        public DeleteNodeAction(NodeComponent node)
        {
            storedNode = new StoredNode(node);
        }
    }
}