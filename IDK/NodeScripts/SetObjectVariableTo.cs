using AC.Node_Related_Scripts;
using AC.Node_Related_Scripts.NodeRunning;
using AC.Node_Related_Scripts.NodeRunning.Instructions.Courtines;
using HarmonyLib;
using Landfall.TABS;
using Sirenix.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AC.NodeScripts
{
    public class SetObjectVariableTo : IBehaviorNode
    {
        public IEnumerator<CoroutineReturn> Execute(NodeEnv env)
        {
            var variablesEnum = env.GetValues(NodeBlueprint.ConnectionClass.ReciveObjectVariable);
            var objectsEnum = env.GetValues(NodeBlueprint.ConnectionClass.ReciveAnything);
            foreach (var item in variablesEnum)
            {
                if (!(item.value is ObjectVariable ov))
                    continue;
                foreach (var item2 in objectsEnum)
                {
                    if (!(item2.value is object o))
                        continue;
                    ov.value = ov.value.AddToArray(o);
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