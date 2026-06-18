using AC.Node_Related_Scripts.ConnectionStuff;
using AC.Node_Related_Scripts.NodeRunning;
using AC.Node_Related_Scripts.NodeRunning.Instructions;
using AC.NodeScripts;
using IDK.AbilityHandling;
using IDK.GlobalReferencing;
using Landfall.TABS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using static AC.Node_Related_Scripts.NodeRunning.Instructions.NodeProgram;

namespace AC
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
        [Header("Baisic Linking")]
        public Unit rootUnit;

        public NodeProgram nodeProgram;
        public NodeContextManager contextManager;
        public ComponentCacheSystem cacheSystem;
        [Header("Node Scene")]
        public GlobalReference nodeSceneReference;
        public VirtualNodeScene m_nodeScene;
        public VirtualNodeScene NodeScene
        {
            get
            {
                if (m_nodeScene is null)
                    m_nodeScene = (VirtualNodeScene)nodeSceneReference.GetValue();
                return m_nodeScene;
            }
        }

        [Header("General Info")]
        public VirtualNode[] virtualNodes;
        public Dictionary<VirtualNode, INode> nodeFunctionInstances;
        public Dictionary<int, VirtualNode> nodeDatabase;

        [Header("Caching")]
        public List<ITriggerNode> triggerNodes = new List<ITriggerNode>();
        public Dictionary<VirtualNode, NodeEnv> nodeEnvCache = new Dictionary<VirtualNode, NodeEnv>();

        public async void Start()
        {
            
            rootUnit = transform.root.GetComponent<Unit>();
            if (!rootUnit)
                return;
            contextManager = new NodeContextManager(new List<NodeContext>());
            cacheSystem = gameObject.AddComponent<ComponentCacheSystem>();
            virtualNodes = NodeScene.savedObjects.Where(n => n is VirtualNode).Select(n => (VirtualNode)n).ToArray();
            nodeFunctionInstances = new Dictionary<VirtualNode, INode>(virtualNodes.Length);
            nodeDatabase = new Dictionary<int, VirtualNode>(virtualNodes.Length);
            for (int i = 0; i < virtualNodes.Length; i++)
            {
                nodeFunctionInstances.Add(virtualNodes[i], Activator.CreateInstance(AbilityCreator.nodeDatabase[virtualNodes[i].nodeBlueprint].nodeFunction) as INode);
                nodeDatabase.Add(virtualNodes[i].id, virtualNodes[i]);
            }
            
            GenerateContexts();
            CacheEverything();
            nodeProgram = new NodeProgram(contextManager);
            for (int i = 0; i < contextManager.AllContexts.Count; i++)
            {
                NodeContext nodeContext = contextManager.AllContexts[i];
                nodeProgram.nodeProccesses.Add(new NodeProgram.NodeCore()
                {
                    contextIndex = i,
                    nodeInstructions = GetNodeInstructions(nodeContext),
                    root = nodeContext.houses[0].owner,
                    runner = this
                });
            }
        }
        public IEnumerator RunCore(NodeCore instructions)
        {
            if (instructions.nodeInstructions.Length == 0)
            {
                DeveloperLogger.LogError("brother make an actual node system");
                yield break;
            }
            instructions.Next();
        }
        public NodeInstruction[] GetNodeInstructions(NodeContext nodeContext)
        {
            VirtualNode[] allNodesOrdered = nodeContext.houses.Select(n => n.owner).ToArray();
            NodeInstruction[] nodeInstructions = new NodeInstruction[allNodesOrdered.Length];
            for (int i = 0; i < allNodesOrdered.Length; i++)
            {
                nodeInstructions[i] = new NodeInstruction()
                {
                    node = allNodesOrdered[i],
                    nodeFunction = nodeFunctionInstances[allNodesOrdered[i]] as INode,
                };
            }
            return nodeInstructions;
        }
        public void GenerateContexts()
        {
            foreach (VirtualNode virtualNode in virtualNodes)
            {

                if (!(nodeFunctionInstances[virtualNode] is ITriggerNode trigger))
                    continue;
                NodeTree tree = new NodeTree(virtualNode, nodeDatabase);
                NodeContext nodeConext = new NodeContext(tree.virtualNodes.ToArray(), contextManager);
                contextManager.AddContext(nodeConext);
                NodeEnv nodeEnv = new NodeEnv(this, rootUnit, virtualNode, nodeConext.GetHouse(virtualNode), nodeConext);
                triggerNodes.Add(trigger);
                trigger.Execute(nodeEnv);
            }
        }
        public void CacheEverything()
        {
            foreach (VirtualNode virtualNode in virtualNodes)
            {
                NodeEnv nodeEnv = GetNodeEnviroment(virtualNode);
                nodeEnvCache.Add(virtualNode, nodeEnv);
                nodeFunctionInstances[virtualNode].Cache(nodeEnv);
            }
        }
        public NodeEnv GetNodeEnviroment(VirtualNode virtualNode)
        {
            NodeEnv result;
            if (nodeEnvCache.TryGetValue(virtualNode, out result))
                return result;
            NodeContext nodeContext = contextManager.GetContext(virtualNode);
            NodeContext.House house = nodeContext.GetHouse(virtualNode);
            ConnectionType[] connectionTypes = virtualNode.connectionTypes.ToArray();
            result = new NodeEnv(this, rootUnit, virtualNode, house , nodeContext, connectionTypes);
            return result;
        }
    }
}
