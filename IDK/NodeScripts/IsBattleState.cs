using AC.Node_Related_Scripts.NodeRunning;
using AC.Node_Related_Scripts.NodeRunning.Instructions.Courtines;
using Landfall.TABS.GameState;
using System.Collections.Generic;

namespace AC.NodeScripts
{
    public class IsBattleState : IBehaviorNode
    {
        GameStateManager service;
        public IEnumerator<CoroutineReturn> Execute(NodeEnv env)
        {
            if (!(service.GameState is GameState.BattleState)) yield break;
            yield return new CoroutineReturn(CoroutineReturn.CourtineType.ContinueBranch);
        }
        public IEnumerator<CoroutineReturn> Cache(NodeEnv env)
        {
            service = ServiceLocator.GetService<GameStateManager>();
            return null;
        }
    }
}