using AC.Node_Related_Scripts;
using AC.Node_Related_Scripts.NodeRunning;
using AC.Node_Related_Scripts.NodeRunning.Instructions.Courtines;
using Landfall.TABS;
using System.Collections.Generic;
using UnityEngine;

namespace AC.NodeScripts
{
    public class RandomNode : IValueNode
    {
        int value, min, max;
        bool hasChosen, isStatic;
        public IEnumerator<CoroutineReturn> Execute(NodeEnv env)
        {
            env.ClearValue(NodeBlueprint.ConnectionClass.GiveVariable);
            if (isStatic && !hasChosen)
            {
                value = Random.Range(min, max + 1);
                hasChosen = true;
            }
            else
                value = Random.Range(min, max + 1);
            env.AddValue(NodeBlueprint.ConnectionClass.GiveVariable, new Variable() { value = value });
            return null;
        }
        public IEnumerator<CoroutineReturn> Cache(NodeEnv env)
        {
            min = env.GetField(0).QuickParseInt();
            max = env.GetField(1).QuickParseInt();
            isStatic = env.GetField(0) == "Static";
            return null;
        }
    }
}