using IDK.Node_Related_Scripts;
using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IDK.NodeScripts
{
    public class LogAnything : IBehaviorNode
    {
        public override IEnumerator RunNode(LegacySavedNode savedNode, Unit unit, List<NodeComponent.LegacyConnection> connections, string[] fields, NodeRunner nodeRunner)
        {
            object[] everything = connections.GetNode(NodeBlueprint.ConnectionClass.ReciveAnything).GetValuePoolSmart(unit).GetValues<object>();
            for (int i = 0; i < everything.Length; i++)
            {
                DeveloperLogger.Log("(" + nodeRunner.nodeScene.sceneName + ")" +"Log:"+ everything[i], true);
            }
            yield return savedNode.TriggerConnection(nodeRunner);

        }
    }
}