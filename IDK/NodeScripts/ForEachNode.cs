using AC.Help;
using AC.Node_Related_Scripts.NodeRunning;
using AC.Node_Related_Scripts.NodeRunning.Instructions.Courtines;
using Landfall.TABS;
using System.Collections.Generic;
using UnityEngine;

namespace AC.NodeScripts
{
    public class ForEachNode : IBehaviorNode
    {

        public IEnumerator<CoroutineReturn> Cache(NodeEnv env)
        {
            return null;
        }
        public IEnumerator<CoroutineReturn> Execute(NodeEnv env)
        {
            var objects = env.GetValues(NodeBlueprint.ConnectionClass.ReciveAnything);
            env.ClearValue(NodeBlueprint.ConnectionClass.GiveAnything);
            string option = env.GetField(0);
            System.Type filterType;
            switch (option)
            {
                case "All":
                    filterType = null;
                    break;
                case "Gameobjects only":
                    filterType = typeof(GameObject);
                    break;
                case "Units only":
                    filterType = typeof(Unit);
                    break;
                default:
                    filterType = typeof(Component);
                    break;
            }
            FixedPool<object> pool = new FixedPool<object>(objects.Length);
            foreach (var item in objects)
            {
                object typed = item.value;
                if (typed.GetType() != filterType)
                    continue;
                pool.Insert(item);
            }
            AbilityCreator.reapeter.AddTask(pool.Length, env.GetField(1).QuickParse(),i => Run(env, i, pool));
            yield return new CoroutineReturn(CoroutineReturn.CourtineType.ContinueBranch);
        }
        public void Run(NodeEnv env, int i, FixedPool<object> pool)
        {
            env.ClearValue(NodeBlueprint.ConnectionClass.GiveAnything);
            env.AddValue(NodeBlueprint.ConnectionClass.GiveAnything, pool[i]);
            env.RunTrigger();
        }
    }
}