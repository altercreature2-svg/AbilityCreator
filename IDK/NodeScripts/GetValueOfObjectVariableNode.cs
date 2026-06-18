using BitCode.Extensions;
using HarmonyLib;
using AC.Node_Related_Scripts;
using Landfall.TABS;
using System.Collections.Generic;
using UnityEngine;
using AC.Node_Related_Scripts.NodeRunning.Instructions.Courtines;
using AC.Node_Related_Scripts.NodeRunning;

namespace AC.NodeScripts
{
    public class GetValueOfObjectVariableNode : IValueNode
    {
        public IEnumerator<CoroutineReturn> Cache(NodeEnv env)
        {
            return null;
        }
        public IEnumerator<CoroutineReturn> Execute(NodeEnv env)
        {
            string name = env.GetField(0);
            var reciveObjects = env.GetValues(NodeBlueprint.ConnectionClass.ReciveObjectVariable);
            foreach (var reciveObject in reciveObjects)
            {
                if (!(reciveObject.value is ObjectVariable objectVariable))
                    continue;
                object[] store = objectVariable.value;
                env.AddValues(NodeBlueprint.ConnectionClass.GiveAnything, store);
            }
            yield return new CoroutineReturn(CoroutineReturn.CourtineType.ContinueBranch);
        }
    }
}