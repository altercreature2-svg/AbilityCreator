

using AC.Node_Related_Scripts.NodeRunning;
using AC.Node_Related_Scripts.NodeRunning.Instructions.Courtines;
using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AC.NodeScripts
{
    public class AddComponentNode : IBehaviorNode
    {
        public IEnumerator<CoroutineReturn> Execute(NodeEnv env)
        {
            var gameObjects = env.GetValues(NodeBlueprint.ConnectionClass.ReciveGameObject);
            foreach (var gameObj in gameObjects)
            {
                if (!(gameObj.value is GameObject obj))
                    continue;
                env.AddValue(NodeBlueprint.ConnectionClass.GiveGameObject, obj.AddComponent(AbilityCreator.components[env.GetField(0)]));    
            }
            yield return new CoroutineReturn(CoroutineReturn.CourtineType.ContinueBranch);
        }
        public IEnumerator<CoroutineReturn> Cache(NodeEnv env)
        {
            yield break;
        }
    }
}