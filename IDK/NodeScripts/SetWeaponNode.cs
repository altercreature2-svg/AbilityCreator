using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HoldingHandler;

namespace IDK.NodeScripts
{
    public class SetWeaponNode : IBehaviorNode
    {
        public override IEnumerator RunNode(LegacySavedNode savedNode, Unit unit, List<NodeComponent.LegacyConnection> connections, string[] fields, NodeRunner nodeRunner)
        {
            Unit[] units = connections.GetNode(NodeBlueprint.ConnectionClass.ReciveUnit).GetValuePoolSmart(unit).GetValues<Unit>();
            GameObject[] weapons = connections.GetNode(NodeBlueprint.ConnectionClass.ReciveGameObject).GetValuePoolSmart(unit).GetValues<GameObject>();
            foreach (var unitIndex in units)
            {
                foreach (var weapon in weapons)
                {
                    if (fields[0] == "Both")
                    {
                        
                        unitIndex.data.weaponHandler.rightWeapon?.GetComponent<Holdable>()?.Dissarm();
                        unitIndex.data.weaponHandler.leftWeapon?.GetComponent<Holdable>()?.Dissarm();
                        unitIndex.unitBlueprint.SetWeapon(unitIndex, unitIndex.Team, weapon, weapon.GetComponent<WeaponItem>().PropData ?? null, HandType.Right, Quaternion.identity, new List<GameObject>());
                        unitIndex.holdingHandler.leftHandActivity = HandActivity.HoldingRightObject;
                    }
                    if (fields[0] == "Right")
                    {
                        unitIndex.data.weaponHandler.rightWeapon?.GetComponent<Holdable>()?.Dissarm();
                        unitIndex.unitBlueprint.SetWeapon(unitIndex, unitIndex.Team, weapon, weapon.GetComponent<WeaponItem>().PropData ?? null, HandType.Right, Quaternion.identity, new List<GameObject>());
                    }
                    if (fields[0] == "Left")
                    {
                        unitIndex.data.weaponHandler.leftWeapon?.GetComponent<Holdable>()?.Dissarm();
                        unitIndex.unitBlueprint.SetWeapon(unitIndex, unitIndex.Team, weapon, weapon.GetComponent<WeaponItem>().PropData ?? null, HandType.Left, Quaternion.identity, new List<GameObject>());
                    }
                }

            }

            yield return savedNode.TriggerConnection(nodeRunner);

        }
    }
}