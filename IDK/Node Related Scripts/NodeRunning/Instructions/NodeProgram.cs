using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.IL2CPP.CompilerServices;

namespace IDK.Node_Related_Scripts.NodeRunning.Instructions
{
    public class NodeProgram
    {
        public struct NodeInstructions
        {
            public int contextIndex;
            public NodeInstruction[] nodeInstructions;
        }
        public NodeContextManager contextManager;
        public List<NodeInstructions> nodeInstructions; 
        public NodeProgram(NodeContextManager contextManager)
        {
            nodeInstructions = new List<NodeInstructions>();
            this.contextManager = contextManager;
        }
    }
}
