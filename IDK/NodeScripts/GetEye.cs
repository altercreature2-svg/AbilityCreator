using AC.Node_Related_Scripts.NodeRunning;
using AC.Node_Related_Scripts.NodeRunning.Instructions.Courtines;
using Landfall.TABS;
using Landfall.TABS.UnitEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AC.NodeScripts
{
    public class GetEye : IValueNode
    {
        public IEnumerator<CoroutineReturn> Execute(NodeEnv env)
        {
            env.ClearValue(NodeBlueprint.ConnectionClass.GiveGameObject);
            var unitsEnum = env.GetValues(NodeBlueprint.ConnectionClass.ReciveUnit);
            foreach (var item in unitsEnum)
            {
                if (!(item.value is Unit u))
                    continue;
                EyeSpawner eyeSpawner = env.cacheSystem.GetCachedComponentInChildren<EyeSpawner>(u.gameObject);
                env.AddValues(NodeBlueprint.ConnectionClass.GiveGameObject, eyeSpawner.spawnedEyes.Select(n => n.gameObject).ToArray());

            }
            yield return new CoroutineReturn(CoroutineReturn.CourtineType.ContinueBranch);
        }
        public IEnumerator<CoroutineReturn> Cache(NodeEnv env)
        {
            return null;
        }
    }
}