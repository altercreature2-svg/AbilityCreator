using IDK.Node_Related_Scripts.NodeRunning;
using IDK.Node_Related_Scripts.NodeRunning.Instructions;
using IDK.NodeScripts;
using Landfall.TABS;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace IDK
{
    public class NodeRunner : MonoBehaviour
    {
        // Notes of what i need to do:

        // preserving of values; but how?
        // Instead of each node having it's own value pool
        // i will implement a *heirachy* context system
        // every trigger node will have it's own context system

        // nodes can write their data in their context
        // value nodes start redircting the values to the context
        // but what if the value changes?
        // the value node can just overwrite it's value when it's requested

        // what about nodes like repeating?
        // when it starts, it creates a new child context
        // for every itteration it resets and triggers the child context,

        // remind me of the new data structure!
        // NodeRunner > context > context > ...
        // context > id
        // context > node housing > dictionary (connection type (room), connection value (furniture)) + extra room idk

        // how will this work practically?
        // trigger nodes > make parent context > has babies (nodes)
        // repeat nodes and stuff > make own context children of parent

        // please add helpers for getting connection types easily

        // behavior node code example (psuedo code):
        // object value = dosomething(env.house.GetNeighborFurniture(connectionType)) -> search in same context, if not present search for other context
        // env.house.AddFurnitureToRoom(connectionType, dootherthing(value))

        // BOI THATS TUFF

        public VirtualNodeScene nodeScene;
        public List<ITriggerNode> triggerNodes = new List<ITriggerNode>();
        public VirtualNode[] savedNodes;
        public Dictionary<VirtualNode, INode> nodeMemory;
        public Dictionary<int, VirtualNode> nodeDatabase;
        public Unit rootUnit;
        public NodeContextManager contextManager;
        public NodeProgram nodeProgram;
        public async void Begin()
        {
            contextManager = new NodeContextManager(new List<NodeContext>());

            rootUnit = transform.root.GetComponent<Unit>();
            if (!rootUnit)
                return;
            savedNodes = nodeScene.savedObjects.Where(n => n is VirtualNode).Select(n => (VirtualNode)n).ToArray();
            nodeMemory = new Dictionary<VirtualNode, INode>(savedNodes.Length);
            nodeDatabase = new Dictionary<int, VirtualNode>(savedNodes.Length);
            for (int i = 0; i < savedNodes.Length; i++)
            {
                nodeMemory.Add(savedNodes[i], Activator.CreateInstance(AbilityCreator.nodeDatabase[savedNodes[i].nodeBlueprint].nodeFunction) as INode);
                nodeDatabase.Add(savedNodes[i].id, savedNodes[i]);
            }
            foreach (VirtualNode savedNode in savedNodes)
            {
                if (AbilityCreator.nodeDatabase[savedNode.nodeBlueprint].nodeFunction?.BaseType == typeof(ITriggerNode))
                {
                    var tree = GetTree(savedNode);
                    NodeContext nodeConext = new NodeContext(tree.ToArray(), contextManager);
                    contextManager.AddContext(nodeConext);
                    NodeEnv nodeEnv = new NodeEnv(this, rootUnit, savedNode, nodeConext.GetHouse(savedNode), nodeConext);
                    var triggerNode = (ITriggerNode)nodeMemory[savedNode];
                    triggerNodes.Add(triggerNode);
                    triggerNode.Cache(nodeEnv);
                }
            }
            foreach (VirtualNode savedNode in savedNodes)
            {
                NodeContext context = contextManager.GetContext(savedNode);
                NodeEnv nodeEnv = new NodeEnv(this, rootUnit, savedNode, context.GetHouse(savedNode), context);
                nodeMemory[savedNode].Cache(nodeEnv);
            }

            nodeProgram = new NodeProgram(contextManager);
        }
        public List<VirtualNode> GetTree(VirtualNode root)
        {
            List<VirtualNode> virtualNodes = new List<VirtualNode>();
            Mine(root, virtualNodes);
            return virtualNodes;
        }
        public void Mine(VirtualNode current, List<VirtualNode> ls)
        {
            if (ls.Contains(current)) return;
            ls.Add(current);
            int[] connections = current.GetConnections(NodeBlueprint.ConnectionClass.Trigger);
            for (int i = 0; i < connections.Length; i++)
            {
                Mine(nodeDatabase[connections[i]], ls);
            }

        }
    }
}
