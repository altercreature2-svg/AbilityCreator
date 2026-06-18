using AC.Help;
using AC.Help_Componets;
using AC.Node_Related_Scripts.NodeRunning;
using AC.Node_Related_Scripts.NodeRunning.Instructions.Courtines;
using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AC.NodeScripts
{
    public class PlayAllAbilites : IBehaviorNode
    {
        public IEnumerator<CoroutineReturn> Execute(NodeEnv env)
        {
            var units = env.GetValues(NodeBlueprint.ConnectionClass.ReciveUnit);
            foreach (var item in units)
            {
                if (!(item.value is Unit u))
                    continue;
                var condos = env.cacheSystem.GetCachedComponentsInChildren<ConditionalEvent>(u.gameObject);
                for (int i = 0; i < condos.Length; i++)
                {
                    for (int j = 0; j < condos[i].events.Length; j++)
                    {
                        condos[i].events[j].continuousEvent.Invoke();
                        condos[i].events[j].turnOnEvent.Invoke();
                    }
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