

using AC.Help_Componets;
using AC.Node_Related_Scripts.NodeRunning;
using AC.Node_Related_Scripts.NodeRunning.Instructions.Courtines;
using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace AC.NodeScripts
{
    public class RunFunctionNode : IBehaviorNode
    {
        string funcName;
        FunctionStorer functionStorer;
        public IEnumerator<CoroutineReturn> Execute(NodeEnv env)
        {
            functionStorer.RunAction(funcName);
            yield return new CoroutineReturn(CoroutineReturn.CourtineType.ContinueBranch);
        }
        public IEnumerator<CoroutineReturn> Cache(NodeEnv env)
        {
            functionStorer = env.cacheSystem.GetCachedComponent<FunctionStorer>(env.unit.gameObject);
            if (!functionStorer)
                functionStorer = env.unit.gameObject.AddComponent<FunctionStorer>();
            funcName = env.GetField(0);
            return null;
        }
    }
}