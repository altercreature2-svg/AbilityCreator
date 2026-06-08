

using Landfall.TABS;
using System.Collections.Generic;
using UnityEngine;

namespace IDK.NodeScripts
{
    public class GetFieldNode : IValueNode
    {
        public override ValuePool GetDynamicValue(LegacySavedNode savedNode, Unit unit, List<NodeComponent.LegacyConnection> connections, string[] fields)
        {
            ValuePool valuePool = new ValuePool();
            object[] objects = connections.GetNode(NodeBlueprint.ConnectionClass.ReciveAnything).GetValuePoolSmart(unit).GetValues<object>();
            Debug.Log("Getting Fields for " + objects.Length + " objects!");
            foreach (var @object in objects)
            {
                Debug.Log($"FieldName:{fields[0]}");
                object value = @object.GetField(fields[0]);
                if (value != null)
                {
                    var newValue = EnumerableHelper.ToArrayIfEnumerable(value);
                    if (newValue == null)
                        valuePool.AddValue(value);
                    else
                    {
                        for (int i = 0; i < newValue.Length; i++)
                        {
                            valuePool.AddValue(newValue[i]);
                        }
                    }
                }
            }
            return valuePool;
        }
        public override bool IsDynamic()
        {
            return true;
        }
        public override ValuePool GetValuePool(LegacySavedNode savedNode, Unit unit, List<NodeComponent.LegacyConnection> connections, string[] fields)
        {
            return null;
        }
    }
}