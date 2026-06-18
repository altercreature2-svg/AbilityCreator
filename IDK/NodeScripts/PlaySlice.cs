using AC.Node_Related_Scripts.NodeRunning;
using AC.Node_Related_Scripts.NodeRunning.Instructions.Courtines;
using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AC.NodeScripts
{
    public class PlaySlice : IBehaviorNode
    {
        public IEnumerator<CoroutineReturn> Execute(NodeEnv env)
        {
            env.ClearValue(NodeBlueprint.ConnectionClass.GiveGameObject);
            var gameObjects = env.GetValues(NodeBlueprint.ConnectionClass.ReciveGameObject);

            foreach (var item in gameObjects)
            {
                if (!(item.value is GameObject projectile))
                    continue;

                Vector3 spawnRot = env.unit.data ?
                        projectile.transform.position - env.unit.data.mainRig.transform.position :
                        projectile.transform.position - env.unit.data.torso.position;
                Vector3 sliceDir = env.unit.data ?
                    Vector3.Cross((projectile.transform.position - env.unit.data.torso.transform.position).normalized, env.unit.data.characterForwardObject.forward).normalized :
                    Vector3.forward;

                GameObject sliceEffect = PlaySliceEffect(projectile.transform.position, Quaternion.LookRotation(spawnRot, sliceDir), env);
                env.AddValue(NodeBlueprint.ConnectionClass.GiveGameObject, sliceEffect);
            }
            yield return new CoroutineReturn(CoroutineReturn.CourtineType.ContinueBranch);
        }
        public IEnumerator<CoroutineReturn> Cache(NodeEnv env)
        {
            return null;
        }
        private GameObject PlaySliceEffect(Vector3 pos, Quaternion rot, NodeEnv env)
        {
            GameObject obj = UnityEngine.Object.Instantiate(AbilityCreator.sliceEffect, null);
            obj.transform.position = pos;
            obj.transform.rotation = rot;
            env.cacheSystem.GetCachedComponent<CodeAnimation>(obj)?.PlayIn();
            obj.AddComponent<RemoveAfterSeconds>().seconds = 0.5f;
            return obj;
        }
    }
}