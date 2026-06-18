using AC.Node_Related_Scripts.NodeRunning;
using AC.Node_Related_Scripts.NodeRunning.Instructions.Courtines;
using IDK.Help;
using Landfall.TABS;
using Photon.Bolt.Utils;
using System.Collections;
using System.Collections.Generic;
using TFBGames;
using UnityEngine;

namespace AC.NodeScripts
{
    public class SpawnExplosionNode : IBehaviorNode
    {
        public EasyPool easyPool;
        public GameObject prefab;
        public float damage;
        public Team team;
        public IEnumerator<CoroutineReturn> Execute(NodeEnv env)
        {
            var objs = env.GetValues(NodeBlueprint.ConnectionClass.ReciveGameObject);
            foreach (var item in objs)
            {
                if (!(item.value is GameObject go))
                    continue;
                Transform transform = go.transform;
                GameObject explosion = easyPool.Spawn(transform.position, transform.rotation);
                env.AddValue(NodeBlueprint.ConnectionClass.GiveGameObject, explosion);
            }
            yield return new CoroutineReturn(CoroutineReturn.CourtineType.ContinueBranch);
        }
        public IEnumerator<CoroutineReturn> Cache(NodeEnv env)
        {
            prefab = AbilityCreator.explosions[env.GetField(0)];
            damage = env.GetField(1).QuickParse();
            team = env.GetField(2) == "My team" ? env.unit.Team : env.unit.Team.Reverse();
            easyPool = new EasyPool(prefab, go => OnSpawn(env,go));
            return null;
        }
        public void OnSpawn(NodeEnv env, GameObject go)
        {
            TeamHolder[] teamHolders = env.cacheSystem.GetCachedComponentsInChildren<TeamHolder>(go);
            Explosion explosion = env.cacheSystem.GetCachedComponent<Explosion>(go);
            foreach (var teamHolder in teamHolders)
            {
                teamHolder.team = team;
            }
            explosion?.SetField("team", team);
            explosion?.SetField("ownUnit", env.unit);
        }
    }
}