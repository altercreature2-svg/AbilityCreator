using AC.Node_Related_Scripts.NodeRunning;
using AC.Node_Related_Scripts.NodeRunning.Instructions.Courtines;
using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HoldingHandler;

namespace AC.NodeScripts
{
    public class SetWeaponNode : IBehaviorNode
    {
        int mode;
        public IEnumerator<CoroutineReturn> Execute(NodeEnv env)
        {
            var unitsEnum = env.GetValues(NodeBlueprint.ConnectionClass.ReciveUnit);
            var gosEnum = env.GetValues(NodeBlueprint.ConnectionClass.ReciveGameObject);
            foreach ( var item in unitsEnum )
            {
                if (!(item.value is Unit u))
                    continue;
                foreach (var item2 in gosEnum)
                {
                    if (!(item2.value is GameObject go))
                        continue;
                    switch (mode)
                    {
                        case 0:
                            u.data.weaponHandler.rightWeapon?.GetComponent<Holdable>()?.Dissarm();
                            u.data.weaponHandler.leftWeapon?.GetComponent<Holdable>()?.Dissarm();
                            u.unitBlueprint.SetWeapon(u, u.Team, go, env.cacheSystem.GetCachedComponent<WeaponItem>(go)?.PropData, HandType.Right, Quaternion.identity, new List<GameObject>());
                            u.holdingHandler.rightHandActivity = HandActivity.HoldingRightObject;
                            u.holdingHandler.leftHandActivity = HandActivity.HoldingRightObject;
                            break;
                        case 1:
                            u.data.weaponHandler.rightWeapon?.GetComponent<Holdable>()?.Dissarm();
                            u.unitBlueprint.SetWeapon(u, u.Team, go, env.cacheSystem.GetCachedComponent<WeaponItem>(go)?.PropData, HandType.Right, Quaternion.identity, new List<GameObject>());
                            u.holdingHandler.rightHandActivity = HandActivity.HoldingRightObject;
                            break;
                        case 2:
                            u.data.weaponHandler.leftWeapon?.GetComponent<Holdable>()?.Dissarm();
                            u.unitBlueprint.SetWeapon(u, u.Team, go, env.cacheSystem.GetCachedComponent<WeaponItem>(go)?.PropData, HandType.Left, Quaternion.identity, new List<GameObject>());
                            u.holdingHandler.leftHandActivity = HandActivity.HoldingLeftObject;
                            break;
                        default:
                            break;
                    }
                }
            }
            yield return new CoroutineReturn(CoroutineReturn.CourtineType.ContinueBranch);
        }
        public IEnumerator<CoroutineReturn> Cache(NodeEnv env)
        {
            mode = env.GetField(0) == "Both" ? 0 : 999;
            mode = env.GetField(0) == "Right" ? 1 : mode;
            mode = env.GetField(0) == "Left" ? 2 : mode;
            return null;
        }
    }
}