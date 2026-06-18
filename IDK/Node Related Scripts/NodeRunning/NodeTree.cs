using System.Collections.Generic;
using static HoldingAnimation;

namespace AC.Node_Related_Scripts.NodeRunning
{
    public struct NodeTree
    {
        public List<VirtualNode> virtualNodes;
        public NodeTree(VirtualNode root, Dictionary<int, VirtualNode> nodeDatabase)
        {
            virtualNodes = new List<VirtualNode>();
            Mine(root, virtualNodes, nodeDatabase);
        }
        public void Mine(VirtualNode current, List<VirtualNode> ls, Dictionary<int, VirtualNode> nodeDatabase)
        {
            if (ls.Contains(current)) return;
            ls.Add(current);
                          
            int[] connections = current.GetConnections(NodeBlueprint.ConnectionClass.AllRecivers | NodeBlueprint.ConnectionClass.Trigger);
            for (int i = 0; i < connections.Length; i++)
            {
                var key = connections[i];
                Mine(nodeDatabase[key], ls, nodeDatabase);
            }
        }
    }
}
