using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IDK.NodeScripts
{
    public class Deal_Damage : IBehaviorNode
    {
        public override IEnumerator RunNode(LegacySavedNode savedNode, Unit unit, List<NodeComponent.LegacyConnection> connections, string[] fields, NodeRunner nodeRunner)
        {
            
            Unit[] units = connections.GetNode(NodeBlueprint.ConnectionClass.ReciveUnit).GetValuePoolSmart(unit).GetValues<Unit>();
            foreach (var unitIndex in units)
            {
                if (fields[1] == "Normal")
                    unitIndex.data.healthHandler.TakeDamage(fields[0].QuickParse(), Vector3.zero);
                else
                    unitIndex.data.healthHandler.TakeDamage((unitIndex.data.maxHealth/100)*fields[0].QuickParse(), Vector3.zero);
            }
            yield return savedNode.TriggerConnection(nodeRunner);
            
        }
    }
}