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
        public override IEnumerator RunNode(SavedNode savedNode, Unit unit, List<Node.Connection> connections, string[] fields, NodeRunner nodeRunner)
        {
            SavedNode nodeToTrigger = savedNode.connections.GetNode(NodeBlueprint.ConnectionType.Trigger);
            Unit[] units = savedNode.connections.GetNode(NodeBlueprint.ConnectionType.ReciveUnit).GetValuePoolSmart(unit).GetValues<Unit>();
            for (int i = 0; i < units.Length; i++)
            {
                if (units[i].dead)
                    yield return nodeRunner.RunNode(nodeToTrigger);
            }
                
            
            
            
        }
    }
}