using AC.Node_Related_Scripts.NodeRunning;
using AC.Node_Related_Scripts.NodeRunning.Instructions.Courtines;
using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AC.NodeScripts
{
    public class ReapetNode : IBehaviorNode
    {
        int times;
        float interval;
        public IEnumerator<CoroutineReturn> Execute(NodeEnv env)
        {
            AbilityCreator.reapeter.AddTask(times, interval, i => env.RunTrigger());
            yield return new CoroutineReturn(CoroutineReturn.CourtineType.ContinueBranch);
        }
        public IEnumerator<CoroutineReturn> Cache(NodeEnv env)
        {
            times = env.GetField(0).QuickParseInt();
            interval = env.GetField(1).QuickParse();
            return null;
        }
    }
}