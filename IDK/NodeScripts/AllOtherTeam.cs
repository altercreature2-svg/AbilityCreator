using Landfall.TABS;
using Landfall.TABS.AI.Systems;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using UnityEngine;

namespace IDK.NodeScripts
{
    public class AllOtherTeam : IValueNode
    {
        public override bool IsDynamic()
        {
            return true;
        }
        public override ValuePool GetDynamicValue(LegacySavedNode savedNode, Unit unit, List<NodeComponent.LegacyConnection> connections, string[] fields)
        {
            ValuePool valuePool = new ValuePool();
            List<Unit> units = World.Active.GetOrCreateManager<TeamSystem>().GetTeamUnits(unit.Team.Reverse());
            for (int i = 0; i < units.Count; i++)
            {
                valuePool.AddValue(units[i]);
            }
            return valuePool;
        }
        public override ValuePool GetValuePool(LegacySavedNode savedNode, Unit unit, List<NodeComponent.LegacyConnection> connections, string[] fields)
        {
            return null;
        }

    }
}