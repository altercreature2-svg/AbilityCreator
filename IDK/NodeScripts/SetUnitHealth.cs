using IDK.Node_Related_Scripts;
using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IDK.NodeScripts
{
    public class SetUnitHealth : IBehaviorNode
    {
        public override IEnumerator RunNode(SavedNode savedNode, Unit unit, List<Node.Connection> connections, string[] fields, NodeRunner nodeRunner)
        {
            Unit[] units = connections.GetNode(NodeBlueprint.ConnectionType.ReciveUnit).GetValuePoolSmart(unit).GetValues<Unit>();
            for (int i = 0; i < units.Length; i++)
            {
                if (fields[1] == "Set")
                    units[i].data.health = fields[0].QuickParse();
                if (fields[1] == "Add")
                    units[i].data.health += fields[0].QuickParse();
                if (fields[1] == "Multiply")
                    units[i].data.health *= fields[0].QuickParse();
                if (fields[1] == "Set (%)")
                    units[i].data.health = (unit.data.maxHealth * (fields[0].QuickParse()))/100;
                if (fields[1] == "Add (%)")
                    units[i].data.health += (unit.data.maxHealth * (fields[0].QuickParse())) / 100;
            }
            yield return savedNode.TriggerConnection(nodeRunner);
        }
    }
}