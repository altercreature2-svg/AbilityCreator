using AC.Node_Related_Scripts.NodeRunning;
using AC.Node_Related_Scripts.NodeRunning.Instructions.Courtines;
using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AC.NodeScripts
{
    public class ParryProjectile : IBehaviorNode
    {
        bool playSlice;
        bool reflect;
        public IEnumerator<CoroutineReturn> Execute(NodeEnv env)
        {
            var gameObjects = env.GetValues(NodeBlueprint.ConnectionClass.ReciveGameObject);
            
            foreach (var item in gameObjects)
            {
                if (!(item.value is GameObject projectile))
                    continue;

                Vector3 spawnRot = env.unit.data ?
                        projectile.transform.position - env.unit.data.mainRig.transform.position :
                        projectile.transform.position - env.unit.data.torso.position;
                Vector3 sliceDir = env.unit.data ?
                    Vector3.Cross((projectile.transform.position - env.unit.data.torso.transform.position).normalized, env.unit.data.characterForwardObject.forward).normalized :
                    Vector3.forward;
                Vector3 normal = env.unit.data.torso.position - projectile.transform.position;

                ProjectileHit projectileHit = env.cacheSystem.GetCachedComponent<ProjectileHit>(projectile);
                if (projectileHit)
                {
                    if (projectileHit.destroyOnHit)
                    {
                        Object.Destroy(projectileHit.gameObject);
                        continue;
                    }
                    MoveTransform moveTransform = env.cacheSystem.GetCachedComponent<MoveTransform>(projectile);
                    RaycastTrail raycastTrail = env.cacheSystem.GetCachedComponent<RaycastTrail>(projectile);
                    if (reflect && moveTransform)
                        moveTransform.velocity = Vector3.Reflect(moveTransform.velocity, normal) * Random.Range(0.3f, .5f) * 1.25f;
                    if (raycastTrail)
                        raycastTrail.ignoredFrames = 3;
                    
                    if (playSlice)
                        PlaySlice(projectile.transform.position, Quaternion.LookRotation(spawnRot, sliceDir), env);
                    if (!env.cacheSystem.GetCachedComponentInChildren<ProjectileRotate>(projectile) && reflect)
                    {
                        TeamHolder teamHolder = env.cacheSystem.GetCachedComponent<TeamHolder>(projectile);
                        DataHandler spawner = env.cacheSystem.GetCachedComponent<Unit>(teamHolder.spawner.transform.root.gameObject).data;
                        projectile.transform.rotation = Quaternion.LookRotation(spawner.mainRig.transform.position - spawner.torso.position);
                    }
                }
                else
                {
                    if (playSlice)
                        PlaySlice(projectile.transform.position, Quaternion.LookRotation(spawnRot, sliceDir), env);
                    if (reflect)
                        env.cacheSystem.GetCachedComponent<Rigidbody>(projectile)?.AddForce(-300 * normal);
                }
            }
            yield return new CoroutineReturn(CoroutineReturn.CourtineType.ContinueBranch);
        }
        public IEnumerator<CoroutineReturn> Cache(NodeEnv env)
        {
            playSlice = env.GetField(0) == "Play Slice";
            reflect = env.GetField(1) == "Reflect";
            return null;
        }
        private void PlaySlice(Vector3 pos, Quaternion rot, NodeEnv env)
        {
            GameObject obj = UnityEngine.Object.Instantiate(AbilityCreator.sliceEffect, null);
            obj.transform.position = pos;
            obj.transform.rotation = rot;
            env.cacheSystem.GetCachedComponent<CodeAnimation>(obj)?.PlayIn();
            obj.AddComponent<RemoveAfterSeconds>().seconds = 0.5f;
        }
    }
}