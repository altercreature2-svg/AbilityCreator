using DM;
using HarmonyLib;
using AC.Node_Related_Scripts;
using AC.Node_Related_Scripts.ConnectionStuff;
using AC.Node_Related_Scripts.NodeRunning;
using AC.Node_Related_Scripts.NodeRunning.Instructions.Courtines;
using Landfall.TABS;
using Landfall.TABS.UnitEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using static AC.Node_Related_Scripts.NodeRunning.Instructions.Courtines.CoroutineReturn;
using IDK.Help;
namespace AC.NodeScripts
{
    public class AddAbility : IBehaviorNode
    {
        public IEnumerator<CoroutineReturn> Execute(NodeEnv env)
        {
            var units = env.GetValues(NodeBlueprint.ConnectionClass.ReciveUnit);
            var abilties = env.GetValues(NodeBlueprint.ConnectionClass.ReciveGameObject);
            foreach (var item in units)
            {
                if (!(item.value is Unit unit))
                    continue;
                foreach (var item2 in abilties)
                {
                    if (!(item2.value is GameObject ability))
                        continue;
                    GameObject abiltiy = Object.Instantiate(ability, unit.api.GetHipPosition(), Quaternion.identity, unit.transform);
                    FastTeamColor.SetTeamColors(abiltiy, unit.Team);
                    env.AddValue(NodeBlueprint.ConnectionClass.GiveGameObject, abiltiy);
                }
            }
            
            yield return new CoroutineReturn(CourtineType.ContinueBranch);
        }
        public IEnumerator<CoroutineReturn> Cache(NodeEnv env)
        {
            return null;
        }

    }
}