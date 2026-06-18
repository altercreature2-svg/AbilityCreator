using AC.Node_Related_Scripts.NodeRunning;
using AC.Node_Related_Scripts.NodeRunning.Instructions.Courtines;
using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AC.NodeScripts
{
    public class SetWillBeRevived : IBehaviorNode
    {
        bool willBeRevived;
        public IEnumerator<CoroutineReturn> Execute(NodeEnv env)
        {
            var units = env.GetValues(NodeBlueprint.ConnectionClass.ReciveUnit);
            foreach (var item in units)
            {
                if (!(item.value is Unit u))
                    continue;
                u.data.healthHandler.willBeRewived = willBeRevived;
            }
            yield return new CoroutineReturn(CoroutineReturn.CourtineType.ContinueBranch);
        }
        public IEnumerator<CoroutineReturn> Cache(NodeEnv env)
        {
            willBeRevived = env.GetField(0) == "Will be revived";
            return null;
        }
    }
}