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
    public class ClosestTeamMateUnit : IValueNode
    {
        public IEnumerator<CoroutineReturn> Execute(NodeEnv env)
        {
            
            var unitsEnum = env.GetValues(NodeBlueprint.ConnectionClass.ReciveUnit);
            foreach (var item in unitsEnum)
            {
                if (!(item.value is Unit u))
                    continue;
                env.AddValue(NodeBlueprint.ConnectionClass.GiveUnit, GetClosestUnit(env));
            }
            yield return new CoroutineReturn(CoroutineReturn.CourtineType.ContinueBranch);
        }
        public Unit GetClosestUnit(NodeEnv env)
        {
            
            var allUnits = World.Active.GetOrCreateManager<TeamSystem>().GetAllUnits().Where(n => n.Team == env.unit.Team);
            float closest = 999;
            Unit closestUnit = null;
            foreach (var unit  in allUnits)
            {
                if (unit == env.unit)
                    continue;
                float distance = Vector3.Distance(env.unit.data.mainRig.position, unit.data.mainRig.position);
                if (distance < closest)
                {
                    closest = distance;
                    closestUnit = unit;
                }
            }

            if (closestUnit == null)
                closestUnit = env.unit;
            
            return closestUnit;
        }
        public IEnumerator<CoroutineReturn> Cache(NodeEnv env)
        {
            return null;
        }

    }
}