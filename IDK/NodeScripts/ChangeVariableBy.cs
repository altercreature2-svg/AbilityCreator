using AC.Node_Related_Scripts;
using AC.Node_Related_Scripts.NodeRunning;
using AC.Node_Related_Scripts.NodeRunning.Instructions.Courtines;
using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AC.NodeScripts
{
    public class ChangeVariableBy : IBehaviorNode
    {
        public IEnumerator<CoroutineReturn> Execute(NodeEnv env)
        {
            
            var variables = env.GetValues(NodeBlueprint.ConnectionClass.ReciveVariable);
            foreach (var variable in variables)
            {
                if ((variable.value is Variable))
                    continue;
                Variable v = variable.value as Variable;
                v.value += env.GetField(0).QuickParse();
            }
            yield return new CoroutineReturn(CoroutineReturn.CourtineType.ContinueBranch);
        }
        public IEnumerator<CoroutineReturn> Cache(NodeEnv env)
        {
            return null;
        }
    }
}