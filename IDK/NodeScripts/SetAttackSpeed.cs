using AC.Node_Related_Scripts.NodeRunning;
using AC.Node_Related_Scripts.NodeRunning.Instructions.Courtines;
using Landfall.TABS;
using System.Collections.Generic;

namespace AC.NodeScripts
{
    public class SetAttackSpeed : IBehaviorNode
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
                SetSpeed(env,u , value, mode);
            }
            yield return new CoroutineReturn(CoroutineReturn.CourtineType.ContinueBranch);
        }
        public IEnumerator<CoroutineReturn> Cache(NodeEnv env)
        {
            value = env.GetField(0).QuickParse();
            mode = env.GetField(1);
            return null;
        }
        private void SetSpeed(NodeEnv env, Unit u, float value, string mode)
        {
            Weapon[] weapons = env.cacheSystem.GetCachedComponentsInChildren<Weapon>(u.gameObject);
            switch (mode)
            {
                case "Set":
                    for (int i = 0; i < weapons.Length; i++)
                    {
                        weapons[i].internalCooldown = value;
                    }
                    break;
                case "Add":
                    for (int i = 0; i < weapons.Length; i++)
                    {
                        weapons[i].internalCooldown += value;
                    }
                    break;
                case "Multiply":
                    for (int i = 0; i < weapons.Length; i++)
                    {
                        weapons[i].internalCooldown *= value;
                    }
                    break;
                case "Set (%)":
                    for (int i = 0; i < weapons.Length; i++)
                    {
                        weapons[i].internalCooldown = (weapons[i].internalCooldown / 100) * value;
                    }
                    break;
                case "Add (%)":
                    for (int i = 0; i < weapons.Length; i++)
                    {
                        weapons[i].internalCooldown += (weapons[i].internalCooldown / 100) * value;
                    }
                    break;
                default:
                    break;
            }
        }
    }
}