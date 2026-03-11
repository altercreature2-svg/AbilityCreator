using Landfall.TABS;
using System.Collections.Generic;

namespace IDK.NodeScripts
{
    public class LeftOrRight : IValueNode
    {
        public override ValuePool GetValuePool(SavedNode savedNode, Unit unit, List<Node.Connection> connections, string[] fields)
        {
            ValuePool valuePool = savedNode.GetValuePool(unit);
            BundleManager.LeftRight leftRight = BundleManager.LeftRight.Right;
            if (fields[0] == "Left")
                leftRight = BundleManager.LeftRight.Left;
            valuePool.ClearValues();
            valuePool.AddValue(leftRight);
            return valuePool;
        }
        public override bool IsDynamic()
        {
            return false;
        }
        public override ValuePool GetDynamicValue(SavedNode savedNode, Unit unit, List<Node.Connection> connections, string[] fields)
        {
            return null;
        }
    }

}