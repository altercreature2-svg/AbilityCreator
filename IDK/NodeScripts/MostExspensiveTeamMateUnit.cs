using AC.Node_Related_Scripts.NodeRunning;
using AC.Node_Related_Scripts.NodeRunning.Instructions.Courtines;
using Landfall.TABS;
using Landfall.TABS.AI.Systems;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using UnityEngine;

namespace AC.NodeScripts
{
    public class MostExspensiveTeamMateUnit : IValueNode
    {
        public IEnumerator<CoroutineReturn> Execute(NodeEnv env)
        {
            env.ClearValue(NodeBlueprint.ConnectionClass.GiveUnit);
            var unitsEnum = env.GetValues(NodeBlueprint.ConnectionClass.ReciveUnit);
            foreach (var item in unitsEnum)
            {
                if (!(item.value is Unit u))
                    continue;
                env.AddValue(NodeBlueprint.ConnectionClass.GiveUnit, GetMostExpensiveUnit(env.unit, env.unit.Team));
            }
            yield return new CoroutineReturn(CoroutineReturn.CourtineType.ContinueBranch);
        }
        public IEnumerator<CoroutineReturn> Cache(NodeEnv env)
        {
            return null;
        }
        public Unit GetMostExpensiveUnit(Unit meUnit, Team team)
        {
            var allUnits = World.Active.GetOrCreateManager<TeamSystem>().GetTeamUnits(team);
            float closest = 999;
            Unit closestUnit = null;
            Unit me = meUnit;
            foreach (var unit in allUnits)
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

    }
}