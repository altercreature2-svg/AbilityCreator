using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using TFBGames;
using UnityEngine;
using static UnityEngine.ParticleSystem;

namespace IDK.NodeScripts
{
    public class SpawnProjectileNode : IBehaviorNode
    {
        public override ValuePool GetValuePool(SavedNode savedNode, Unit unit, List<Node.Connection> connections, string[] fields)
        {
            return savedNode.GetValuePool(unit);
        }
        public override IEnumerator RunNode(SavedNode savedNode, Unit unit, List<Node.Connection> connections, string[] fields, NodeRunner nodeRunner)
        {
            GameObject[] gameObjects = connections.GetNode(NodeBlueprint.ConnectionType.ReciveGameObject).GetValuePoolSmart(unit).GetValues<GameObject>();
            ValuePool valuePool = savedNode.GetValuePool(unit);
            valuePool.ClearValues();
            foreach (var obj in gameObjects)
            {
                GameObject proj = SpawnProjectile(Main.Projectiles[fields[0]], obj.transform.position, unit, obj.transform, obj.transform, fields[1].QuickParse());
                if (fields[2] == "Other team only")
                    proj.AddComponent<IgnoreProjectileForTeam>().team = unit.Team;
                else if (fields[2] == "Same team only")
                    proj.AddComponent<IgnoreProjectileForTeam>().team = unit.Team.Reverse();
                valuePool.AddValue(proj);
            }
            yield return savedNode.TriggerConnection(nodeRunner);

        }
        public static GameObject SpawnProjectile(GameObject proj, Vector3 pos, Unit u, Transform spawn, Transform mainrig, float spread)
        {
            var spawnDirection = GetSpawnDirection(spawn.forward, spawn.forward, mainrig);
            return ServiceLocator.GetService<ProjectilesSpawnManager>().SpawnProjectile(proj, pos, Quaternion.LookRotation(spawnDirection + 0.01f * spread * UnityEngine.Random.insideUnitSphere), u, 0, spawnDirection, spawn.forward, null, spawn.forward, out var projectile); ;
        }
        private static Vector3 GetSpawnDirection(Vector3 directionToTarget, Vector3 forcedDirection, Transform mainrig)
        {

            Vector3 result = Vector3.Lerp(directionToTarget, mainrig.forward, new AnimationCurve().Evaluate(Vector3.Angle(directionToTarget, mainrig.forward))).normalized;

            return result;
        }
    }
}