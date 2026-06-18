

using AC.Node_Related_Scripts.NodeRunning;
using AC.Node_Related_Scripts.NodeRunning.Instructions.Courtines;
using InControl.NativeDeviceProfiles;
using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.ParticleSystemJobs;

namespace AC.NodeScripts
{
    public class InvokeMethodNode : IBehaviorNode
    {
        public IEnumerator<CoroutineReturn> Execute(NodeEnv env)
        {
            env.ClearValue(NodeBlueprint.ConnectionClass.GiveAnything);
            var anythingEnum = env.GetValues(NodeBlueprint.ConnectionClass.ReciveAnything);
            string name = env.GetField(0);
            foreach (var eachthing in anythingEnum )
            {
                if (eachthing.value == null)
                    continue;
                MethodInfo methodInfo = eachthing.value.GetType().GetMethod(name, (BindingFlags)(-1));
                if (methodInfo.GetParameters().Length != 0)
                    continue;
                env.AddValue(NodeBlueprint.ConnectionClass.GiveAnything, methodInfo.Invoke(eachthing.value, System.Array.Empty<object>()));
            }
            yield return new CoroutineReturn(CoroutineReturn.CourtineType.ContinueBranch);
        }
        public IEnumerator<CoroutineReturn> Cache(NodeEnv env)
        {
            return null;
        }
    }
}