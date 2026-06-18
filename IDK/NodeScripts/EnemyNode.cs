using AC.Node_Related_Scripts.NodeRunning;
using AC.Node_Related_Scripts.NodeRunning.Instructions.Courtines;
using Landfall.TABS;
using Landfall.TABS.AI.Systems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AC.NodeScripts
{
    public class EnemyNode : IValueNode
    {
        public IEnumerator<CoroutineReturn> Cache(NodeEnv env)
        {
            return null;
        }
        public IEnumerator<CoroutineReturn> Execute(NodeEnv env)
        {
            env.ClearValue(NodeBlueprint.ConnectionClass.GiveUnit);
            env.AddValue(NodeBlueprint.ConnectionClass.GiveUnit, env.unit.data.targetData.unit);
            yield return new CoroutineReturn(CoroutineReturn.CourtineType.ContinueBranch);
        }
    }
}