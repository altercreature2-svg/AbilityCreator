using IDK.Node_Related_Scripts;
using Landfall.TABS;
using System.Collections.Generic;

namespace IDK.NodeScripts
{
    public class GetMaxUnitHealth : IValueNode
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
                valuePool.AddValue(new Variable() { value = units[i].data.maxHealth });
            }

            return valuePool;
        }
    }
}