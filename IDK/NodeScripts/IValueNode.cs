using HarmonyLib;
using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using Sirenix.Utilities;

namespace AC.NodeScripts
{
    public class ValuePool
    {
        private List<object> values = new List<object>();
        public T[] GetValues<T>()
        {
            try
            {

                List<T> array = new List<T>();
                for (int i = 0; i < values.Count; i++)
                {
                    if (values[i] is T t)
                    {
                        array.Add(t);
                    }
                }

                return array.ToArray();
            }
            catch (Exception e)
            {
                Debug.Log("(ValuePool GetValues) Something went wrong!" + e);
                return null;
            }

        }
        public void AddValue<T>(T value)
        {
            try
            {
                values.Add(value);
            }
            catch (Exception e)
            {
                Debug.Log("(ValuePool AddValue) Something went wrong!" + e);
            }
        }
        public void AddValue(object value)
        {
            try
            {
                values.Add(value);
            }
            catch (Exception e)
            {
                Debug.Log("(ValuePool AddValue) Something went wrong!" + e);
            }
        }
        public void AddRange(object[] value)
        {
            for (int i = 0; i < value.Length; i++)
            {
                AddValue(value[i]);
            }
        }

        public void ClearValues()
        {
            try
            {
                values.Clear();
            }
            catch (Exception e)
            {
                Debug.Log("(ValuePool ClearValues) Something went wrong!" + e);
            }
        }

    }
    public interface IValueNode : INode
    {
    }
}