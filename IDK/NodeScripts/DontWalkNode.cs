using AC.Node_Related_Scripts.NodeRunning;
using AC.Node_Related_Scripts.NodeRunning.Instructions.Courtines;
using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace AC.NodeScripts
{
    public class DontWalkNode : IBehaviorNode
    {
        public IEnumerator<CoroutineReturn> Execute(NodeEnv env)
        {
            var units = env.GetValues(NodeBlueprint.ConnectionClass.ReciveUnit);
            foreach (var item in units)
            {
                if (!(item.value is Unit u))
                    continue;
                UnitDontWalkFor unitDontWalkFor = env.unit.gameObject.AddComponent<UnitDontWalkFor>();
                unitDontWalkFor.time = env.GetField(0).QuickParse();
                unitDontWalkFor.Go();
            }
            yield return new CoroutineReturn(CoroutineReturn.CourtineType.ContinueBranch);
        }
        public IEnumerator<CoroutineReturn> Cache(NodeEnv env)
        {
            return null;
        }
    }
}