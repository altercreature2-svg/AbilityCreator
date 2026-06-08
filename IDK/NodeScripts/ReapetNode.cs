using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IDK.NodeScripts
{
    public class ReapetNode : IBehaviorNode
    {
        public override IEnumerator RunNode(LegacySavedNode savedNode, Unit unit, List<NodeComponent.LegacyConnection> connections, string[] fields, NodeRunner nodeRunner)
        {
            int times = (int)fields[0].QuickParse();
            float interval = fields[1].QuickParse();
            AbilityCreator.reapeter.AddTask(times, interval, () => nodeRunner.StartCoroutine(nodeRunner.TriggerConnection(savedNode)));
            yield break;   
        }
    }
}