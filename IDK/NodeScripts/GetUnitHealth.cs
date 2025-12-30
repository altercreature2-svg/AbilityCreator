using IDK.Node_Related_Scripts;
using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IDK.NodeScripts
{
    public class GetUnitHealth : IValueNode
    {
        public override bool IsDynamic()
        {
            return false;
        }
        public override ValuePool GetDynamicValue(SavedNode savedNode, Unit unit, List<Node.Connection> connections, string[] fields)
        {
            return null;
        }
        public override ValuePool GetValuePool(SavedNode savedNode, Unit unit, List<Node.Connection> connections, string[] fields)
        {
            Unit[] units = connections.GetNode(NodeBlueprint.ConnectionType.ReciveUnit).GetValuePoolSmart(unit).GetValues<Unit>();
            ValuePool valuePool = savedNode.GetValuePool(unit);
            for (int i = 0; i < units.Length; i++)
            {
                if (fields[0] == "Normal")
                    valuePool.AddValue(new Variable() { value = units[i].data.health });
                else
                    valuePool.AddValue(new Variable() { value = (int)((units[i].data.maxHealth/ units[i].data.health)*100) });
            }
            
            return valuePool;
        }
    }
}