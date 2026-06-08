using IDK.Node_Related_Scripts;
using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IDK.NodeScripts
{
    public class ChangeVariableBy : IBehaviorNode
    {
        public override IEnumerator RunNode(LegacySavedNode savedNode, Unit unit, List<NodeComponent.LegacyConnection> connections, string[] fields, NodeRunner nodeRunner)
        {
            
            Variable[] variables = connections.GetNode(NodeBlueprint.ConnectionClass.ReciveVariable).GetValuePoolSmart(unit).GetValues<Variable>();
            for (int i = 0; i < variables.Length; i++)
            {
                variables[i].value += fields[0].QuickParse();
            }
            yield return savedNode.TriggerConnection(nodeRunner);
            
        }
    }
}