using AC.Node_Related_Scripts.NodeRunning;
using AC.Node_Related_Scripts.NodeRunning.Instructions.Courtines;
using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AC.NodeScripts
{
    public class GetComponentNode : IValueNode
    {
        System.Type component;
        public IEnumerator<CoroutineReturn> Execute(NodeEnv env)
        {
            var objs = env.GetValues(NodeBlueprint.ConnectionClass.ReciveGameObject);
            env.ClearValue(NodeBlueprint.ConnectionClass.GiveComponent);
            string option1 = env.GetField(1);
            string option2 = env.GetField(2);
            bool getFirstOnly = option1 == "First";
            bool getComponentsInChildren = option2 != "Don't";
             
            foreach (var item in objs)
            {
                if (!(item.value is GameObject obj))
                    continue;

                switch (getFirstOnly)
                {
                    case true:
                        switch (getComponentsInChildren)
                        {
                            case true:
                                env.AddValue(NodeBlueprint.ConnectionClass.GiveComponent, 
                                    env.cacheSystem.GetCachedComponentInChildren(component, obj));
                                break;
                            case false:
                                env.AddValue(NodeBlueprint.ConnectionClass.GiveComponent,
                                    env.cacheSystem.GetCachedComponent(component, obj));
                                break;
                        }
                        break;
                    case false:
                        switch (getComponentsInChildren)
                        {
                            case true:
                                env.AddValues(NodeBlueprint.ConnectionClass.GiveComponent,
                                    ((IEnumerable)env.cacheSystem.GetCachedComponentsInChildren(component, obj)).Cast<object>().ToArray());
                                break;
                            case false:
                                env.AddValues(NodeBlueprint.ConnectionClass.GiveComponent,
                                    ((IEnumerable)env.cacheSystem.GetCachedComponents(component, obj)).Cast<object>().ToArray());
                                break;
                        }
                        break;
                }
            }
            
            yield return new CoroutineReturn(CoroutineReturn.CourtineType.ContinueBranch);
        }
        public IEnumerator<CoroutineReturn> Cache(NodeEnv env)
        {
            component = AbilityCreator.components[env.GetField(0)];
            return null;
        }
    }
}