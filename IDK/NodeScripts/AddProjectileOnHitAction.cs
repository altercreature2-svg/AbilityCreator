

using HarmonyLib;
using IDK.Help_Componets;
using Landfall.TABS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using TFBGames;
using UnityEngine;

namespace IDK.NodeScripts
{
    public class AddProjectileOnHitAction : IBehaviorNode
    {
       
        public override IEnumerator RunNode(SavedNode savedNode, Unit unit, List<Node.Connection> connections, string[] fields, NodeRunner nodeRunner)
        {
            
            GameObject[] gameObjects = connections.GetNode(NodeBlueprint.ConnectionType.ReciveGameObject).GetValuePoolSmart(unit).GetValues<GameObject>();
            for (int i = 0; i < gameObjects.Length; i++)
            {
                if (gameObjects[i].GetComponent<ProjectileHit>())
                {
                    
                    ProjectileHit projectileHit = gameObjects[i].GetComponent<ProjectileHit>();
                    HitEvents hitEvent = new HitEvents
                    {
                        hitEvent = new UnityEngine.Events.UnityEvent()
                    };
                    hitEvent.hitEvent.AddListener(() => RunAction(unit,nodeRunner, savedNode, projectileHit));
                    projectileHit.hitEvents = projectileHit.hitEvents.AddItem(hitEvent).ToArray();
                }
            }
            
            yield return null;
        }
        public override ValuePool GetValuePool(SavedNode savedNode, Unit unit, List<Node.Connection> connections, string[] fields)
        {
            return savedNode.GetValuePool(unit);
        }
        public void RunAction(Unit unit, NodeRunner nodeRunner, SavedNode savedNode, ProjectileHit projectileHit)
        {
            Debug.Log("RUNNING ACTION!!!!");
            ValuePool valuePool = savedNode.GetValuePool(unit);
            valuePool.ClearValues();
            valuePool.AddValue(projectileHit.GetField<HitData>("hit").collider.transform.root.gameObject);
            nodeRunner.StartCoroutine(savedNode.TriggerConnection(nodeRunner));
        }
    }
}