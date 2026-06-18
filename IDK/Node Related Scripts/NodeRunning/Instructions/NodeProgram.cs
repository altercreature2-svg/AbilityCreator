using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.IL2CPP.CompilerServices;

namespace AC.Node_Related_Scripts.NodeRunning.Instructions
{
    public class NodeProgram
    {
        public class NodeCore
        {
            public int contextIndex;
            public VirtualNode root;
            public NodeRunner runner;
            public NodeInstruction[] nodeInstructions;
            private int _counter = 0;
            public void Next()
            {
                if (_counter >= nodeInstructions.Length)
                    return;
                NodeInstruction nodeInstruction = nodeInstructions[_counter];
                runner.StartCoroutine(nodeInstruction.Execute(runner.GetNodeEnviroment(nodeInstruction.node), this));
                _counter++;
            }
        }
        public NodeContextManager contextManager;
        public List<NodeCore> nodeProccesses; 
        public NodeProgram(NodeContextManager contextManager)
        {
            nodeProccesses = new List<NodeCore>();
            this.contextManager = contextManager;
        }
        
    }
}
