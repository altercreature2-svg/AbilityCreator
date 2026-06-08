using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IDK.NodeScripts
{
    public class PauseNode : IBehaviorNode
    {
        public override IEnumerator RunNode(LegacySavedNode savedNode, Unit unit, List<NodeComponent.LegacyConnection> connections, string[] fields, NodeRunner nodeRunner)
        {
            LegacySavedNode nodeToTrigger = savedNode.connections.GetNode(NodeBlueprint.ConnectionClass.Trigger);
            yield return new WaitForSeconds(savedNode.fields[0].QuickParse());
            yield return nodeRunner.RunNode(nodeToTrigger);
            yield return null;
            
        }
    }
}