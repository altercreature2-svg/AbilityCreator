

using Landfall.TABS;
using Newtonsoft.Json.Linq;
using Sirenix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace IDK.NodeScripts
{
    public class SetFieldNode : IBehaviorNode
    {
        public override IEnumerator RunNode(LegacySavedNode savedNode, Unit unit, List<NodeComponent.LegacyConnection> connections, string[] fields, NodeRunner nodeRunner)
        {
            object[] components = connections.GetNode(NodeBlueprint.ConnectionClass.ReciveAnything).GetValuePoolSmart(unit).GetValues<object>();
            Debug.Log("Setting Fields for " + components.Length + " components!");
            foreach (var component in components)
            {
                Debug.Log($"FieldName:{fields[0]}");
                object fieldInfo = Refelection.GetFieldInfo(component.GetType(),fields[0]);
                Debug.Log($"Field :{fieldInfo}");
                object value = fields[1];

                Type type = null;
                if (fieldInfo is FieldInfo info)
                    type = info.FieldType;
                if (fieldInfo is PropertyInfo info1)
                    type = info1.PropertyType;
                try
                {
                    if ((string)value == "null")
                    {
                        value = null;
                    }
                    else if ((string)value == "new")
                    {
                        value = Activator.CreateInstance(type);
                    }
                    else
                        value = Convert.ChangeType(value, type);
                } catch 
                {
                    try
                    {
                        if (IsDesendantOf(type,typeof(UnityEngine.Object)))
                        {
                            if (type.IsSerializable)
                            {
                                value = JsonUtility.FromJson(fields[1], type);
                            }
                        }
                    }catch { }
                }
                Debug.Log("value:" + value);
                if (fieldInfo is FieldInfo field)
                    field.SetValue(component, value);
                if (fieldInfo is PropertyInfo property)
                    property.SetValue(component, value);
            }
            yield return savedNode.TriggerConnection(nodeRunner);

        }
        public bool IsDesendantOf(Type type1, Type Desendant)
        {
            Type type = type1;
            for (int i = 0; i < 8; i++)
            {
                if (type.BaseType == Desendant)
                    return true;
                else
                    type = type.BaseType;
                    
            }
            return false;
        }
    }
    
}