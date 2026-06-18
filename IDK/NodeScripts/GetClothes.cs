using AC.Node_Related_Scripts.NodeRunning;
using AC.Node_Related_Scripts.NodeRunning.Instructions.Courtines;
using Landfall.TABS;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Landfall.TABS.UnitRig;

namespace AC.NodeScripts
{
    public class GetClothes : IValueNode
    {
        UnitRig.GearType gearType = default;
        public IEnumerator<CoroutineReturn> Execute(NodeEnv env)
        {

            env.ClearValue(NodeBlueprint.ConnectionClass.GiveGameObject);
            var units = env.GetValues(NodeBlueprint.ConnectionClass.ReciveUnit);
            Debug.Log("Units Length: " + units.Length);
            foreach (var item in units)
            {
                if (!(item.value is Unit u))
                    continue;

                PropItem[] props = env.cacheSystem.GetCachedComponentsInChildren<PropItem>(u.gameObject);
                for (int i = 0; i < props.Length; i++)
                {
                    env.AddValue(NodeBlueprint.ConnectionClass.GiveGameObject, props[i]);
                }
            }

            yield return new CoroutineReturn(CoroutineReturn.CourtineType.ContinueBranch);
        }
        public IEnumerator<CoroutineReturn> Cache(NodeEnv env)
        {
            switch (env.GetField(0))
            {
                case "All":
                    gearType = (UnitRig.GearType.HEAD) - 1;
                    break;
                case "Head":
                    gearType = UnitRig.GearType.HEAD | UnitRig.GearType.NECK;
                    break;
                case "Torso":
                    gearType = UnitRig.GearType.TORSO | UnitRig.GearType.WAIST;
                    break;
                case "Arms":
                    gearType = UnitRig.GearType.ARMS | UnitRig.GearType.SHOULDER;
                    break;
                case "Pants":
                    gearType = UnitRig.GearType.LEGS;
                    break;
                case "Shoes":
                    gearType = UnitRig.GearType.FEET;
                    break;
                default:
                    break;
            }
            return null;
        }
    }
}