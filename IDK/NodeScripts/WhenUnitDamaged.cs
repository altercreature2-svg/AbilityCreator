using AC.Node_Related_Scripts;
using AC.Node_Related_Scripts.NodeRunning;
using AC.Node_Related_Scripts.NodeRunning.Instructions.Courtines;
using Landfall.TABS;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace AC.NodeScripts
{
    public class WhenUnitDamaged : ITriggerNode
    {
        public IEnumerator<CoroutineReturn> Execute(NodeEnv env)
        {
            return null;
        }
        public IEnumerator<CoroutineReturn> Cache(NodeEnv env)
        {
            Action<float> action = (Action<float>)env.unit.GetField("WasDealtDamageAction");
            Action<float> action2 = (float f) => OnDamage(env, f);
            env.unit.SetField("WasDealtDamageAction", Delegate.Combine(action, action2));
            return null;
        }
        public void OnDamage(NodeEnv env, float damage)
        {
            env.AddValue(NodeBlueprint.ConnectionClass.GiveUnit, env.unit.data.targetData);
            env.AddValue(NodeBlueprint.ConnectionClass.GiveVariable, new Variable() { value = damage});
        }
    }

}