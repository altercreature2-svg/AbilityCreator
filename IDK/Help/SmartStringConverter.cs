using System;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;

namespace IDK.Help
{
    public class SmartStringConverter
    {
        public static object Convert(Type type, string str)
        {
            object obj = null;
            switch (str)
            {
                case "new":
                    obj = Activator.CreateInstance(type);
                    break;
                case "null":
                    obj = default;
                    break;
                default:
                    obj = System.Convert.ChangeType(str, type);
                    break;
            }
            if (type.IsSerializable)
            {
                if (!IsValidJson(str))
                    return obj;
                obj = JsonUtility.FromJson(str, type);
            }
            return obj;
            
        }
        public static bool IsDesendantOf(Type type1, Type Desendant)
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

        public static bool IsValidJson(string json) // i dont even know anymore
        {
            if (string.IsNullOrWhiteSpace(json)) return false;
            json = json.Trim();
            if (!((json.StartsWith("{") && json.EndsWith("}")) ||
                  (json.StartsWith("[") && json.EndsWith("]"))))
                return false;

            string sanitized = Regex.Replace(json, @"\\[""\\\/bfnrtu]", "@");
            sanitized = Regex.Replace(sanitized, @"""[^""\\\n\r]*""|true|false|null|-?\d+(?:\.\d*)?(?:[eE][+\-]?\d+)?", "]");
            sanitized = Regex.Replace(sanitized, @"(?:^|:|,)(?:\s*\[)+", "");
            return Regex.IsMatch(sanitized, @"^[\],:{}\s]*$");
        }
    }
}