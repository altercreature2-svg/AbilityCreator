using Landfall.TABS;
using Landfall.TABS.AI.Systems;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using UnityEngine;

namespace IDK.NodeScripts
{
    public class MostExspensiveEnemy : IValueNode
    {
        public override bool IsDynamic()
        {
            return true;
        }
        public override ValuePool GetDynamicValue(LegacySavedNode savedNode, Unit unit, List<NodeComponent.LegacyConnection> connections, string[] fields)
        {
            ValuePool valuePool = new ValuePool();
            Unit[] units = connections.GetNode(NodeBlueprint.ConnectionClass.ReciveUnit).GetValuePoolSmart(unit).GetValues<Unit>();
            Debug.Log("Units Length:" + units.Length);
            for (int i = 0; i < units.Length; i++)
            {
                Unit expensiveUnit = GetMostExpensiveUnit(units[i], units[i].Team);
                valuePool.AddValue(expensiveUnit);
            }
            return valuePool;
        }
        public Unit GetMostExpensiveUnit(Unit meUnit, Team team)
        {
            var allUnits = World.Active.GetOrCreateManager<TeamSystem>().GetAllUnits().Where(n => n.Team == TeamUtlity.GetOtherTeam(team));
            float closest = 999;
            Unit closestUnit = null;
            Unit me = meUnit;
            foreach (var unit  in allUnits)
            {
                if (unit == me)
                    continue;
                float price = unit.unitBlueprint.GetUnitCost(true);
                if (price < closest)
                {
                    Debug.Log("Closest So Far! " + unit.unitBlueprint.Entity.Name);
                    closest = price;
                    closestUnit = unit;
                }
            }


            if (closestUnit == null)
            {
                closestUnit = me;
            }
            Debug.Log($"Found closest meUnit! ({closestUnit.unitBlueprint.Entity.Name})");
            return closestUnit;
        }
        public override ValuePool GetValuePool(LegacySavedNode savedNode, Unit unit, List<NodeComponent.LegacyConnection> connections, string[] fields)
        {
            return null;
        }

    }
}