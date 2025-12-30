using Landfall.TABS;
using Landfall.TABS.AI.Components.Tags;
using Landfall.TABS.AI.Systems;
using Landfall.TABS.WinConditions;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace IDK.NodeScripts
{
    public class SwapTeam : IBehaviorNode
    {
        public override IEnumerator RunNode(SavedNode savedNode, Unit unit, List<Node.Connection> connections, string[] fields, NodeRunner nodeRunner)
        {
            yield return null;
            yield return null;
            yield return null;
            yield return null;
            yield return null;
            Unit[] units = connections.GetNode(NodeBlueprint.ConnectionType.ReciveUnit).GetValuePoolSmart(unit).GetValues<Unit>();
            foreach (var unitIndex in units)
            {
                FlipTeam(unitIndex);
                
            }
            yield return savedNode.TriggerConnection(nodeRunner);
            
        }
        public static void FlipTeam(Unit unitIndex)
        {
            var team = unitIndex.Team;
            unitIndex.Team = TeamUtlity.GetOtherTeam(team);
            unitIndex.data.team = TeamUtlity.GetOtherTeam(team);
            var entity = unitIndex.transform.root.GetComponent<GameObjectEntity>();
            entity.EntityManager.RemoveComponent<Landfall.TABS.AI.Components.Team>(entity.Entity);
            entity.EntityManager.AddSharedComponentData(entity.Entity, new Landfall.TABS.AI.Components.Team()
            {
                Value = (int)TeamUtlity.GetOtherTeam(team)
            });
            World.Active.GetOrCreateManager<TeamSystem>().RemoveEntity(entity.Entity, team, unitIndex);
            World.Active.GetOrCreateManager<TeamSystem>().AddUnit(entity.Entity, unitIndex.transform.root.gameObject, unitIndex.transform.root, unitIndex.data.mainRig, unitIndex.data, TeamUtlity.GetOtherTeam(team), unitIndex, true);

        }
    }
}