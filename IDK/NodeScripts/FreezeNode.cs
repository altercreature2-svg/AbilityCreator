using AC.Help;
using AC.Node_Related_Scripts.NodeRunning;
using AC.Node_Related_Scripts.NodeRunning.Instructions.Courtines;
using BitCode.Extensions;
using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace AC.NodeScripts
{
    public class FreezeNode : IBehaviorNode
    {
        public IEnumerator<CoroutineReturn> Execute(NodeEnv env)
        {
            var units = env.GetValues(NodeBlueprint.ConnectionClass.ReciveUnit);
            float pause = env.GetField(0).QuickParse();
            FixedPool<Unit> pool = new FixedPool<Unit>(units.Length); 
            foreach (var item in units)
            {
                if (!(item.value is Unit u))
                    continue;
                u.gameObject.AddComponent<FreezeBody>().Freeze();
                u.gameObject.AddComponent<StopAttacks>().StopAttacksFor(pause);
                UnitDontWalkFor unitDontWalkFor = u.gameObject.AddComponent<UnitDontWalkFor>();
                unitDontWalkFor.time = pause;
                unitDontWalkFor.Go();
            }
            env.runner.StartCoroutine(UnfreezeAfterPause(pool.ToArray(), pause));
            yield return new CoroutineReturn(CoroutineReturn.CourtineType.ContinueBranch);
        }
        public IEnumerator<CoroutineReturn> Cache(NodeEnv env)
        {
            return null;
        }
        public IEnumerator UnfreezeAfterPause(Unit[] unitsToUnfreeze, float pause)
        {
            yield return new WaitForSeconds(pause);
            foreach (var unit in unitsToUnfreeze)
            {
                unit.gameObject.GetOrAddComponent<FreezeBody>().UnFreeze();
            }
        }
    }
}