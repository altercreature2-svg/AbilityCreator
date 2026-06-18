using AC.Node_Related_Scripts.NodeRunning.Instructions;
using System.Collections.Generic;
using UnityEngine;

namespace AC.Node_Related_Scripts.NodeRunning
{
    public class NodeContextManager
    {
        public NodeRegistery Registery { get; private set; }
        public List<NodeContext> RootContexts { get; private set; }
        public List<NodeContext> AllContexts { get; private set; }
        public Dictionary<VirtualNode, NodeContext> contextCache = new Dictionary<VirtualNode, NodeContext>();

        public NodeContextManager(List<NodeContext> nodeContexts)
        {
            Registery = new NodeRegistery();
            RootContexts = nodeContexts;
            AllContexts = nodeContexts;
        }
        public void AddContext(NodeContext context)
        {
            RootContexts.Add(context);
        }
        public NodeContext GetContext(VirtualNode node)
        {
            if (contextCache.TryGetValue(node, out NodeContext outContext))
                return outContext;

            foreach (NodeContext context in RootContexts)
            {
                if (SearchContext(context, node, out NodeContext context1))
                {
                    contextCache.Add(node, context1);
                    return context;
                }
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
            if ((nodeContext.children?.Length ?? 0) == 0)
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
