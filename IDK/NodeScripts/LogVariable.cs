using AC.Node_Related_Scripts;
using AC.Node_Related_Scripts.NodeRunning;
using AC.Node_Related_Scripts.NodeRunning.Instructions.Courtines;
using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AC.NodeScripts
{
    public class LogVariable : IBehaviorNode
    {
        public IEnumerator<CoroutineReturn> Execute(NodeEnv env)
        {
            var everything = env.GetValues(NodeBlueprint.ConnectionClass.ReciveVariable);
            foreach (var eachthing in everything)
            {
                if (!(eachthing.value is Variable v))
                    continue;
                DeveloperLogger.Log("(" + env.runner.NodeScene.abilityName + ")" + "Log:" + v, true);
            }
            yield return new CoroutineReturn(CoroutineReturn.CourtineType.ContinueBranch);
        }

        public IEnumerator<CoroutineReturn> Cache(NodeEnv env)
        {
            return null;
        }
    }
}