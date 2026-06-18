

using AC.Node_Related_Scripts.NodeRunning;
using AC.Node_Related_Scripts.NodeRunning.Instructions.Courtines;
using IDK.Help;
using Landfall.TABS;
using Newtonsoft.Json.Linq;
using Sirenix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace AC.NodeScripts
{
    public class SetFieldNode : IBehaviorNode
    {
        public IEnumerator<CoroutineReturn> Execute(NodeEnv env)
        {
            string fieldName = env.GetField(0);
            var objectsEnum = env.GetValues(NodeBlueprint.ConnectionClass.ReciveAnything);
            foreach (var obj in objectsEnum)
            {
                if (obj.value is null)
                    continue;
                object info = obj.GetFieldInfo(fieldName);
                if (info is FieldInfo fieldInfo)
                {
                    object value = SmartStringConverter.Convert(fieldInfo.FieldType, env.GetField(1));
                    fieldInfo.SetValue(obj, value);
                }
                else if (info is PropertyInfo propertyInfo)
                {
                    object value = SmartStringConverter.Convert(propertyInfo.PropertyType, env.GetField(1));
                    propertyInfo.SetValue(obj, value);
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