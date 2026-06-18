using AC.Node_Related_Scripts.NodeRunning;
using AC.Node_Related_Scripts.NodeRunning.Instructions.Courtines;
using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using TFBGames;
using UnityEngine;

namespace AC.NodeScripts
{
    public class AddEffectNode : IBehaviorNode
    {
        UnitEffectBase unitEffectBase;
        
        public IEnumerator<CoroutineReturn> Execute(NodeEnv env)
        {
            var units = env.GetValues(NodeBlueprint.ConnectionClass.ReciveUnit);
            
            foreach (var item in units) 
            {
                if (!(item.value is Unit unit))
                    continue;
                var effect = UnitEffectBase.AddEffectToTarget(unit.gameObject, unitEffectBase);
                effect.transform.position = unit.transform.position;
                UnitEffectBase spawnedEffect = env.cacheSystem.GetCachedComponent<UnitEffectBase>(effect.gameObject);
                if (spawnedEffect)
                {
                    spawnedEffect.DoEffect();
                }
            }
            yield return new CoroutineReturn(CoroutineReturn.CourtineType.ContinueBranch);

        }
        public IEnumerator<CoroutineReturn> Cache(NodeEnv env)
        {
            unitEffectBase = env.cacheSystem.GetCachedComponent<UnitEffectBase>(AbilityCreator.effects[env.GetField(0)]);
            yield break;
        }  
    }
}