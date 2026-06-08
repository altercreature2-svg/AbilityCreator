using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IDK.NodeScripts
{
    public class DoEveryNode : ITriggerNode
    {
        public override void EveryFrame(LegacySavedNode savedNode, Unit unit, List<NodeComponent.LegacyConnection> connections, string[] fields, NodeRunner nodeRunner)
        {
            
        }
        public override void StartFrame(LegacySavedNode savedNode, Unit unit, List<NodeComponent.LegacyConnection> connections, string[] fields, NodeRunner nodeRunner)
        {
            nodeRunner.StartCoroutine(IntervalLoop(nodeRunner, savedNode, fields[0].QuickParse()));
        }
        public IEnumerator IntervalLoop(NodeRunner nodeRunner,LegacySavedNode savedNode, float seconds)
        {

            yield return new WaitForSeconds(seconds);
            nodeRunner.StartCoroutine(nodeRunner.TriggerConnection(savedNode));
            nodeRunner.StartCoroutine(IntervalLoop(nodeRunner,savedNode, seconds));
        }

    }
    
}