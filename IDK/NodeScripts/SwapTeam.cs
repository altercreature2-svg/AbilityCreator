using AC.Node_Related_Scripts.NodeRunning;
using AC.Node_Related_Scripts.NodeRunning.Instructions.Courtines;
using Landfall.TABS;
using Landfall.TABS.AI.Components.Tags;
using Landfall.TABS.AI.Systems;
using Landfall.TABS.GameMode;
using Landfall.TABS.WinConditions;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace AC.NodeScripts
{
    public class SwapTeam : IBehaviorNode
    {
        public IEnumerator<CoroutineReturn> Execute(NodeEnv env)
        {
            var units = env.GetValues(NodeBlueprint.ConnectionClass.ReciveUnit);
            foreach (var item in units)
            {
                if (!(item.value is Unit u))
                    continue;
                FlipTeam(u);
            }
            yield return new CoroutineReturn(CoroutineReturn.CourtineType.ContinueBranch);
        }
        public IEnumerator<CoroutineReturn> Cache(NodeEnv env)
        {
            return null;
        }
        public static void FlipTeam(Unit unitIndex)
        {
            var team = unitIndex.Team;
            unitIndex.Team = TeamUtlity.GetOtherTeam(team);
            unitIndex.data.team = TeamUtlity.GetOtherTeam(team);
            var entity = unitIndex.transform.root.GetComponent<GameObjectEntity>();

            entity.EntityManager.SetSharedComponentData(entity.Entity, new Landfall.TABS.AI.Components.Team()
            {
                Value = (int)TeamUtlity.GetOtherTeam(team)
            });

            TeamSystem teamSystem = World.Active.GetOrCreateManager<TeamSystem>();
            ServiceLocator.GetService<GameModeService>().CurrentGameMode.OnUnitDied(unitIndex);
            teamSystem.RemoveEntity(entity.Entity, team, unitIndex);
            teamSystem.AddUnit(entity.Entity, unitIndex.transform.root.gameObject, unitIndex.transform.root, unitIndex.data.mainRig, unitIndex.data, TeamUtlity.GetOtherTeam(team), unitIndex, true);

        }
    }
}