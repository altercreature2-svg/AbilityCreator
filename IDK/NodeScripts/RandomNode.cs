using IDK.Node_Related_Scripts;
using InControl.UnityDeviceProfiles;
using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IDK.NodeScripts
{
    public class RandomNode : IValueNode
    {
        
        public Dictionary<Unit, int> values = new Dictionary<Unit, int>();
        public Dictionary<Unit, bool> hasChosens = new Dictionary<Unit, bool>();
        public override bool IsDynamic()
        {
            return true;
        }
        public override ValuePool GetDynamicValue(SavedNode savedNode, Unit unit, List<Node.Connection> connections, string[] fields)
        {
            if (!hasChosens.ContainsKey(unit))
                hasChosens.Add(unit, false);
            if (!values.ContainsKey(unit))
                values.Add(unit, 0);
            if (fields[2] == "Static")
            {
                if (!hasChosens[unit])
                {
                    values[unit] = Random.Range(int.Parse(fields[0]), int.Parse(fields[1]) + 1);
                    hasChosens[unit] = true;
                }
            } 
            else
                 values[unit] = Random.Range(int.Parse(fields[0]), int.Parse(fields[1]) + 1);
            ValuePool valuePool = savedNode.GetValuePool(unit);
            Debug.Log("Random value:" + values[unit]);
            valuePool.ClearValues();
            valuePool.AddValue(new Variable() { value = values[unit] });
            return valuePool;
        }
        public override ValuePool GetValuePool(SavedNode savedNode, Unit unit, List<Node.Connection> connections, string[] fields)
        {
            return null;
        }
    }
}