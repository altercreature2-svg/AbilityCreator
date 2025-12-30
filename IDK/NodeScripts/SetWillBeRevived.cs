using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IDK.NodeScripts
{
    public class SetWillBeRevived : IBehaviorNode
    {
        public override IEnumerator RunNode(SavedNode savedNode, Unit unit, List<Node.Connection> connections, string[] fields, NodeRunner nodeRunner)
        {
            
            Unit[] units = connections.GetNode(NodeBlueprint.ConnectionType.ReciveUnit).GetValuePoolSmart(unit).GetValues<Unit>();
            foreach (var unitIndex in units)
            {
                unitIndex.data.healthHandler.willBeRewived = fields[0] == "Will be revived";
                
            }
            yield return savedNode.TriggerConnection(nodeRunner);
            
        }
    }
}