using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace IDK.NodeScripts
{
    public class DontWalkNode : IBehaviorNode
    {
        public override IEnumerator RunNode(LegacySavedNode savedNode, Unit unit, List<NodeComponent.LegacyConnection> connections, string[] fields, NodeRunner nodeRunner)
        {
            Unit[] units = connections.GetNode(NodeBlueprint.ConnectionClass.ReciveUnit).GetValuePoolSmart(unit).GetValues<Unit>();
            foreach (var unitIndex in units)
            {
                unitIndex.gameObject.AddComponent<UnitDontWalkFor>().time = fields[0].QuickParse();
                unitIndex.gameObject.GetComponent<UnitDontWalkFor>().Go();
            }
            yield return savedNode.TriggerConnection(nodeRunner);

        }
    }
}