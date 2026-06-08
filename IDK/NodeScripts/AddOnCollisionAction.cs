

using HarmonyLib;
using IDK.Help_Componets;
using Landfall.TABS;
using LevelCreator;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

namespace IDK.NodeScripts
{
    public class AddOnCollisionAction : IBehaviorNode
    {
        public override IEnumerator RunNode(LegacySavedNode savedNode, Unit unit, List<NodeComponent.LegacyConnection> connections, string[] fields, NodeRunner nodeRunner)
        {
            ValuePool valuePool = savedNode.GetValuePool(unit);
            GameObject[] gameObjects = connections.GetNode(NodeBlueprint.ConnectionClass.ReciveGameObject).GetValuePoolSmart(unit).GetValues<GameObject>();
            for (int i = 0; i < gameObjects.Length; i++)
            {
                OnCollisionEvent collisionEvent = gameObjects[i].AddComponent<OnCollisionEvent>();
                collisionEvent.enter.AddListener((n => OnCollisionEnterEvent(n, savedNode, unit,nodeRunner)));
            }
            
            yield return null;

        }
        public void OnCollisionEnterEvent(Collision other, LegacySavedNode savedNode,Unit unit, NodeRunner nodeRunner)
        {
            ValuePool valuePool = savedNode.GetValuePool(unit);
            valuePool.ClearValues();
            valuePool.AddValue(other.gameObject);
            valuePool.AddValue(other.rigidbody);
            nodeRunner.StartCoroutine(nodeRunner.TriggerConnection(savedNode));
        }
        public override ValuePool GetValuePool(LegacySavedNode savedNode, Unit unit, List<NodeComponent.LegacyConnection> connections, string[] fields)
        {
            return savedNode.GetValuePool(unit);
        }
        public void RunAction(NodeRunner nodeRunner, LegacySavedNode savedNode, ProjectileHit projectileHit)
        {
            Debug.Log("RUNNING ACTION!!!!");
            nodeRunner.StartCoroutine(savedNode.TriggerConnection(nodeRunner));
        }
    }
}