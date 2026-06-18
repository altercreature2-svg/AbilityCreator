using AC.Node_Related_Scripts.NodeRunning;
using AC.Node_Related_Scripts.NodeRunning.Instructions.Courtines;
using Landfall.TABS;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AC.NodeScripts
{
    public class WhenUnitDies : ITriggerNode
    {
        public IEnumerator<CoroutineReturn> Execute(NodeEnv env)
        {
            return null;
        }
        public IEnumerator<CoroutineReturn> Cache(NodeEnv env)
        {
            Action action = (Action)Delegate.Combine((Action)(() => env.RunTrigger()), (Action)env.unit.data.healthHandler.GetField("DieAction"));
            env.unit.data.healthHandler.SetField("DieAction", action);
            return null;
        }
    }
    
}