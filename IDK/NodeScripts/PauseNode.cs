using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IDK.NodeScripts
{
    public class PauseNode : IBehaviorNode
    {
        public override IEnumerator RunNode(SavedNode savedNode, Unit unit, List<Node.Connection> connections, string[] fields, NodeRunner nodeRunner)
        {
            SavedNode nodeToTrigger = savedNode.connections.GetNode(NodeBlueprint.ConnectionType.Trigger);
            yield return new WaitForSeconds(savedNode.fields[0].QuickParse());
            yield return nodeRunner.RunNode(nodeToTrigger);
            yield return null;
            
        }
    }
}