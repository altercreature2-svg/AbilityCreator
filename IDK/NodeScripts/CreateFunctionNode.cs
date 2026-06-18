

using AC.Help_Componets;
using AC.Node_Related_Scripts.NodeRunning;
using AC.Node_Related_Scripts.NodeRunning.Instructions.Courtines;
using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace AC.NodeScripts
{
    public class CreateFunctionNode : ITriggerNode
    {
        public IEnumerator<CoroutineReturn> Cache(NodeEnv env)
        {
            FunctionStorer functionStorer = env.cacheSystem.GetCachedComponent<FunctionStorer>(env.unit.gameObject);
            if (!functionStorer)
                functionStorer = env.unit.gameObject.AddComponent<FunctionStorer>();
            FunctionStorer.NodeAction action = functionStorer.actions.Find(n => n.name == env.GetField(0));
            if (action == null)
            {
                action = new FunctionStorer.NodeAction() { name = env.GetField(0), @event = new UnityEngine.Events.UnityEvent() };
                functionStorer.actions.Add(action);
            }
            action?.@event?.AddListener(env.RunTrigger);
            yield break;
        }
        public IEnumerator<CoroutineReturn> Execute(NodeEnv env)
        {
            return null;
        }
    }
}