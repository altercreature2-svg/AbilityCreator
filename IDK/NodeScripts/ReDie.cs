using HarmonyLib;
using Landfall.TABS;
using Landfall.TABS.AI.Systems;
using Landfall.TABS.GameMode;
using Landfall.TABS.WinConditions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Entities;
using UnityEngine;

namespace IDK.NodeScripts
{
    public class ReDie : IBehaviorNode
    {
        public override IEnumerator RunNode(LegacySavedNode savedNode, Unit unit, List<NodeComponent.LegacyConnection> connections, string[] fields, NodeRunner nodeRunner)
        {
            Unit[] units = connections.GetNode(NodeBlueprint.ConnectionClass.ReciveUnit).GetValuePoolSmart(unit).GetValues<Unit>();
            foreach (var unitIndex in units)
            {
                
                GameObjectEntity entity = unitIndex.GetComponentInChildren<GameObjectEntity>();
                TeamSystem teamSystem = World.Active.GetOrCreateManager<TeamSystem>();
                try
                {
                    teamSystem.RemoveEntity(entity.Entity, unitIndex.Team, unitIndex);
                    teamSystem.AddUnit(entity.Entity, unitIndex.gameObject, unitIndex.transform, unitIndex.data.mainRig, unitIndex.data, unitIndex.Team, unitIndex, false);
                } catch { };

                unitIndex.data.Dead = false;
                unitIndex.data.muscleControl = 1;
                unitIndex.data.ragdollControl = 1;
                unitIndex.data.fallTime = 0;
                Rigidbody[] rigs = unitIndex.GetComponentInChildren<RigidbodyHolder>().AllRigs;
                for (int i = 0; i < rigs.Length; i++)
                {
                    rigs[i].gameObject.layer = 0;
                }
                /*GroundChecker[] groundCheckers = unitIndex.unitBlueprint.UnitBase?.GetComponentsInChildren<GroundChecker>();
                for (int i = 0; i < groundCheckers.Length; i++)
                {
                    GroundChecker checker = groundCheckers[i];
                    Transform transform = checker.transform;
                    List<Transform> parents = new List<Transform>();
                    while (transform)
                    {
                        parents.Add(transform);
                        transform = transform.parent;
                    }
                    parents.Reverse();
                    parents.RemoveAt(0);
                    GameObject gameObject = null;
                    for (int i2 = 0; i2 < parents.Count; i2++)
                    {
                        Debug.Log("Searching for:" + parents[i2].name);
                        try
                        {
                            gameObject = unitIndex.transform.Find(parents[i2].name).gameObject;
                        }
                        catch
                        {
                            Debug.LogError("very bad"); 
                            Debug.Log("<#ff0000>very bad");
                            
                            break;
                        }
                    }
                    if (gameObject)
                    {
                        GroundChecker checker2 = gameObject.AddComponent<GroundChecker>();
                        checker2.footDownEvent = new UnityEngine.Events.UnityEvent();
                    }
                }*/
                
                unitIndex.gameObject.AddComponent<Revive>().DoRevive();
            }
            yield return savedNode.TriggerConnection(nodeRunner);

        }
    }
}