using Landfall.TABS;
using Landfall.TABS.GameMode;
using Landfall.TABS.GameState;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IDK.NodeScripts
{
    public class IsDead : IBehaviorNode
    {
        public override IEnumerator RunNode(LegacySavedNode savedNode, Unit unit, List<NodeComponent.LegacyConnection> connections, string[] fields, NodeRunner nodeRunner)
        {
            LegacySavedNode nodeToTrigger = savedNode.connections.GetNode(NodeBlueprint.ConnectionClass.Trigger);
            Unit[] units = savedNode.connections.GetNode(NodeBlueprint.ConnectionClass.ReciveUnit).GetValuePoolSmart(unit).GetValues<Unit>();
            for (int i = 0; i < units.Length; i++)
            {
                if (units[i].dead)
                    yield return nodeRunner.RunNode(nodeToTrigger);
            }
                
            
            
            
        }
    }
}