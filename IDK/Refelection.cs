using Landfall.TABS;
using System;
using System.Reflection;
using UnityEngine;

namespace IDK
{
    public static class Refelection
    {

        // Token: 0x06000003 RID: 3 RVA: 0x00002078 File Offset: 0x00000278
        public static object CallMethod(this object o, string methodName, params object[] args)
        {
            MethodInfo method = o.GetType().GetMethod(methodName, (BindingFlags)(-1));
            object result;
            if (method != null)
            {
                result = method.Invoke(o, args);
            }
            else
            {

                result = null;
            }
            return result;
        }

        // Token: 0x06000004 RID: 4 RVA: 0x000020C0 File Offset: 0x000002C0
        public static TField GetField<TField>(this object instance, string fieldName, BindingFlags flags = (BindingFlags)(-1))
        {
            FieldInfo field = instance.GetType().GetField(fieldName, flags);
            return (TField)field.GetValue(instance);
        }
        public static object GetProprety(this object instance, string fieldName, BindingFlags flags = (BindingFlags)(-1))
        {

            try
            {
                Type type = instance.GetType();
                for (int i = 0; i < 8; i++)
                {
                    if (type == null)
                        return type;
                    try
                    {
                        PropertyInfo field = type.GetProperty(fieldName, flags);
                        return field.GetValue(instance);
                    }
                    catch
                    {
                        type = type.BaseType;
                    }
                }

            }
            catch
            {

            }
            return null;
        }
        public static object GetField(this object instance, string fieldName, BindingFlags flags = (BindingFlags)(-1))
        {
            try
            {
                Type type = instance.GetType();
                for (int i = 0; i < 8; i++)
                {
                    if (type == null)
                        throw new Exception();
                    try
                    {
                        FieldInfo field = (FieldInfo)GetFieldInfo(type, fieldName, flags);
                        return field.GetValue(instance);
                    }
                    catch
                    {
                        type = type.BaseType;
                    }
                }
            }
            catch
            {

            }
            return GetProprety(instance, fieldName);
        }
        public static object GetFieldInfo(Type type, string fieldName, BindingFlags flags = (BindingFlags)(-1))
        {

            object field = (object)type.GetField(fieldName, flags) ?? (object)type.GetProperty(fieldName, flags);
            return field;

           

        }
        public static FieldInfo GetFieldInfo<T>(this T instance, string fieldName, BindingFlags flags = (BindingFlags)(-1))
        {
            FieldInfo field = typeof(T).GetField(fieldName, flags);
            return field;
        }

        // Token: 0x06000005 RID: 5 RVA: 0x0000214C File Offset: 0x0000034C
        public static void SetField(this object instance, string fieldName, object newValue, BindingFlags flags = (BindingFlags)(-1))
        {
            FieldInfo field = instance.GetType().GetField(fieldName, flags) ?? instance.GetType().BaseType.GetField(fieldName, flags);
            field.SetValue(instance, newValue);
        }
        public static Team Reverse(this Team team)
        {
            if (team == Team.Red)
            {
                return Team.Blue;
            }
            else
            {
                return Team.Red;
            }
        }
    }
}