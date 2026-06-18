using AC.Node_Related_Scripts.NodeRunning;
using AC.Node_Related_Scripts.NodeRunning.Instructions.Courtines;
using Landfall.TABS;
using System.Collections.Generic;
using UnityEngine;

namespace AC.NodeScripts
{
    public class WhenProjectileEntersRange : ITriggerNode
    {
        public IEnumerator<CoroutineReturn> Execute(NodeEnv env)
        {
            return null;
        }
        public IEnumerator<CoroutineReturn> Cache(NodeEnv env)
        {
            float blockPower = env.GetField(0).QuickParse();
            float range = env.GetField(1).QuickParse();
            if (!env.runner.GetComponent<BlockMove>())
            {
                BlockMove blockMove = env.runner.gameObject.AddComponent<BlockMove>();
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
                env.ClearValue(NodeBlueprint.ConnectionClass.GiveGameObject);
                env.AddValue(NodeBlueprint.ConnectionClass.GiveGameObject, gameObject);
                env.GetTriggerCore();
            }
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.parent = env.runner.transform;
            sphere.transform.localScale *= range;
            sphere.transform.position = env.unit.data.torso.position;
            sphere.layer = LayerMask.NameToLayer("Block");
            //sphere.GetComponent<SphereCollider>().isTrigger = true;
            sphere.AddComponent<SurfaceEvent>().onBlock.AddListener(n => Block(n));
            sphere.AddComponent<FollowTransform>().target = env.runner.transform;
            Object.Destroy(sphere.GetComponent<MeshRenderer>());
            return null;
        }

    }

}