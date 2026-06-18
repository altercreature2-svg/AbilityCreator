using AC.Node_Related_Scripts.NodeRunning;
using AC.Node_Related_Scripts.NodeRunning.Instructions.Courtines;
using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AC.NodeScripts
{
    public class FilterNode : IValueNode
    {
        public IEnumerator<CoroutineReturn> Cache(NodeEnv env)
        {
            return null;
        }
        public IEnumerator<CoroutineReturn> Execute(NodeEnv env)
        {
            string option = env.GetField(0);
            var everything = env.GetValues(NodeBlueprint.ConnectionClass.ReciveAnything);
            foreach (var item in everything)
            {
                if (item.value == null)
                    continue;
                if (option == "Gameobjects only")
                    if (item.value is GameObject converted)
                        env.AddValue(NodeBlueprint.ConnectionClass.GiveAnything, converted);
                if (option == "Units only")
                    if (item.value is Unit converted)
                        env.AddValue(NodeBlueprint.ConnectionClass.GiveAnything, converted);
                if (option == "Components only")
                    if (item.value is Component converted)
                        env.AddValue(NodeBlueprint.ConnectionClass.GiveAnything, converted);
                if (option == "Other")
                    if (!((item.value is GameObject) | (item.value is Unit) | (item.value is Component)))
                        env.AddValue(NodeBlueprint.ConnectionClass.GiveAnything, item.value);
            }
            
            yield return new CoroutineReturn(CoroutineReturn.CourtineType.ContinueBranch);
        }

    }
}