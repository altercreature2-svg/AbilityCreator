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
            return true;
        }
        public override ValuePool GetDynamicValue(LegacySavedNode savedNode, Unit unit, List<NodeComponent.LegacyConnection> connections, string[] fields)
        {
            Unit[] units = connections.GetNode(NodeBlueprint.ConnectionClass.ReciveUnit).GetValuePoolSmart(unit).GetValues<Unit>();
            ValuePool valuePool = savedNode.GetValuePool(unit);
            for (int i = 0; i < units.Length; i++)
            {
                if (fields[0] == "Normal")
                    valuePool.AddValue(new Variable() { value = units[i].data.health });
                else
                {
                    float preune = units[i].data.health / units[i].data.maxHealth;
                    valuePool.AddValue(new Variable() { value = Mathf.Clamp01(preune) * 100 });
                }
            }

            return valuePool;
        }
        public override ValuePool GetValuePool(LegacySavedNode savedNode, Unit unit, List<NodeComponent.LegacyConnection> connections, string[] fields)
        {
            return null;
        }
    }
}