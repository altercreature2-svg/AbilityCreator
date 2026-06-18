using AC.Node_Related_Scripts;
using AC.Node_Related_Scripts.NodeRunning;
using AC.Node_Related_Scripts.NodeRunning.Instructions.Courtines;
using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AC.NodeScripts
{
    public class IfVariableNode : IBehaviorNode
    {
        public IEnumerator<CoroutineReturn> Execute(NodeEnv env)
        {
           
            var variablesEnum = env.GetValues(NodeBlueprint.ConnectionClass.ReciveVariable);
            float value = env.GetField(0).QuickParse();
            string option = env.GetField(1);
            foreach (var item in variablesEnum)
            {
                if (!(item.value is Variable variable))
                    continue;
                Debug.Log("value:" + variable.value);
                if (option == "=")
                    if (variable.value == value)
                        yield return new CoroutineReturn(CoroutineReturn.CourtineType.ContinueBranch);
                if (option == ">")
                    if (variable.value > value)
                        yield return new CoroutineReturn(CoroutineReturn.CourtineType.ContinueBranch);
                if (option == "<")
                    if (variable.value < value)
                        yield return new CoroutineReturn(CoroutineReturn.CourtineType.ContinueBranch);
                if (option == ">=")
                    if (variable.value >= value)
                        yield return new CoroutineReturn(CoroutineReturn.CourtineType.ContinueBranch);
                if (option == "<=")
                    if (variable.value <= value)
                        yield return new CoroutineReturn(CoroutineReturn.CourtineType.ContinueBranch);
            }
            
        }
        public IEnumerator<CoroutineReturn> Cache(NodeEnv env)
        {
            return null;
        }
    }
}