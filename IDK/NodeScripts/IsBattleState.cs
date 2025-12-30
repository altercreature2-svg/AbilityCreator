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
        public override IEnumerator RunNode(SavedNode savedNode, Unit unit, List<Node.Connection> connections, string[] fields, NodeRunner nodeRunner)
        {
            SavedNode nodeToTrigger = savedNode.connections.GetNode(NodeBlueprint.ConnectionType.Trigger);
            var service = ServiceLocator.GetService<GameStateManager>();
            if (service.GameState == GameState.BattleState)
            {
                yield return nodeRunner.RunNode(nodeToTrigger);
            }
            yield return null;
            
        }
    }
}