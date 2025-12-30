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
        public int value;
        public bool hasChosen;
        public override bool IsDynamic()
        {
            return true;
        }
        public override ValuePool GetDynamicValue(SavedNode savedNode, Unit unit, List<Node.Connection> connections, string[] fields)
        {
            if (fields[2] == "Static")
            {
                if (!hasChosen)
                {
                    value = Random.Range(int.Parse(fields[0]), int.Parse(fields[1]) + 1);
                    hasChosen = true;
                }
            } 
            else
                 value = Random.Range(int.Parse(fields[0]), int.Parse(fields[1]) + 1);
            ValuePool valuePool = savedNode.GetValuePool(unit);
            Debug.Log("Random value:" + value);
            valuePool.ClearValues();
            valuePool.AddValue(new Variable() { value = value });
            return valuePool;
        }
        public override ValuePool GetValuePool(SavedNode savedNode, Unit unit, List<Node.Connection> connections, string[] fields)
        {
            return null;
        }
    }
}