using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace IDK.NodeScripts
{
    public class GetGameobject : IValueNode
    {
        public override ValuePool GetDynamicValue(LegacySavedNode savedNode, Unit unit, List<NodeComponent.LegacyConnection> connections, string[] fields)
        {
            ValuePool valuePool = new ValuePool();
            Unit[] units = connections.GetNode(NodeBlueprint.ConnectionClass.ReciveUnit).GetValuePoolSmart(unit).GetValues<Unit>();
            Debug.Log("Units node:" + connections.GetNode(NodeBlueprint.ConnectionClass.ReciveUnit));
            Debug.Log("Units Length: " + units.Length);
            foreach (var unitIndex in units)
            {
                
                if (fields[0] == "Root")
                {
                    valuePool.AddValue(unitIndex.gameObject);
                    valuePool.AddValue(unitIndex.data.torso.GetComponent<Rigidbody>());
                }
                if (fields[0] == "Torso")
                {
                    valuePool.AddValue(unitIndex.data.torso.gameObject);
                    valuePool.AddValue(unitIndex.data.torso.GetComponent<Rigidbody>());
                }
                if (fields[0] == "Hip")
                {
                    valuePool.AddValue(unitIndex.data.hip.gameObject);
                    valuePool.AddValue(unitIndex.data.hip.GetComponent<Rigidbody>());
                }
                if (fields[0] == "Head")
                {
                    valuePool.AddValue(unitIndex.data.head.gameObject);
                    valuePool.AddValue(unitIndex.data.head.GetComponent<Rigidbody>());
                }
                if (fields[0] == "Right Arm")
                {
                    valuePool.AddValue(unitIndex.data.rightArm.gameObject);
                    valuePool.AddValue(unitIndex.data.rightArm.GetComponent<Rigidbody>());
                }
                if (fields[0] == "Left Arm")
                {
                    valuePool.AddValue(unitIndex.data.leftArm.gameObject);
                    valuePool.AddValue(unitIndex.data.leftArm.GetComponent<Rigidbody>());
                }
                if (fields[0] == "Right Hand")
                {
                    valuePool.AddValue(unitIndex.data.rightHand.gameObject);
                    valuePool.AddValue(unitIndex.data.rightHand.GetComponent<Rigidbody>());
                }
                if (fields[0] == "Left Hand")
                {
                    valuePool.AddValue(unitIndex.data.leftHand.gameObject);
                    valuePool.AddValue(unitIndex.data.leftHand.GetComponent<Rigidbody>());
                }
                if (fields[0] == "Right Knee")
                {
                    valuePool.AddValue(unitIndex.data.legRight.gameObject);
                    valuePool.AddValue(unitIndex.data.legRight.GetComponent<Rigidbody>());
                }
                if (fields[0] == "Left Knee")
                {
                    valuePool.AddValue(unitIndex.data.legLeft.gameObject);
                    valuePool.AddValue(unitIndex.data.legLeft.GetComponent<Rigidbody>());
                }
                if (fields[0] == "Right Foot")
                {
                    valuePool.AddValue(unitIndex.data.footRight.gameObject);
                    valuePool.AddValue(unitIndex.data.footRight.GetComponent<Rigidbody>());
                }
                if (fields[0] == "Left Foot")
                {
                    valuePool.AddValue(unitIndex.data.footLeft.gameObject);
                    valuePool.AddValue(unitIndex.data.footLeft.GetComponent<Rigidbody>());
                }
                if (fields[0] == "Mesh")
                {
                    SkinnedMeshRenderer meshReference = unitIndex.unitBlueprint.UnitBase.GetComponentInChildren<SkinnedMeshRenderer>();
                    GameObject mesh = unitIndex.GetComponentsInChildren<SkinnedMeshRenderer>().First(n => n.sharedMesh == meshReference.sharedMesh).transform.parent.gameObject;
                    valuePool.AddValue(mesh) ;
                }
                if (fields[0] == "Both Foots")
                {
                    valuePool.AddValue(unitIndex.data.footLeft.gameObject);
                    valuePool.AddValue(unitIndex.data.footLeft.GetComponent<Rigidbody>());
                    valuePool.AddValue(unitIndex.data.footRight.gameObject);
                    valuePool.AddValue(unitIndex.data.footRight.GetComponent<Rigidbody>());
                }
                if (fields[0] == "Both Hands")
                {
                    valuePool.AddValue(unitIndex.data.rightHand.gameObject);
                    valuePool.AddValue(unitIndex.data.leftHand.GetComponent<Rigidbody>());
                    valuePool.AddValue(unitIndex.data.rightHand.gameObject);
                    valuePool.AddValue(unitIndex.data.leftHand.GetComponent<Rigidbody>());
                }
                if (fields[0] == "All")
                {
                    valuePool.AddValue(unitIndex.data.torso.gameObject);
                    valuePool.AddValue(unitIndex.data.torso.GetComponent<Rigidbody>());
                    valuePool.AddValue(unitIndex.data.head.gameObject);
                    valuePool.AddValue(unitIndex.data.head.GetComponent<Rigidbody>());
                    valuePool.AddValue(unitIndex.data.rightArm.gameObject);
                    valuePool.AddValue(unitIndex.data.rightArm.GetComponent<Rigidbody>());
                    valuePool.AddValue(unitIndex.data.leftArm.gameObject);
                    valuePool.AddValue(unitIndex.data.leftArm.GetComponent<Rigidbody>());
                    valuePool.AddValue(unitIndex.data.rightHand.gameObject);
                    valuePool.AddValue(unitIndex.data.rightHand.GetComponent<Rigidbody>());
                    valuePool.AddValue(unitIndex.data.leftHand.gameObject);
                    valuePool.AddValue(unitIndex.data.leftHand.GetComponent<Rigidbody>());
                    valuePool.AddValue(unitIndex.data.legRight.gameObject);
                    valuePool.AddValue(unitIndex.data.legRight.GetComponent<Rigidbody>());
                    valuePool.AddValue(unitIndex.data.legLeft.gameObject);
                    valuePool.AddValue(unitIndex.data.legLeft.GetComponent<Rigidbody>());
                    valuePool.AddValue(unitIndex.data.footRight.gameObject);
                    valuePool.AddValue(unitIndex.data.footRight.GetComponent<Rigidbody>());
                    valuePool.AddValue(unitIndex.data.footLeft.gameObject);
                    valuePool.AddValue(unitIndex.data.footLeft.GetComponent<Rigidbody>());
                    valuePool.AddValue(unitIndex.data.hip.gameObject);
                    valuePool.AddValue(unitIndex.data.hip.GetComponent<Rigidbody>());
                }
            }
            return valuePool;
        }
        public override bool IsDynamic()
        {
            return true;
        }
        public override ValuePool GetValuePool(LegacySavedNode savedNode, Unit unit, List<NodeComponent.LegacyConnection> connections, string[] fields)
        {
            return null;
        }
    }
}