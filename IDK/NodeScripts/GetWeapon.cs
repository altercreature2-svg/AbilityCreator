using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IDK.NodeScripts
{
    public class GetWeapon : IValueNode
    {
        public override ValuePool GetDynamicValue(SavedNode savedNode, Unit unit, List<Node.Connection> connections, string[] fields)
        {
            ValuePool valuePool = new ValuePool();
            Unit[] units = connections.GetNode(NodeBlueprint.ConnectionType.ReciveUnit).GetValuePoolSmart(unit).GetValues<Unit>();
            Debug.Log("Units node:" + connections.GetNode(NodeBlueprint.ConnectionType.ReciveUnit));
            Debug.Log("Units Length: " + units.Length);
            foreach (var unitIndex in units)
            {
                
                if (fields[0] == "Both")
                {
                    valuePool.AddValue(unitIndex.data?.weaponHandler?.rightWeapon?.gameObject);
                    valuePool.AddValue(unitIndex.data?.weaponHandler?.leftWeapon?.gameObject);
                    valuePool.AddValue(unitIndex.data?.weaponHandler?.rightWeapon?.GetComponent<Rigidbody>());
                    valuePool.AddValue(unitIndex.data?.weaponHandler?.leftWeapon?.GetComponent<Rigidbody>());
                }
                if (fields[0] == "Left Weapon")
                {
                    valuePool.AddValue(unitIndex.data?.weaponHandler?.leftWeapon?.gameObject);
                    valuePool.AddValue(unitIndex.data?.weaponHandler?.leftWeapon?.GetComponent<Rigidbody>());
                }
                if (fields[0] == "Right Weapon")
                {
                    valuePool.AddValue(unitIndex.data?.weaponHandler?.rightWeapon?.gameObject);
                    valuePool.AddValue(unitIndex.data?.weaponHandler?.rightWeapon?.GetComponent<Rigidbody>());
                }
            }
            return valuePool;
        }
        public override bool IsDynamic()
        {
            return true;
        }
        public override ValuePool GetValuePool(SavedNode savedNode, Unit unit, List<Node.Connection> connections, string[] fields)
        {
            return null;
        }
    }
}