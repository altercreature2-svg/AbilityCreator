using IDK.Node_Related_Scripts;
using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IDK.NodeScripts
{
    public class ClearObjectVariable : IBehaviorNode
    {
        public override IEnumerator RunNode(LegacySavedNode savedNode, Unit unit, List<NodeComponent.LegacyConnection> connections, string[] fields, NodeRunner nodeRunner)
        {
            ObjectVariable[] variables = connections.GetNode(NodeBlueprint.ConnectionClass.ReciveObjectVariable).GetValuePoolSmart(unit).GetValues<ObjectVariable>();
            for (int i = 0; i < variables.Length; i++)
            {
                variables[i].value = new object[0];
            }
            yield return savedNode.TriggerConnection(nodeRunner);

        }
    }
}