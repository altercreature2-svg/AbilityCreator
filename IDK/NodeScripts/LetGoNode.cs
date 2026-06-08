using BitCode.Debug.Commands;
using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HoldingHandler;

namespace IDK.NodeScripts
{
    public class LetGoNode : IBehaviorNode
    {
        public override IEnumerator RunNode(LegacySavedNode savedNode, Unit unit, List<NodeComponent.LegacyConnection> connections, string[] fields, NodeRunner nodeRunner)
        {
            Unit[] units = connections.GetNode(NodeBlueprint.ConnectionClass.ReciveUnit).GetValuePoolSmart(unit).GetValues<Unit>();
            foreach (var unitIndex in units)
            {
                
                if (fields[0] == "Both")
                {
                    LetGoRight(unitIndex);
                    LetGoLeft(unitIndex);
                }
                if (fields[0] == "Right hand")
                {
                    LetGoRight(unitIndex);
                }
                if (fields[0] == "Left hand")
                {
                    LetGoLeft(unitIndex);
                }
            }

            yield return savedNode.TriggerConnection(nodeRunner);

        }

        public void LetGoRight(Unit unitIndex)
        {
            Holdable rightObject = unitIndex.holdingHandler.rightObject;
            HoldingHandler holdingHandler = unitIndex.holdingHandler;
            if ((bool)rightObject && !rightObject.ignoreDissarm)
            {
                holdingHandler.rightHandActivity = HandActivity.NotHolding;
                rightObject.WasDropped();
            }


            bool flag = false;
            if ((bool)rightObject && rightObject.ignoreDissarm)
            {
                flag = true;
            }
            if (rightObject != null && !rightObject.ignoreDissarm)
            {
                rightObject.rig.isKinematic = false;
                rightObject.gameObject.AddComponent<SetInterpolation>();
                holdingHandler.rightObject = null;
            }
            if (!flag)
            {
                if ((bool)holdingHandler.GetField<ConfigurableJoint>("rightHandJoint"))
                {
                    UnityEngine.Object.Destroy(holdingHandler.GetField<ConfigurableJoint>("rightHandJoint"));
                }

            }
            if (rightObject?.GetComponent<Weapon>())
                unitIndex.WeaponHandler.rightWeapon = null;
            holdingHandler.rightHandActivity = HandActivity.NotHolding;
            holdingHandler.rightObject = null;
        }
        public void LetGoLeft(Unit unitIndex)
        {
            Holdable leftObject = unitIndex.holdingHandler.leftObject;
            HoldingHandler holdingHandler = unitIndex.holdingHandler;
            if ((bool)leftObject && !leftObject.ignoreDissarm)
            {
                holdingHandler.leftHandActivity = HandActivity.NotHolding;
                leftObject.WasDropped();
            }


            bool flag = false;
            if ((bool)leftObject && leftObject.ignoreDissarm)
            {
                flag = true;
            }
            if (leftObject != null && !leftObject.ignoreDissarm)
            {
                leftObject.rig.isKinematic = false;
                leftObject.gameObject.AddComponent<SetInterpolation>();
                holdingHandler.leftObject = null;
            }
            if (!flag)
            {
                if ((bool)holdingHandler.GetField<ConfigurableJoint>("leftHandJoint"))
                {
                    UnityEngine.Object.Destroy(holdingHandler.GetField<ConfigurableJoint>("leftHandJoint"));
                }

            }
            if (leftObject?.GetComponent<Weapon>())
                unitIndex.WeaponHandler.leftWeapon = null;
            holdingHandler.leftHandActivity = HandActivity.NotHolding;
            holdingHandler.leftObject = null;
        }
    }
    
}