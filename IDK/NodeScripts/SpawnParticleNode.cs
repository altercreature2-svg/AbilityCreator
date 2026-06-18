using AC.Node_Related_Scripts.NodeRunning;
using AC.Node_Related_Scripts.NodeRunning.Instructions.Courtines;
using IDK.Help;
using Landfall.TABS;
using Landfall.TABS.AI.Components;
using Landfall.TABS.RuntimeCleanup;
using Photon.Bolt.Utils;
using System.Collections;
using System.Collections.Generic;
using TFBGames;
using UnityEngine;

namespace AC.NodeScripts
{
    public class SpawnParticleNode : IBehaviorNode
    {
        public EasyPool easyPool;
        public GameObject prefab;
        public float scale;
        public float speed;
        public float length;
        public bool loop;
        public bool follow;
        public RuntimeGarbageCollector garbageCollector;
        public IEnumerator<CoroutineReturn> Execute(NodeEnv env)
        {
            var gameObjects = env.GetValues(NodeBlueprint.ConnectionClass.ReciveGameObject);
            foreach (var item in gameObjects)
            {
                if (!(item.value is GameObject go))
                    continue;
                Transform transform = go.transform;
                GameObject particle = easyPool.Spawn(transform.position, transform.rotation);
                ParticleSystem particleSystem = env.cacheSystem.GetCachedComponent<ParticleSystem>(particle);
                if (!particleSystem.main.playOnAwake)
                    particleSystem.Play();
                if (follow)
                    particle.transform.parent = transform;
                env.AddValue(NodeBlueprint.ConnectionClass.GiveGameObject, particle);
            }
            yield return new CoroutineReturn(CoroutineReturn.CourtineType.ContinueBranch);
        }
        public IEnumerator<CoroutineReturn> Cache(NodeEnv env)
        {
            prefab = AbilityCreator.particles[env.GetField(0)];
            scale = env.GetField(1).QuickParse();
            scale = scale == 0 ? 1 : scale;
            loop = env.GetField(2) != "Don't";
            speed = env.GetField(3).QuickParse();
            speed = speed == 0 ? 1 : speed;
            length = env.GetField(4).QuickParse();
            follow = env.GetField(5) == "Follow";
            garbageCollector = ServiceLocator.GetService<RuntimeGarbageCollector>();
            easyPool = new EasyPool(prefab, go => OnSpawn(env, go));
            return null;
        }
        public void OnSpawn(NodeEnv env, GameObject particle)
        {
            garbageCollector.AddGameObject(particle);
            ParticleSystem particleSystem = env.cacheSystem.GetCachedComponent<ParticleSystem>(particle);
            particle.transform.localScale *= scale;
            ParticleSystem.MainModule main = particleSystem.main;
            main.loop = loop;
            main.simulationSpeed *= speed;
            particle.AddComponent<RemoveAfterSeconds>().seconds = length;
        }
    }
}