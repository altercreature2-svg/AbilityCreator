using AC.Node_Related_Scripts;
using AC.Node_Related_Scripts.NodeRunning;
using AC.Node_Related_Scripts.NodeRunning.Instructions.Courtines;
using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AC.NodeScripts
{
    public class GetDistanceFrom : IValueNode
    {
        public IEnumerator<CoroutineReturn> Execute(NodeEnv env)
        {
            env.ClearValue(NodeBlueprint.ConnectionClass.GiveVariable);
            var units = env.GetValues(NodeBlueprint.ConnectionClass.ReciveUnit);
            var gameObjects = env.GetValues(NodeBlueprint.ConnectionClass.ReciveGameObject);
            foreach (var item in units)
            {
                if (!(item.value is Unit u))
                    continue;
                foreach (var item2 in gameObjects)
                {
                    if (!(item2.value is GameObject go))
                        continue;
                    env.AddValue(NodeBlueprint.ConnectionClass.GiveVariable, new Variable()
                    {
                        value = Vector3.Distance(u.data.mainRig.position, go.transform.position)
                    });
                }
            }
            env.AddValue(NodeBlueprint.ConnectionClass.GiveVariable, AbilityCreator.abilites[env.GetField(0)]);
            yield return new CoroutineReturn(CoroutineReturn.CourtineType.ContinueBranch);
        }
        public IEnumerator<CoroutineReturn> Cache(NodeEnv env)
        {
            return null;
        }

    }
}