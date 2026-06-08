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
        public override ValuePool GetDynamicValue(LegacySavedNode savedNode, Unit unit, List<NodeComponent.LegacyConnection> connections, string[] fields)
        {
            return null;
        }
        public override ValuePool GetValuePool(LegacySavedNode savedNode, Unit unit, List<NodeComponent.LegacyConnection> connections, string[] fields)
        {
            Unit[] units = connections.GetNode(NodeBlueprint.ConnectionClass.ReciveUnit).GetValuePoolSmart(unit).GetValues<Unit>();
            ValuePool valuePool = savedNode.GetValuePool(unit);
            for (int i = 0; i < units.Length; i++)
            {
                valuePool.AddValue(new Variable() { value = units[i].data.maxHealth });
            }

            return valuePool;
        }
    }
}