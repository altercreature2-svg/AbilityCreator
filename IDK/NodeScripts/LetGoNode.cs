using AC.Help;
using AC.Node_Related_Scripts.NodeRunning;
using AC.Node_Related_Scripts.NodeRunning.Instructions.Courtines;
using BitCode.Debug.Commands;
using BitCode.Extensions;
using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HoldingHandler;

namespace AC.NodeScripts
{
    public class LetGoNode : IBehaviorNode
    {
        public IEnumerator<CoroutineReturn> Execute(NodeEnv env)
        {
            var units = env.GetValues(NodeBlueprint.ConnectionClass.ReciveUnit);
            float pause = env.GetField(0).QuickParse();
            FixedPool<Unit> pool = new FixedPool<Unit>(units.Length);
            switch (env.GetField(0))
            {
                case "Both":
                    foreach (var item in units)
                    {
                        if (!(item.value is Unit u))
                            continue;
                        LetGoLeft(u, env);
                        LetGoRight(u, env);
                    }
                    break;
                case "Right Hand":
                    foreach (var item in units)
                    {
                        if (!(item.value is Unit u))
                            continue;
                        LetGoRight(u, env);
                    }
                    break;
                case "Left Hand":
                    foreach (var item in units)
                    {
                        if (!(item.value is Unit u))
                            continue;
                        LetGoLeft(u, env);
                    }
                    break;
                default:
                    break;
            }
            
            yield return new CoroutineReturn(CoroutineReturn.CourtineType.ContinueBranch);
        }
        public IEnumerator<CoroutineReturn> Cache(NodeEnv env)
        {
            return null;
        }

        

        public void LetGoRight(Unit unit, NodeEnv env)
        {
            
            HoldingHandler holdingHandler = unit.holdingHandler;
            Holdable rightObject = holdingHandler.rightObject;
            if (!rightObject)
                return;
            if (!rightObject.ignoreDissarm)
            {
                holdingHandler.rightHandActivity = HandActivity.NotHolding;
                rightObject.WasDropped();
                rightObject.rig.isKinematic = false;
                rightObject.gameObject.AddComponent<SetInterpolation>();
                holdingHandler.rightObject = null;
                var joint = holdingHandler.GetField<ConfigurableJoint>("rightHandJoint");
                if (joint)
                {
                    Object.Destroy(joint);
                }
            }
            if (env.cacheSystem.GetCachedComponent<Weapon>(rightObject.gameObject))
                unit.WeaponHandler.rightWeapon = null;
        }
        public void LetGoLeft(Unit unit, NodeEnv env)
        {
            HoldingHandler holdingHandler = unit.holdingHandler;
            Holdable leftObject = holdingHandler.leftObject;
            if (!leftObject)
                return;
            if (!leftObject.ignoreDissarm)
            {
                holdingHandler.leftHandActivity = HandActivity.NotHolding;
                leftObject.WasDropped();
                leftObject.rig.isKinematic = false;
                leftObject.gameObject.AddComponent<SetInterpolation>();
                holdingHandler.leftObject = null;
                var joint = holdingHandler.GetField<ConfigurableJoint>("rightHandJoint");
                if (joint)
                {
                    Object.Destroy(joint);
                }
            }
            if (env.cacheSystem.GetCachedComponent<Weapon>(leftObject.gameObject))
                unit.WeaponHandler.leftWeapon = null;
        }
    }
    
}