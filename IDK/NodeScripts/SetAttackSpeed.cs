using IDK.Node_Related_Scripts;
using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IDK.NodeScripts
{
    public class SetAttackSpeed : IBehaviorNode
    {
        public override IEnumerator RunNode(LegacySavedNode savedNode, Unit unit, List<NodeComponent.LegacyConnection> connections, string[] fields, NodeRunner nodeRunner)
        {
            Unit[] units = connections.GetNode(NodeBlueprint.ConnectionClass.ReciveUnit).GetValuePoolSmart(unit).GetValues<Unit>();
            for (int i = 0; i < units.Length; i++)
            {

                SetSpeed(units[i], fields[0].QuickParse(), fields[1]);
            }
            yield return savedNode.TriggerConnection(nodeRunner);
        }
        private void SetSpeed(Unit unit, float value, string field)
        {
            Weapon[] weapons = unit.GetComponentsInChildren<Weapon>();
            for (int i = 0; i < weapons.Length; i++)
            {
                if (field == "Set")
                    weapons[i].internalCooldown = value;
                if (field == "Add")
                    weapons[i].internalCooldown += value;
                if (field == "Multiply")
                    weapons[i].internalCooldown *= value;
                if (field == "Set (%)")
                    weapons[i].internalCooldown = (weapons[i].internalCooldown / 100) * value;
                if (field == "Add (%)")
                    weapons[i].internalCooldown += (weapons[i].internalCooldown / 100) * value;
            }
        }
    }
}