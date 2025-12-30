using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IDK.NodeScripts
{
    public class SetFistNode : IBehaviorNode
    {
        public override IEnumerator RunNode(SavedNode savedNode, Unit unit, List<Node.Connection> connections, string[] fields, NodeRunner nodeRunner)
        {
            Unit[] units = connections.GetNode(NodeBlueprint.ConnectionType.ReciveUnit).GetValuePoolSmart(unit).GetValues<Unit>();
            Debug.Log("Units node:" + connections.GetNode(NodeBlueprint.ConnectionType.ReciveUnit));
            Debug.Log("Units Length: " + units.Length);
            foreach (var unitIndex in units)
            {
                unitIndex.holdingHandler.LetGoOfAll();
                unitIndex.WeaponHandler.UseHands();
            }
            yield return savedNode.TriggerConnection(nodeRunner);
        }
    }
}