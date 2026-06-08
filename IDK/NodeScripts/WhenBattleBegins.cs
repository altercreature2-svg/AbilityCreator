using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IDK.NodeScripts
{
    public class WhenBattleBegins : ITriggerNode
    {
        public override void StartFrame(LegacySavedNode savedNode, Unit unit, List<NodeComponent.LegacyConnection> connections, string[] fields, NodeRunner nodeRunner)
        {
            nodeRunner.StartCoroutine(WaitABit(savedNode, unit, connections, fields, nodeRunner));
        }
        public IEnumerator WaitABit(LegacySavedNode savedNode, Unit unit, List<NodeComponent.LegacyConnection> connections, string[] fields, NodeRunner nodeRunner)
        {
            yield return new WaitUntil(() => unit.data.targetData);
            yield return savedNode.TriggerConnection(nodeRunner);
        }
        public override void EveryFrame(LegacySavedNode savedNode, Unit unit, List<NodeComponent.LegacyConnection> connections, string[] fields, NodeRunner nodeRunner)
        {
        }
        
    }
}