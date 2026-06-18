

using AC.Node_Related_Scripts.NodeRunning;
using AC.Node_Related_Scripts.NodeRunning.Instructions.Courtines;
using InControl.NativeDeviceProfiles;
using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace AC.NodeScripts
{
    public class Duplicate : IBehaviorNode
    {
        public IEnumerator<CoroutineReturn> Execute(NodeEnv env)
        {
            var gameObjects = env.GetValues(NodeBlueprint.ConnectionClass.ReciveGameObject);
            foreach (var item in gameObjects)
            {
                if (!(item.value is UnityEngine.Object obj))
                    continue;
                env.AddValue(NodeBlueprint.ConnectionClass.GiveGameObject, Object.Instantiate(obj));
            }
            yield return new CoroutineReturn(CoroutineReturn.CourtineType.ContinueBranch);
        }
        public IEnumerator<CoroutineReturn> Cache(NodeEnv env)
        {
            return null;
        }
    }
}