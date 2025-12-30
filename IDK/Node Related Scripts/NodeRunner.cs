using IDK.NodeScripts;
using Landfall.TABS;
using Landfall.TABS.GameState;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IDK
{
    public class NodeRunner : MonoBehaviour
    {

        public NodeScene nodeScene;
        public List<ITriggerNode> triggerNodes = new List<ITriggerNode>();
        
        public void Begin()
        {
            nodeScene = nodeScene.CreateCopy();
            for (int i = 0; i < nodeScene.everyNode.Length; i++)
            {
                nodeScene.everyNode[i].valuePools = new Dictionary<Unit, ValuePool>();
            }
            if (transform.root.GetComponent<Unit>())
            {
                StartCoroutine(BeginInternal());
            }
        }
        public IEnumerator BeginInternal()
        {
            yield return null;
            yield return null;
            SavedNode[] savedNodes = nodeScene.everyNode;
            Unit unit = transform.root.GetComponent<Unit>();
            foreach (SavedNode savedNode in savedNodes)
            {
                savedNode.nodeRunners.Add(unit, this);
                if (savedNode.blueprint.nodeFunction != null)
                {
                    if (savedNode.blueprint.nodeFunction.BaseType == typeof(ITriggerNode))
                    {
                        var triggerNode = (ITriggerNode)savedNode.InstanceFunction;
                        triggerNode.StartFrame(savedNode, unit, savedNode.connections, savedNode.fields.ToArray(), this);
                        triggerNodes.Add(triggerNode);
                    }
                    if (savedNode.blueprint.nodeFunction == typeof(ReDie))
                    {
                        Unit[] units = savedNode.connections.GetNode(NodeBlueprint.ConnectionType.ReciveUnit).GetValuePoolSmart(unit).GetValues<Unit>();
                        for (int i = 0; i < units.Length; i++)
                        {
                        }
                    }
                }
            }
            
        }
        public IEnumerator TriggerConnection(SavedNode savedNode)
        {
            try
            {
                SavedNode node = savedNode.connections.GetNode(NodeBlueprint.ConnectionType.Trigger);
                StartCoroutine(RunNode(node));
            }
            catch (Exception e) { Debug.Log("(NodeRunner TriggerConnection) Something went wrong! exception:" + e); }
            yield return null;
        }
        public IEnumerator RunNode(SavedNode node)
        {
            yield return null;
            Debug.Log("Running node " + node?.blueprint?.Name + "...");
            if (node == null)
            {
                Debug.Log("nvm this node is null 😣");
                yield break;
            }
            try
            {
                GetValueOfConnections(node);
                var behaviorNode = (IBehaviorNode)node.InstanceFunction;
                StartCoroutine(behaviorNode.RunNode(node, transform.root.GetComponent<Unit>(), node.connections, node.fields.ToArray(), this));
            }
            catch (Exception e) { Debug.Log("(NodeRunner RunNode) Something went wrong! exception:" + e); }
            
        }
        public void GetValueOfConnections(SavedNode savedNode)
        {
            var nodes = savedNode.connections;
            foreach (var node in nodes)
            {
                try
                {
                    if (node?.savedNode?.blueprint?.nodeFunction == null)
                        continue;
                    if (node.savedNode.blueprint.nodeFunction.BaseType != typeof(IValueNode))
                        continue;
                    IValueNode valueNode = (IValueNode)node.savedNode.InstanceFunction;
                    valueNode.GetValuePool(node.savedNode, transform.root.GetComponent<Unit>(), node.savedNode.connections, node.savedNode.fields.ToArray());
                }
                catch (Exception e) { Debug.Log("(GetValueOfConnections) Something went wrong! exception:" + e); }
            }
        }
    }
}
