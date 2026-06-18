using AC.Node_Related_Scripts;
using AC.Node_Related_Scripts.NodeRunning;
using AC.Node_Related_Scripts.NodeRunning.Instructions.Courtines;
using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AC.NodeScripts
{
    public class SetUnitHealth : IBehaviorNode
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
                SetHealth(env, u, value, mode);
            }
            yield return new CoroutineReturn(CoroutineReturn.CourtineType.ContinueBranch);
        }
        public IEnumerator<CoroutineReturn> Cache(NodeEnv env)
        {
            value = env.GetField(0).QuickParse();
            mode = env.GetField(1);
            return null;
        }
        private void SetHealth(NodeEnv env, Unit u, float value, string mode)
        {
            Weapon[] weapons = env.cacheSystem.GetCachedComponentsInChildren<Weapon>(env.unit.gameObject);
            switch (mode)
            {
                case "Set":
                    u.data.health = value;
                    break;
                case "Add":
                    u.data.health += value;
                    break;
                case "Multiply":
                    u.data.health *= value;
                    break;
                case "Set (%)":
                    u.data.health = (u.data.maxHealth / 100) * value;
                    break;
                case "Add (%)":
                    u.data.health += (u.data.maxHealth / 100) * value;
                    break;
                default:
                    break;
            }
        }
    }
}