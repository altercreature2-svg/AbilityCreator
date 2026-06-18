using AC.Node_Related_Scripts.NodeRunning;
using AC.Node_Related_Scripts.NodeRunning.Instructions.Courtines;
using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AC.NodeScripts
{
    public class PauseNode : IBehaviorNode
    {
        float time = 0f;
        public IEnumerator<CoroutineReturn> Execute(NodeEnv env)
        {
            yield return new CoroutineReturn(CoroutineReturn.CourtineType.WaitForSeconds, time);
            yield return new CoroutineReturn(CoroutineReturn.CourtineType.ContinueBranch);
        }

        public IEnumerator<CoroutineReturn> Cache(NodeEnv env)
        {
            time = env.GetField(0).QuickParse();
            return null;
        }
    }
}