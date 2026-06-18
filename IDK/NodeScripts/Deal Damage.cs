using AC.Node_Related_Scripts.NodeRunning;
using AC.Node_Related_Scripts.NodeRunning.Instructions.Courtines;
using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AC.NodeScripts
{
    public class Deal_Damage : IBehaviorNode
    {
        public IEnumerator<CoroutineReturn> Execute(NodeEnv env)
        {
            var units = env.GetValues(NodeBlueprint.ConnectionClass.ReciveUnit);
            foreach (var item in units)
            {
                if (!(item.value is Unit u))
                    continue;
                if (env.GetField(1) == "Normal")
                    u.data.healthHandler.TakeDamage(env.GetField(0).QuickParse(), Vector3.zero);
                else
                    u.data.healthHandler.TakeDamage((u.data.maxHealth/100)*env.GetField(0).QuickParse(), Vector3.zero);
            }
            yield return new CoroutineReturn(CoroutineReturn.CourtineType.ContinueBranch);
        }
        public IEnumerator<CoroutineReturn> Cache(NodeEnv env)
        {
            return null;
        }
    }
}