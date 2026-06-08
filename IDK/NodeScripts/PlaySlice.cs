using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IDK.NodeScripts
{
    public class PlaySlice : IBehaviorNode
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
                Transform rig = unit.data.torso;

                sliceDir = Vector3.Cross((projectile.transform.position - rig.transform.position).normalized, unit.data.characterForwardObject.forward).normalized;
                spawnRot = projectile.transform.position - unit.data.mainRig.transform.position;
                GameObject sliceEffect = PlaySliceEffect(projectile.transform.position, Quaternion.LookRotation(spawnRot, sliceDir));
                valuePool.AddValue(sliceEffect);
            }
            yield return savedNode.TriggerConnection(nodeRunner);

        }
        private GameObject PlaySliceEffect(Vector3 pos, Quaternion rot)
        {
            GameObject obj = UnityEngine.Object.Instantiate(AbilityCreator.sliceEffect, null);
            obj.transform.position = pos;
            obj.transform.rotation = rot;
            obj.GetComponent<CodeAnimation>()?.PlayIn();
            obj.AddComponent<RemoveAfterSeconds>().seconds = 0.5f;
            return obj;
        }
    }
}