using AC.Node_Related_Scripts.NodeRunning;
using AC.Node_Related_Scripts.NodeRunning.Instructions.Courtines;
using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AC.NodeScripts
{
    public class GetWeapon : IValueNode
    {
        public IEnumerator<CoroutineReturn> Execute(NodeEnv env) 
        {
            var unitsEnum = env.GetValues(NodeBlueprint.ConnectionClass.ReciveUnit);
            foreach (var item in unitsEnum)
            {
                if (!(item.value is Unit u))
                    continue;
                switch (env.GetField(0))
                {
                    case "Left Weapon":
                        env.AddValue(NodeBlueprint.ConnectionClass.GiveGameObject,
                            u.data?.weaponHandler?.leftWeapon?.gameObject);
                        break;
                    case "Right Weapon":
                        env.AddValue(NodeBlueprint.ConnectionClass.GiveGameObject,
                            u.data?.weaponHandler?.rightWeapon?.gameObject);
                        break;
                    default:
                        env.AddValue(NodeBlueprint.ConnectionClass.GiveGameObject,
                            u.data?.weaponHandler?.rightWeapon?.gameObject);
                        env.AddValue(NodeBlueprint.ConnectionClass.GiveGameObject,
                            u.data?.weaponHandler?.leftWeapon?.gameObject);
                        break;
                }
            }
            yield return new CoroutineReturn(CoroutineReturn.CourtineType.ContinueBranch);
        }
        public IEnumerator<CoroutineReturn> Cache(NodeEnv env)
        {
            return null;
        } 
    }
}