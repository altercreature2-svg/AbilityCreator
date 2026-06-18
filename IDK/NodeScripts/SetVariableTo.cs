using AC.Node_Related_Scripts;
using AC.Node_Related_Scripts.NodeRunning;
using AC.Node_Related_Scripts.NodeRunning.Instructions.Courtines;
using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AC.NodeScripts
{
    public class SetVariableTo : IBehaviorNode
    {
        double value;
        public IEnumerator<CoroutineReturn> Execute(NodeEnv env)
        {
            var variablesEnum = env.GetValues(NodeBlueprint.ConnectionClass.ReciveVariable);
            foreach (var item in variablesEnum)
            {
                if (!(item.value is Variable v))
                    continue;
                v.value = value;
            }
            yield return new CoroutineReturn(CoroutineReturn.CourtineType.ContinueBranch);
        }
        public IEnumerator<CoroutineReturn> Cache(NodeEnv env)
        {
            value = env.GetField(0).QuickParseDouble();
            return null;
        }
    }
}