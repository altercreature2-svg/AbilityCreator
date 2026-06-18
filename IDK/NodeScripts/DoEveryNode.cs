using AC.Help_Componets;
using AC.Node_Related_Scripts.NodeRunning;
using AC.Node_Related_Scripts.NodeRunning.Instructions.Courtines;
using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AC.NodeScripts
{
    public class DoEveryNode : ITriggerNode
    {
        public IEnumerator<CoroutineReturn> Cache(NodeEnv env)
        {
            env.runner.StartCoroutine(IntervalLoop(env, env.GetField(0).QuickParse()));
            yield break;
        }
        public IEnumerator<CoroutineReturn> Execute(NodeEnv env)
        {
            return null;
        }
        public IEnumerator IntervalLoop(NodeEnv env, float seconds)
        {
            while (true)
            {
                yield return new WaitForSeconds(seconds);
                env.RunTrigger();
            }
        }

    }
    
}