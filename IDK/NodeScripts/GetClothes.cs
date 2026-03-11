using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace IDK.NodeScripts
{
    public class GetClothes : IValueNode
    {
        public override ValuePool GetDynamicValue(SavedNode savedNode, Unit unit, List<Node.Connection> connections, string[] fields)
        {
            
            ValuePool valuePool = new ValuePool();
            Unit[] units = connections.GetNode(NodeBlueprint.ConnectionType.ReciveUnit).GetValuePoolSmart(unit).GetValues<Unit>();
            Debug.Log("Units Length: " + units.Length);
            foreach (var unitIndex in units)
            {
                
                if (fields[0] == "All")
                {
                    valuePool.AddRange(unitIndex.GetComponentsInChildren<PropItem>().Select(n => n.gameObject).ToArray());
                }
                if (fields[0] == "Head")
                {
                    valuePool.AddRange(unitIndex.GetComponentsInChildren<PropItem>().Where(n => (n.GearT == UnitRig.GearType.HEAD) | (n.GearT == UnitRig.GearType.NECK)).Select(n => n.gameObject).ToArray());
                }
                if (fields[0] == "Torso")
                {
                    valuePool.AddRange(unitIndex.GetComponentsInChildren<PropItem>().Where(n => (n.GearT == UnitRig.GearType.TORSO) | (n.GearT == UnitRig.GearType.WAIST)).Select(n => n.gameObject).ToArray());
                }
                if (fields[0] == "Arms")
                {
                    valuePool.AddRange(unitIndex.GetComponentsInChildren<PropItem>().Where(n => (n.GearT == UnitRig.GearType.ARMS) | (n.GearT == UnitRig.GearType.SHOULDER) | (n.GearT == UnitRig.GearType.HANDS) | (n.GearT == UnitRig.GearType.WRISTS)).Select(n => n).Select(n => n.gameObject).ToArray());
                }
                if (fields[0] == "Pants")
                {
                    valuePool.AddRange(unitIndex.GetComponentsInChildren<PropItem>().Where(n => (n.GearT == UnitRig.GearType.LEGS)).Select(n => n.gameObject).ToArray());
                }
                if (fields[0] == "Shoes")
                {
                    valuePool.AddRange(unitIndex.GetComponentsInChildren<PropItem>().Where(n => (n.GearT == UnitRig.GearType.FEET)).Select(n => n.gameObject).ToArray());
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