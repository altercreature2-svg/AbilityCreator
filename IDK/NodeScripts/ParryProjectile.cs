using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IDK.NodeScripts
{
    public class ParryProjectile : IBehaviorNode
    {
        public override ValuePool GetValuePool(LegacySavedNode savedNode, Unit unit, List<NodeComponent.LegacyConnection> connections, string[] fields)
        {
            return savedNode.GetValuePool(unit);
        }
        public override IEnumerator RunNode(LegacySavedNode savedNode, Unit unit, List<NodeComponent.LegacyConnection> connections, string[] fields, NodeRunner nodeRunner)
        {
            ValuePool valuePool = savedNode.GetValuePool(unit);
            valuePool.ClearValues();
            GameObject[] gameObjs = connections.GetNode(NodeBlueprint.ConnectionClass.ReciveGameObject).GetValuePoolSmart(unit).GetValues<GameObject>();
            for (int o = 0; o < gameObjs.Length; o++)
            {


                GameObject projectile = gameObjs[o];
                Vector3 spawnRot;
                Vector3 sliceDir;
                Vector3 normal = unit.data.torso.position - projectile.transform.position;
                Transform rig = unit.data.torso;
                bool reflect = fields[1] == "Reflect";
                ProjectileHit component2 = projectile.GetComponent<ProjectileHit>();
                if (component2)
                {
                    if ((bool)component2)
                    {

                        if (component2.destroyOnRefect)
                        {
                            Object.Destroy(component2.gameObject);
                        }
                    }


                    MoveTransform component3 = projectile.GetComponent<MoveTransform>();

                    if (reflect)
                    {

                        component3.velocity = Vector3.Reflect(component3.velocity, normal) * 1.25f;
                        component3.velocity *= UnityEngine.Random.Range(0.3f, 0.5f);

                    }
                    projectile.GetComponent<RaycastTrail>().ignoredFrames = 3;
                    if ((bool)unit.data)
                    {
                        spawnRot = projectile.transform.position - unit.data.mainRig.transform.position;
                        sliceDir = Vector3.Cross((projectile.transform.position - rig.transform.position).normalized, unit.data.characterForwardObject.forward).normalized;
                    }
                    else
                    {
                        spawnRot = projectile.transform.position - rig.position;
                        sliceDir = Vector3.forward;
                    }
                    if (fields[0] == "Play Slice")
                        PlaySlice(projectile.transform.position, Quaternion.LookRotation(spawnRot, sliceDir));





                    if (!projectile.transform.GetChild(0).gameObject.GetComponent<ProjectileRotate>() && reflect)
                    {

                        projectile.transform.rotation = Quaternion.LookRotation(projectile.GetComponent<TeamHolder>().spawner.transform.GetComponentInChildren<DataHandler>().mainRig.transform.position - unit.data.torso.position);

                    }
                }
                else
                {
                    sliceDir = Vector3.Cross((projectile.transform.position - rig.transform.position).normalized, unit.data.characterForwardObject.forward).normalized;
                    spawnRot = projectile.transform.position - unit.data.mainRig.transform.position;
                    if (fields[0] == "Play Slice")
                    {
                        PlaySlice(projectile.transform.position, Quaternion.LookRotation(spawnRot, sliceDir));
                    }
                    if (reflect)
                    {
                        projectile.GetComponent<Rigidbody>()?.AddForce(-300*(unit.data.torso.position - projectile.transform.position));
                    }
                }
            }
            yield return savedNode.TriggerConnection(nodeRunner);

        }
        private void PlaySlice(Vector3 pos, Quaternion rot)
        {
            GameObject obj = UnityEngine.Object.Instantiate(AbilityCreator.sliceEffect, null);
            obj.transform.position = pos;
            obj.transform.rotation = rot;
            obj.GetComponent<CodeAnimation>()?.PlayIn();
            obj.AddComponent<RemoveAfterSeconds>().seconds = 0.5f;
        }
    }
}