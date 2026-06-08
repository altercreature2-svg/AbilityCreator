using BitCode.Extensions;
using Landfall.TABS;
using System.Collections.Generic;
using UnityEngine;

namespace IDK.NodeScripts
{
    public class CreateGameobject : IValueNode
    {
        public override ValuePool GetDynamicValue(LegacySavedNode savedNode, Unit unit, List<NodeComponent.LegacyConnection> connections, string[] fields)
        {
            return null;
        }
        public override bool IsDynamic()
        {
            return false;
        }
        public override ValuePool GetValuePool(LegacySavedNode savedNode, Unit unit, List<NodeComponent.LegacyConnection> connections, string[] fields)
        {

            ValuePool valuePool = new ValuePool();
            ObjectStorer objectStorer = unit.gameObject.GetOrAddComponent<ObjectStorer>();
            if (!objectStorer.store.ContainsKey(fields[0]))
            {
                GameObject gameObject = new GameObject(fields[0]);
                gameObject.AddComponent<DestroySelfWhenObjectDestroyed>().obj = unit.gameObject;
                objectStorer.store.Add(fields[0], gameObject);
            };
            valuePool.AddValue(objectStorer.store[fields[0]]);
            return valuePool;
        }
    }
}