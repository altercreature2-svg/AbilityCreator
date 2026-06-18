using AC.Node_Related_Scripts.NodeRunning;
using AC.Node_Related_Scripts.NodeRunning.Instructions.Courtines;
using BitCode.Extensions;
using IDK.Help;
using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using TFBGames;
using UnityEngine;
using static UnityEngine.ParticleSystem;

namespace AC.NodeScripts
{
    public class SpawnProjectileNode : IBehaviorNode
    {
        public EasyPool easyPool;
        public GameObject prefab;
        public float spread;
        public int mode;
        public IEnumerator<CoroutineReturn> Execute(NodeEnv env)
        {
            
            var gameObjects = env.GetValues(NodeBlueprint.ConnectionClass.ReciveGameObject);
            env.ClearValue(NodeBlueprint.ConnectionClass.GiveGameObject);
            foreach (var item in gameObjects)
            {
                if (!(item.value is GameObject go))
                    continue;
                GameObject pooled = easyPool.Spawn(Vector3.zero, Quaternion.identity);
                GameObject projectile = SmartProjectileSpawner.SpawnProjectile(prefab, go, env.unit, go.transform, go.transform, spread, pooledProjectile:pooled);
                env.AddValue(NodeBlueprint.ConnectionClass.GiveGameObject, projectile);
            }
            yield return new CoroutineReturn(CoroutineReturn.CourtineType.ContinueBranch);
        }
        public IEnumerator<CoroutineReturn> Cache(NodeEnv env)
        {
            prefab = AbilityCreator.projectiles[env.GetField(0)];
            spread = env.GetField(1).QuickParse();
            mode = env.GetField(2) == "Other Team only" ? 1 : mode;
            mode = env.GetField(2) == "Same Team only" ? 2 : mode;
            easyPool = new EasyPool(prefab, go => OnSpawn(env, go));
            return null;
        }
        public void OnSpawn(NodeEnv env, GameObject go)
        {
            switch (mode)
            {
                case 1:
                    go.AddComponent<IgnoreProjectileForTeam>().team = env.unit.Team;
                    break;
                case 2:
                    go.AddComponent<IgnoreProjectileForTeam>().team = env.unit.Team.Reverse();
                    break;
            }
        }
    }
}