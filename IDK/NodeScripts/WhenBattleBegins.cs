using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IDK.NodeScripts
{
    public class WhenBattleBegins : ITriggerNode
    {
        public override void StartFrame(SavedNode savedNode, Unit unit, List<Node.Connection> connections, string[] fields, NodeRunner nodeRunner)
        {
            nodeRunner.StartCoroutine(WaitABit(savedNode, unit, connections, fields, nodeRunner));
        }
        public IEnumerator WaitABit(SavedNode savedNode, Unit unit, List<Node.Connection> connections, string[] fields, NodeRunner nodeRunner)
        {
            yield return new WaitUntil(() => unit.data.targetData);
            yield return savedNode.TriggerConnection(nodeRunner);
        }
        public override void EveryFrame(SavedNode savedNode, Unit unit, List<Node.Connection> connections, string[] fields, NodeRunner nodeRunner)
        {
        }
        
    }
}