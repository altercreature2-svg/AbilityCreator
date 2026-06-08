using Landfall.TABS;
using System.Collections.Generic;
using UnityEngine;

namespace IDK.NodeScripts
{
    public class WhenProjectileEntersRange : ITriggerNode
    {
        public override ValuePool GetValuePool(LegacySavedNode savedNode, Unit unit, List<NodeComponent.LegacyConnection> connections, string[] fields)
        {
            return savedNode.GetValuePool(unit);
        }
        public override void EveryFrame(LegacySavedNode savedNode, Unit unit, List<NodeComponent.LegacyConnection> connections, string[] fields, NodeRunner nodeRunner)
        {

        }
        public override void StartFrame(LegacySavedNode savedNode, Unit unit, List<NodeComponent.LegacyConnection> connections, string[] fields, NodeRunner nodeRunner)
        {
            float blockPower = fields[0].QuickParse();
            float range = fields[0].QuickParse();
            if (!nodeRunner.GetComponent<BlockMove>())
            {
                BlockMove blockMove = nodeRunner.gameObject.AddComponent<BlockMove>();
                blockMove.enableEvent = new UnityEngine.Events.UnityEvent();
                blockMove.disableEvent = new UnityEngine.Events.UnityEvent();
                blockMove.goodReflectChance = 1;
                blockMove.projectilesPerBlock = 1;
                blockMove.reflect = true;
                blockMove.useSliceEffect = false;
                blockMove.useLineEffect = false;
                blockMove.blockAngle = 360;
                blockMove.blockMoveImpulse = 99999;
                blockMove.blockFriendlyProjectiles = false;
                blockMove.blockCurve = AnimationCurve.Constant(0, 1, 1);
                blockMove.blockPower = blockPower;
                blockMove.timeBetweenMeleeBlocks = 0;

                // blockMove.sliceEffect = GameObject.Find("E_Slice");
            }
            void Block(GameObject gameObject)
            {
                ValuePool valuePool = savedNode.GetValuePool(unit);
                valuePool.ClearValues();
                valuePool.AddValue(gameObject);
                nodeRunner.StartCoroutine(nodeRunner.TriggerConnection(savedNode));
            }
            //for (int i = 1; i < 5; i++)
            //{
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.parent = nodeRunner.transform;
            sphere.transform.localScale *= range;
            sphere.transform.position = unit.data.torso.position;
            sphere.layer = LayerMask.NameToLayer("Block");
            //sphere.GetComponent<SphereCollider>().isTrigger = true;
            sphere.AddComponent<SurfaceEvent>().onBlock.AddListener(n => Block(n));
            sphere.AddComponent<FollowTransform>().target = nodeRunner.transform;
            Object.Destroy(sphere.GetComponent<MeshRenderer>());
            //}


        }

    }

}