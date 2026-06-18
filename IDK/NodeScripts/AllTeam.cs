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
    public class AllTeam : IValueNode
    {
        public IEnumerator<CoroutineReturn> Cache(NodeEnv env)
        {
            List<Unit> units = World.Active.GetOrCreateManager<TeamSystem>().GetTeamUnits(env.unit.Team);
            for (int i = 0; i < units.Count; i++)
            {
                env.AddValue(NodeBlueprint.ConnectionClass.GiveUnit, units[i]);
            }
            yield return new CoroutineReturn(CoroutineReturn.CourtineType.ContinueBranch);
        }
        public IEnumerator<CoroutineReturn> Execute(NodeEnv env)
        {
            return null;
        }
    }
}