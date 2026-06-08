using IDK.Node_Related_Scripts.NodeRunning.Instructions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace IDK.Node_Related_Scripts.NodeRunning
{
    public class NodeContextManager
    {
        public NodeRegistery Registery {get; private set;}
        public List<NodeContext> RootContexts { get; private set; }
        
        public NodeContextManager(List<NodeContext> nodeContexts) 
        {
            Registery = new NodeRegistery();
            RootContexts = nodeContexts;
        }
        public void AddContext(NodeContext context)
        {
            RootContexts.Add(context);
        }
        public NodeContext GetContext(VirtualNode node)
        {
            foreach (NodeContext context in RootContexts)
            {
                if (SearchContext(context, node, out NodeContext context1)) return context;
            }
            Debug.LogError("wtf??? no context for:" + node);
            return null;
        }
        private bool SearchContext(NodeContext nodeContext, VirtualNode nodeToSearch, out NodeContext context)
        {
            context = null;
            if (nodeContext.HasHouse(nodeToSearch))
            {
                context = nodeContext;
                return true;
            }
            if (nodeContext.children.Length == 0)
            {
                context = null;
                return false;
            }
            foreach (var childContext in nodeContext.children)
            {
                bool recursive = SearchContext(childContext, nodeToSearch, out NodeContext temp);
                context = temp;
                return recursive;
            }
            return false;   
        }
        
    }
}
