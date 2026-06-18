using AC.Node_Related_Scripts.NodeRunning;
using AC.Node_Related_Scripts.NodeRunning.Instructions.Courtines;
using BitCode.Extensions;
using Landfall.TABS;
using System.Collections.Generic;
using UnityEngine;

namespace AC.NodeScripts
{
    public class CreateGameobject : IValueNode
    {
        public IEnumerator<CoroutineReturn> Cache(NodeEnv env)
        {
            string name = env.GetField(0);
            ObjectStorer objectStorer = env.cacheSystem.GetCachedComponent<ObjectStorer>(env.unit.gameObject);
            if (objectStorer == null)
                env.unit.gameObject.AddComponent<ObjectStorer>();
            if (!objectStorer.store.ContainsKey(name))
            {
                GameObject gameObject = new GameObject(name);
                gameObject.AddComponent<DestroySelfWhenObjectDestroyed>().obj = env.unit.gameObject;
                objectStorer.store.Add(name, gameObject);
            };
            env.AddValue(NodeBlueprint.ConnectionClass.GiveGameObject, objectStorer.store[name] as GameObject);
            return null;
        }
        public IEnumerator<CoroutineReturn> Execute(NodeEnv env)
        {
            return null;
        }
    }
}