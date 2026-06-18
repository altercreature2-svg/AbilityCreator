using AC.Node_Related_Scripts.NodeRunning;
using AC.Node_Related_Scripts.NodeRunning.Instructions.Courtines;
using Landfall.TABS;
using Landfall.TABS.GameMode;
using Landfall.TABS.GameState;
using Landfall.TABS.UnitEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AC.NodeScripts
{
    public class IsAlive : IBehaviorNode
    {
        public IEnumerator<CoroutineReturn> Execute(NodeEnv env)
        {
            var unitsEnum = env.GetValues(NodeBlueprint.ConnectionClass.ReciveUnit);
            foreach (var item in unitsEnum)
            {
                if (!(item.value is Unit u))
                    continue;
                if (u.data.Dead) continue;
                yield return new CoroutineReturn(CoroutineReturn.CourtineType.ContinueBranch);
            }
            
        }
        public IEnumerator<CoroutineReturn> Cache(NodeEnv env)
        {
            return null;
        }
    }
}