using Landfall.TABS;
using Landfall.TABS.AI.Systems;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using UnityEngine;

namespace IDK.NodeScripts
{
    public class ClosestTeamMateUnit : IValueNode
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
                Unit ClosestUnit = GetClosestUnit(units[i].data.mainRig.transform, units[i].Team);
                valuePool.AddValue(ClosestUnit);
            }
            return valuePool;
        }
        public Unit GetClosestUnit(Transform mainRig, Team team)
        {
            var allUnits = World.Active.GetOrCreateManager<TeamSystem>().GetAllUnits().Where(n => n.Team == team);
            float closest = 999;
            Unit closestUnit = null;
            Unit me = mainRig.transform.root.GetComponent<Unit>();
            foreach (var unit  in allUnits)
            {
                if (unit == me)
                    continue;
                float distance = Vector3.Distance(mainRig.position, unit.data.mainRig.position);
                Debug.Log("distance:" + distance);
                if (distance < closest)
                {
                    Debug.Log("Closest So Far! " + unit.unitBlueprint.Entity.Name);
                    closest = distance;
                    closestUnit = unit;
                }
            }


            if (closestUnit == null)
            {
                closestUnit = me;
            }
            Debug.Log($"Found closest unit! ({closestUnit.unitBlueprint.Entity.Name})");
            return closestUnit;
        }
        public override ValuePool GetValuePool(LegacySavedNode savedNode, Unit unit, List<NodeComponent.LegacyConnection> connections, string[] fields)
        {
            return null;
        }

    }
}