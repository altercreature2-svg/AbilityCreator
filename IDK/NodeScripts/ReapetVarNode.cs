using IDK.Node_Related_Scripts;
using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IDK.NodeScripts
{
    public class ReapetVarNode : IBehaviorNode
    {
        public override IEnumerator RunNode(SavedNode savedNode, Unit unit, List<Node.Connection> connections, string[] fields, NodeRunner nodeRunner)
        {
            Variable[] variables = connections.GetNode(NodeBlueprint.ConnectionType.ReciveVariable).GetValuePool(unit).GetValues<Variable>();
            for (int i = 0; i < variables[0].value; i++)
            {
                if (savedNode.fields[0].QuickParse() == 0)
                    yield return null;
                else if (savedNode.fields[0].QuickParse() < 0) { }
                else
                    yield return new WaitForSeconds(savedNode.fields[1].QuickParse());
                yield return savedNode.TriggerConnection(nodeRunner);
            }
            
            
        }
    }
}