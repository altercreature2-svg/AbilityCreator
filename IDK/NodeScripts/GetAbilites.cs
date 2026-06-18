using AC.Node_Related_Scripts.NodeRunning;
using AC.Node_Related_Scripts.NodeRunning.Instructions.Courtines;
using Landfall.TABS;
using Landfall.TABS.AI.Systems;
using Landfall.TABS.UnitEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AC.NodeScripts
{
    public class GetAbilites : IValueNode
    {
        public IEnumerator<CoroutineReturn> Execute(NodeEnv env)
        {
            env.ClearValue(NodeBlueprint.ConnectionClass.GiveGameObject);
            var unitsEnum = env.GetValues(NodeBlueprint.ConnectionClass.ReciveUnit);
            foreach (var item in unitsEnum)
            {
                if (!(item.value is Unit u))
                    continue;
                SpecialAbility[] specialAbilities = env.cacheSystem.GetCachedComponentsInChildren<SpecialAbility>(env.unit.gameObject);
                foreach (var specialAbility in specialAbilities)
                {
                    env.AddValue(NodeBlueprint.ConnectionClass.GiveGameObject, specialAbility);
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