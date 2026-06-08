using Landfall.TABS;
using Landfall.TABS.GameMode;
using Landfall.TABS.WinConditions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IDK.NodeScripts
{
    public class EndBattle : IBehaviorNode
    {
        public override IEnumerator RunNode(LegacySavedNode savedNode, Unit unit, List<NodeComponent.LegacyConnection> connections, string[] fields, NodeRunner nodeRunner)
        {
            Debug.Log("Ending battle!");
            if (fields[0] == "Win")
            {
                try
                {
                    
                    var service = ServiceLocator.GetService<GameModeService>();
                    Debug.Log("Service: " + service.ToString());
                    var str = service.CurrentGameMode.WinConditionPropagator.GetWinConditionsForTeam(unit.Team)[0].GetWinDescription();
                    Debug.Log("str:" + str);
                    service.CurrentGameMode.DecideWinner(unit.Team, str);
                }
                catch (System.Exception)
                {
                    if (unit.Team == Team.Red)
                    {
                        var service = ServiceLocator.GetService<GameModeService>();
                        var str = "<color=#FFFFFF>Defeated <color=#EE6E66>Red <color=#FFFFFF>Team";
                        service.CurrentGameMode.DecideWinner(unit.Team, str);
                    }
                    else
                    {
                        var str = "<color=#FFFFFF>Defeated <color=#EE6E66>Red <color=#FFFFFF>Team";
                        var service = ServiceLocator.GetService<GameModeService>();
                        service.CurrentGameMode.DecideWinner(unit.Team, str);
                    }
                }
                
            }
            else
            {
                try
                {
                    var service = ServiceLocator.GetService<GameModeService>();
                    var str = service.CurrentGameMode.WinConditionPropagator.GetWinConditionsForTeam(TeamUtlity.GetOtherTeam(unit.Team))[0].GetWinDescription();
                    service.CurrentGameMode.DecideWinner(unit.Team, str);
                }
                catch
                {
                    if (unit.Team == Team.Blue)
                    {
                        
                        var service = ServiceLocator.GetService<GameModeService>();
                        var str = "<color=#FFFFFF>Defeated <color=#EE6E66>Red <color=#FFFFFF>Team";
                        service.CurrentGameMode.DecideWinner(unit.Team, str);
                    }
                    else
                    {
                        var str = "<color=#FFFFFF>Defeated <color=#EE6E66>Red <color=#FFFFFF>Team";
                        var service = ServiceLocator.GetService<GameModeService>();
                        service.CurrentGameMode.DecideWinner(unit.Team, str);
                    }
                }
            }
            yield return savedNode.TriggerConnection(nodeRunner);
        }
    }
}