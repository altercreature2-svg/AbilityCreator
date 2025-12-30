using BitCode.Extensions;
using IDK.Node_Related_Scripts;
using Landfall.TABS;
using Landfall.TABS.AI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace IDK.NodeScripts
{
    public class CreateVariableNode : IValueNode
    {
        public override bool IsDynamic()
        {
            return false;
        }
        public override ValuePool GetDynamicValue(SavedNode savedNode, Unit unit, List<Node.Connection> connections, string[] fields)
        {
            return null;
        }
        
        public override ValuePool GetValuePool(SavedNode savedNode, Unit unit, List<Node.Connection> connections, string[] fields)
        {
            
            ValuePool valuePool = new ValuePool();
            ObjectStorer objectStorer = unit.gameObject.GetOrAddComponent<ObjectStorer>();
            if (!objectStorer.store.ContainsKey(fields[0]))
                objectStorer.store.Add(fields[0], new Variable());
            valuePool.AddValue((Variable)objectStorer.store[fields[0]]);
            return valuePool;
        }
    }
}