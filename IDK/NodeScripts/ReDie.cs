using AC.Node_Related_Scripts.NodeRunning;
using AC.Node_Related_Scripts.NodeRunning.Instructions.Courtines;
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

namespace AC.NodeScripts
{
    public class ReDie : IBehaviorNode
    {
        public IEnumerator<CoroutineReturn> Execute(NodeEnv env)
        {
            var unitsEnum = env.GetValues(NodeBlueprint.ConnectionClass.ReciveUnit);
            foreach (var item in unitsEnum)
            {
                if (!(item.value is Unit u))
                    continue;
                GameObjectEntity entity = u.GetComponentInChildren<GameObjectEntity>();
                TeamSystem teamSystem = World.Active.GetOrCreateManager<TeamSystem>();
                teamSystem?.RemoveEntity(entity.Entity, u.Team, u);
                teamSystem?.AddUnit(entity.Entity, u.gameObject, u.transform, u.data.mainRig, u.data, u.Team, u, false);

                u.data.Dead = false;
                u.data.muscleControl = 1;
                u.data.ragdollControl = 1;
                u.data.fallTime = 0;
                Rigidbody[] rigs = u.GetComponentInChildren<RigidbodyHolder>().AllRigs;
                for (int i = 0; i < rigs.Length; i++)
                {
                    rigs[i].gameObject.layer = 0;
                }
                u.gameObject.AddComponent<Revive>().DoRevive();
            }
            yield return new CoroutineReturn(CoroutineReturn.CourtineType.ContinueBranch);
        }
        public IEnumerator<CoroutineReturn> Cache(NodeEnv env)
        {
            return null;
        }
    }
}