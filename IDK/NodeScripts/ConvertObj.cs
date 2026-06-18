using AC.Node_Related_Scripts;
using AC.Node_Related_Scripts.NodeRunning;
using AC.Node_Related_Scripts.NodeRunning.Instructions.Courtines;
using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AC.NodeScripts
{
    public class ConvertObj : IValueNode
    {
        public IEnumerator<CoroutineReturn> Execute(NodeEnv env)
        {
            var values = env.GetValues(NodeBlueprint.ConnectionClass.ReciveGameObject);
            foreach (var val in values)
            {
                env.AddValue(NodeBlueprint.ConnectionClass.GiveAnything, val);
            }
            yield return new CoroutineReturn(CoroutineReturn.CourtineType.ContinueBranch);
        }
        public IEnumerator<CoroutineReturn> Cache(NodeEnv env)
        {
            return null;
        }
    }
}