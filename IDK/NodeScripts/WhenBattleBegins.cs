using AC.Node_Related_Scripts.NodeRunning;
using AC.Node_Related_Scripts.NodeRunning.Instructions.Courtines;
using System.Collections.Generic;

namespace AC.NodeScripts
{
    public class WhenBattleBegins : ITriggerNode
    {
        public IEnumerator<CoroutineReturn> Execute(NodeEnv env)
        {
            return null;
        }
        public IEnumerator<CoroutineReturn> Cache(NodeEnv env)
        {
            yield return new CoroutineReturn(CoroutineReturn.CourtineType.WaitUntil, arg0:0, arg1: () => env.unit.data?.targetData); // cause it'll only get the data once the battle begins
            env.RunTrigger();
        }
    }
}