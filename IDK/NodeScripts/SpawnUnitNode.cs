using AC.Node_Related_Scripts.NodeRunning;
using AC.Node_Related_Scripts.NodeRunning.Instructions.Courtines;
using DM;
using Landfall.TABS;
using Landfall.TABS.UnitPlacement;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace AC.NodeScripts
{
    public class SpawnUnitNode : IBehaviorNode
    {
        public UnitBlueprint unitToSpawn;
        public Team team;
        public IEnumerator<CoroutineReturn> Execute(NodeEnv env)
        {
            var gameObjects = env.GetValues(NodeBlueprint.ConnectionClass.ReciveGameObject);
            foreach (var item in gameObjects)
            {
                if (!(item.value is GameObject go))
                    continue;
                Transform transform = go.transform;
                GameObject unitGO = unitToSpawn.Spawn(transform.position, Quaternion.identity, team)[0];
                env.AddValue(NodeBlueprint.ConnectionClass.GiveGameObject, unitGO);
            }
            yield return new CoroutineReturn(CoroutineReturn.CourtineType.ContinueBranch);
        }
        public IEnumerator<CoroutineReturn> Cache(NodeEnv env)
        {
            unitToSpawn = AbilityCreator.units[env.GetField(0)];
            team = env.GetField(1) == "My Team" ? env.unit.Team : env.unit.Team.Reverse();
            return null;
        }
    }
}