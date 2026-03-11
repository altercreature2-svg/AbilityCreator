using IDK.Node_Related_Scripts;
using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IDK.NodeScripts
{
    public class LogVariable : IBehaviorNode
    {
        public override IEnumerator RunNode(SavedNode savedNode, Unit unit, List<Node.Connection> connections, string[] fields, NodeRunner nodeRunner)
        {
            Variable[] variables = connections.GetNode(NodeBlueprint.ConnectionType.ReciveVariable).GetValuePoolSmart(unit).GetValues<Variable>();
            for (int i = 0; i < variables.Length; i++)
            {
                DeveloperLogger.Log("(" + nodeRunner.nodeScene.sceneName + ")" +"Log:"+ variables[i].value, true);
            }
            yield return savedNode.TriggerConnection(nodeRunner);

        }
    }
}