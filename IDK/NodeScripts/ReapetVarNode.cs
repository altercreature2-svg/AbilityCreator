using AC.Node_Related_Scripts;
using AC.Node_Related_Scripts.NodeRunning;
using AC.Node_Related_Scripts.NodeRunning.Instructions.Courtines;
using System.Collections.Generic;

namespace AC.NodeScripts
{
    public class ReapetVarNode : IBehaviorNode
    {
        float interval;
        public IEnumerator<CoroutineReturn> Execute(NodeEnv env)
        {
            var variablesEnum = env.GetValues(NodeBlueprint.ConnectionClass.ReciveVariable);
            int ret = 0;
            foreach (var item in variablesEnum)
            {
                if (!(item.value is Variable v))
                    continue;
                ret = (int)v.value;
                break; // squinting black guy holding paper gif
            }
            AbilityCreator.reapeter.AddTask(ret, interval, i => env.RunTrigger());
            yield return new CoroutineReturn(CoroutineReturn.CourtineType.ContinueBranch);
        }
        public IEnumerator<CoroutineReturn> Cache(NodeEnv env)
        {
            interval = env.GetField(1).QuickParse();
            return null;
        }
    }
}