using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IDK.NodeScripts
{
    public class EnemyNode : IValueNode
    {
        public override bool IsDynamic()
        {
            return true;
        }
        public override ValuePool GetDynamicValue(SavedNode savedNode, Unit unit, List<Node.Connection> connections, string[] fields)
        {
            ValuePool valuePool = new ValuePool();
            valuePool.AddValue(unit.data.targetData.unit);
            return valuePool;
        }
        public override ValuePool GetValuePool(SavedNode savedNode, Unit unit, List<Node.Connection> connections, string[] fields)
        {
            return null;
        }

    }
}