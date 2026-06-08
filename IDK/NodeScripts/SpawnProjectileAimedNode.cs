using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using TFBGames;
using UnityEngine;
using static UnityEngine.ParticleSystem;

namespace IDK.NodeScripts
{
    public class SpawnProjectileAimedNode : IBehaviorNode
    {
        public override ValuePool GetValuePool(LegacySavedNode savedNode, Unit unit, List<NodeComponent.LegacyConnection> connections, string[] fields)
        {
            return savedNode.GetValuePool(unit);
        }
        public override IEnumerator RunNode(LegacySavedNode savedNode, Unit unit, List<NodeComponent.LegacyConnection> connections, string[] fields, NodeRunner nodeRunner)
        {
            GameObject[] gameObjects = connections.GetNode(NodeBlueprint.ConnectionClass.ReciveGameObject).GetValuePoolSmart(unit).GetValues<GameObject>();
            Unit[] units = connections.GetNode(NodeBlueprint.ConnectionClass.ReciveUnit).GetValuePoolSmart(unit).GetValues<Unit>();
            ValuePool valuePool = savedNode.GetValuePool(unit);
            valuePool.ClearValues();
            foreach (var obj in gameObjects)
            {
               
                GameObject proj = SmartProjectileSpawner.SpawnProjectile(AbilityCreator.projectiles[fields[0]], obj, unit, obj.transform, obj.transform, fields[1].QuickParse(), units[0].data.mainRig.position, units [0] , true);
                if (fields[2] == "Other team only")
                    proj.AddComponent<IgnoreProjectileForTeam>().team = unit.Team;
                else if (fields[2] == "Same team only")
                    proj.AddComponent<IgnoreProjectileForTeam>().team = unit.Team.Reverse();
                valuePool.AddValue(proj);
            }
            yield return savedNode.TriggerConnection(nodeRunner);

        }
        
    }
}