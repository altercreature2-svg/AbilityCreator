using Landfall.TABS;
using Landfall.TABS.GameMode;
using Landfall.TABS.GameState;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IDK.NodeScripts
{
    public class IsBattleState : IBehaviorNode
    {
        public override IEnumerator RunNode(LegacySavedNode savedNode, Unit unit, List<NodeComponent.LegacyConnection> connections, string[] fields, NodeRunner nodeRunner)
        {
            LegacySavedNode nodeToTrigger = savedNode.connections.GetNode(NodeBlueprint.ConnectionClass.Trigger);
            var service = ServiceLocator.GetService<GameStateManager>();
            if (service.GameState == GameState.BattleState)
            {
                yield return nodeRunner.RunNode(nodeToTrigger);
            }
            yield return null;
            
        }
    }
}