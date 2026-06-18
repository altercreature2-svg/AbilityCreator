

using HarmonyLib;
using AC.Help_Componets;
using Landfall.TABS;
using LevelCreator;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using AC.Node_Related_Scripts.NodeRunning.Instructions.Courtines;
using AC.Node_Related_Scripts.NodeRunning;

namespace AC.NodeScripts
{
    public class AddOnCollisionAction : IBehaviorNode
    {
        bool waitUntilCollision = true;
        public IEnumerator<CoroutineReturn> Execute(NodeEnv env)
        {
            var gameObjects = env.GetValues(NodeBlueprint.ConnectionClass.ReciveGameObject);
            foreach (var gameObject in gameObjects)
            {
                if (!(gameObject.value is GameObject go))
                    continue;
                OnCollisionEvent collisionEvent = go.AddComponent<OnCollisionEvent>();
                collisionEvent.enter.AddListener((n => OnCollisionEnterEvent(n, env)));
            }
            while (waitUntilCollision)
            {
                yield return new CoroutineReturn(CoroutineReturn.CourtineType.PauseFrame);
            }
            
            yield return new CoroutineReturn(CoroutineReturn.CourtineType.ContinueBranch);

        }
        public void OnCollisionEnterEvent(Collision other, NodeEnv env)
        {
            env.AddValue(NodeBlueprint.ConnectionClass.GiveGameObject, other);
            waitUntilCollision = false;
        }
        public IEnumerator<CoroutineReturn> Cache(NodeEnv env)
        {
            return null;
        }
    }
}