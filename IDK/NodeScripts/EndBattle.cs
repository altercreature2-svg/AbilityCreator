using AC.Node_Related_Scripts.NodeRunning;
using AC.Node_Related_Scripts.NodeRunning.Instructions.Courtines;
using Landfall.TABS;
using Landfall.TABS.GameMode;
using Landfall.TABS.WinConditions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AC.NodeScripts
{
    public class EndBattle : IBehaviorNode
    {
        public IEnumerator<CoroutineReturn> Execute(NodeEnv env)
        {
            Debug.Log("Ending battle!");
            if (env.GetField(0) == "Win")
            {
                try
                {
                    var service = ServiceLocator.GetService<GameModeService>();
                    Debug.Log("Service: " + service.ToString());
                    var str = service.CurrentGameMode.WinConditionPropagator.GetWinConditionsForTeam(env.unit.Team)[0].GetWinDescription();
                    Debug.Log("str:" + str);
                    service.CurrentGameMode.DecideWinner(env.unit.Team, str);
                }
                catch (System.Exception)
                {
                    if (env.unit.Team == Team.Red)
                    {
                        var service = ServiceLocator.GetService<GameModeService>();
                        var str = "<color=#FFFFFF>Defeated <color=#EE6E66>Red <color=#FFFFFF>Team";
                        service.CurrentGameMode.DecideWinner(env.unit.Team, str);
                    }
                    else
                    {
                        var str = "<color=#FFFFFF>Defeated <color=#EE6E66>Red <color=#FFFFFF>Team";
                        var service = ServiceLocator.GetService<GameModeService>();
                        service.CurrentGameMode.DecideWinner(env.unit.Team, str);
                    }
                }
                
            }
            else
            {
                try
                {
                    var service = ServiceLocator.GetService<GameModeService>();
                    var str = service.CurrentGameMode.WinConditionPropagator.GetWinConditionsForTeam(TeamUtlity.GetOtherTeam(env.unit.Team))[0].GetWinDescription();
                    service.CurrentGameMode.DecideWinner(env.unit.Team, str);
                }
                catch
                {
                    if (env.unit.Team == Team.Blue)
                    {
                        
                        var service = ServiceLocator.GetService<GameModeService>();
                        var str = "<color=#FFFFFF>Defeated <color=#EE6E66>Red <color=#FFFFFF>Team";
                        service.CurrentGameMode.DecideWinner(env.unit.Team, str);
                    }
                    else
                    {
                        var str = "<color=#FFFFFF>Defeated <color=#EE6E66>Red <color=#FFFFFF>Team";
                        var service = ServiceLocator.GetService<GameModeService>();
                        service.CurrentGameMode.DecideWinner(env.unit.Team, str);
                    }
                }
            }
            yield return new CoroutineReturn(CoroutineReturn.CourtineType.ContinueBranch);
        }
        public IEnumerator<CoroutineReturn> Cache(NodeEnv env)
        {
            return null;
        }
    }
}