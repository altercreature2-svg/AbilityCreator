using AC.Node_Related_Scripts;
using AC.Node_Related_Scripts.NodeRunning;
using AC.Node_Related_Scripts.NodeRunning.Instructions.Courtines;
using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AC.NodeScripts
{
    public class GetUnitHealth : IValueNode
    {
        public IEnumerator<CoroutineReturn> Execute(NodeEnv env)
        {
            env.ClearValue(NodeBlueprint.ConnectionClass.GiveVariable);
            var unitsEnum = env.GetValues(NodeBlueprint.ConnectionClass.ReciveUnit);
            bool usePrecentage = env.GetField(0) == "Normal";  
            foreach (var item in unitsEnum)
            {
                if (!(item.value is Unit u))
                    continue;
                
                if (!usePrecentage)
                    env.AddValue(NodeBlueprint.ConnectionClass.GiveVariable, new Variable() { value = u.data.health });
                else
                {
                    float precent = u.data.health / u.data.maxHealth;
                    env.AddValue(NodeBlueprint.ConnectionClass.GiveVariable, new Variable() { value = Mathf.Clamp01(precent) * 100 });
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