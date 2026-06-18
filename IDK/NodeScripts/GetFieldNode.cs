

using AC.Node_Related_Scripts.NodeRunning;
using AC.Node_Related_Scripts.NodeRunning.Instructions.Courtines;
using Landfall.TABS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace AC.NodeScripts
{
    public class GetFieldNode : IValueNode
    {
        public IEnumerator<CoroutineReturn> Execute(NodeEnv env)
        {
            env.ClearValue(NodeBlueprint.ConnectionClass.GiveAnything);
            var objs = env.GetValues(NodeBlueprint.ConnectionClass.ReciveAnything);
            

            foreach (var item in objs)
            {
                if (!(item.value is object obj))
                    continue;
                object value = obj.GetField(env.GetField(0));

                if (value != null)
                {
                    var newValue = EnumerableHelper.ToArrayIfEnumerable(value);
                    if (newValue == null)
                        env.AddValue(NodeBlueprint.ConnectionClass.GiveAnything, value);
                    else
                       env.AddValues(NodeBlueprint.ConnectionClass.GiveAnything, newValue);       
                }
            }

            yield return new CoroutineReturn(CoroutineReturn.CourtineType.ContinueBranch);
        }
        public IEnumerator<CoroutineReturn> Cache(NodeEnv env)
        {
            return null;
        }
    }
}