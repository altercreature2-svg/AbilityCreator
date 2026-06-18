using AC.Node_Related_Scripts;
using AC.Node_Related_Scripts.NodeRunning;
using AC.Node_Related_Scripts.NodeRunning.Instructions.Courtines;
using BitCode.Extensions;
using Landfall.TABS;
using Landfall.TABS.AI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AC.NodeScripts
{
    public class CreateVariableNode : IValueNode
    {
        public IEnumerator<CoroutineReturn> Cache(NodeEnv env)
        {
            string name = env.GetField(0);
            ObjectStorer objectStorer = env.cacheSystem.GetCachedComponent<ObjectStorer>(env.unit.gameObject);
            if (objectStorer == null)
                env.unit.gameObject.AddComponent<ObjectStorer>();
            if (!objectStorer.store.ContainsKey(name))
            { 
                objectStorer.store.Add(name, new Variable());
            }
            
            env.AddValue(NodeBlueprint.ConnectionClass.GiveGameObject, objectStorer.store[name] as Variable);
            return null;
        }
        public IEnumerator<CoroutineReturn> Execute(NodeEnv env)
        {
            return null;
        }
    }
}