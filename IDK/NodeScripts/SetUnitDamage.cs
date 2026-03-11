using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;

namespace IDK.NodeScripts
{
    public class SetUnitDamage : IBehaviorNode
    {
        public override IEnumerator RunNode(SavedNode savedNode, Unit unit, List<Node.Connection> connections, string[] fields, NodeRunner nodeRunner)
        {
            Unit[] units = connections.GetNode(NodeBlueprint.ConnectionType.ReciveUnit).GetValuePoolSmart(unit).GetValues<Unit>();
            for (int i = 0; i < units.Length; i++)
            {

                SetDamage(units[i], fields[0].QuickParse(), fields[1]);
            }
            yield return savedNode.TriggerConnection(nodeRunner);
        }
        private void SetDamage(Unit unit, float value, string field)
        {
            Weapon[] weapons = unit.GetComponentsInChildren<Weapon>();
            for (int i = 0; i < weapons.Length; i++)
            {
                float oldLevelMultiplier = weapons[i].levelMultiplier;
                
                if (field == "Set")
                    weapons[i].levelMultiplier = value;
                if (field == "Add")
                    weapons[i].levelMultiplier += value;
                if (field == "Multiply")
                    weapons[i].levelMultiplier *= value;
                if (field == "Set (%)")
                    weapons[i].levelMultiplier = (weapons[i].levelMultiplier / 100) * value;
                if (field == "Add (%)")
                    weapons[i].levelMultiplier += (weapons[i].levelMultiplier / 100) * value;

                if (weapons[i] is MeleeWeapon meleeWeapon)
                {
                    CollisionWeapon collisionWeapon = (CollisionWeapon)meleeWeapon.GetField("collision");
                    if (!collisionWeapon)
                        continue;
                    collisionWeapon.damage /= oldLevelMultiplier;
                    collisionWeapon.damage *= meleeWeapon.levelMultiplier;
                }
            }
        }
    }
}