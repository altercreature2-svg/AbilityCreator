

using AC.Help_Componets;
using AC.Node_Related_Scripts.NodeRunning;
using AC.Node_Related_Scripts.NodeRunning.Instructions.Courtines;
using HarmonyLib;
using Landfall.TABS;
using LevelCreator;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using TFBGames;
using UnityEngine;

namespace AC.NodeScripts
{
    public class AddProjectileOnHitAction : IBehaviorNode
    {
        bool waitUntilCollision = true;
        public IEnumerator<CoroutineReturn> Execute(NodeEnv env)
        {
            var gameObjects = env.GetValues(NodeBlueprint.ConnectionClass.ReciveGameObject);
            foreach (var gameObject in gameObjects)
            {
                if (!(gameObject.value is GameObject go))
                    continue;
                ProjectileHit hit = env.cacheSystem.GetCachedComponent<ProjectileHit>(go);
                HitEvents hitEvent = new HitEvents
                {
                    hitEvent = new UnityEngine.Events.UnityEvent()
                };
                hitEvent.hitEvent.AddListener(() => OnProjectileHitEvent(hit, env));
                hit.hitEvents = hit.hitEvents.AddToArray(hitEvent);
            }
            while (waitUntilCollision)
            {
                yield return new CoroutineReturn(CoroutineReturn.CourtineType.PauseFrame);
            }

            yield return new CoroutineReturn(CoroutineReturn.CourtineType.ContinueBranch);

        }
        public void OnProjectileHitEvent(ProjectileHit hit, NodeEnv env)
        {
            env.AddValue(NodeBlueprint.ConnectionClass.GiveGameObject, hit.GetField<HitData>("hit").collider.transform.root.gameObject);
            waitUntilCollision = false;
        }
        public IEnumerator<CoroutineReturn> Cache(NodeEnv env)
        {
            return null;
        }
    }
}