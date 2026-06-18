using AC.Node_Related_Scripts.NodeRunning;
using AC.Node_Related_Scripts.NodeRunning.Instructions.Courtines;
using Landfall.TABS;
using System.Collections.Generic;

namespace AC.NodeScripts
{
    public class SetUnitDamage : IBehaviorNode
    {
        float value;
        string mode;
        public IEnumerator<CoroutineReturn> Execute(NodeEnv env)
        {
            var unitsEnum = env.GetValues(NodeBlueprint.ConnectionClass.ReciveUnit);
            foreach (var item in unitsEnum)
            {
                if (!(item.value is Unit u))
                    continue;
                Weapon[] allWeapons = env.cacheSystem.GetCachedComponentsInChildren<Weapon>(u.gameObject);
                for (int i = 0; i < allWeapons.Length; i++)
                {

                    SetDamageForWeapon(env, allWeapons[i], value, mode);
                }
            }
            yield return new CoroutineReturn(CoroutineReturn.CourtineType.ContinueBranch);
        }
        public IEnumerator<CoroutineReturn> Cache(NodeEnv env)
        {
            value = env.GetField(0).QuickParse();
            mode = env.GetField(1);
            return null;
        }
        private void SetDamageForWeapon(NodeEnv env, Weapon weapon, float value, string mode) // NOT anything for optomizations ig
        {
            float oldLevelMultiplier = weapon.levelMultiplier;
            switch (mode)
            {
                case "Set":
                    weapon.levelMultiplier = value;
                    break;
                case "Add":
                    weapon.levelMultiplier += value;
                    break;
                case "Multiply":
                    weapon.levelMultiplier *= value;
                    break;
                case "Set (%)":
                    weapon.levelMultiplier = (weapon.levelMultiplier / 100) * value;
                    break;
                case "Add (%)":
                    weapon.levelMultiplier += (weapon.levelMultiplier / 100) * value;
                    break;
                default:
                    break;
            }
            Level level = env.cacheSystem.GetCachedComponent<Level>(weapon.gameObject);
            if (level)
                level.levelMultiplier = weapon.levelMultiplier;
            if (weapon is MeleeWeapon meleeWeapon)
            {
                CollisionWeapon collisionWeapon = (CollisionWeapon)env.cacheSystem.GetCachedComponent<CollisionWeapon>(meleeWeapon.gameObject);
                if (!collisionWeapon)
                    return;
                collisionWeapon.damage /= oldLevelMultiplier;
                collisionWeapon.damage *= meleeWeapon.levelMultiplier;
            }
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