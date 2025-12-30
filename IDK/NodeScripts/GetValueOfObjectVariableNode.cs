using BitCode.Extensions;
using HarmonyLib;
using IDK.Node_Related_Scripts;
using Landfall.TABS;
using System.Collections.Generic;
using UnityEngine;

namespace IDK.NodeScripts
{
    public class GetValueOfObjectVariableNode : IValueNode
    {
        public override bool IsDynamic()
        {
            return true;
        }
        public override ValuePool GetDynamicValue(SavedNode savedNode, Unit unit, List<Node.Connection> connections, string[] fields)
        {
            ObjectVariable[] objectVariables = connections.GetNode(NodeBlueprint.ConnectionType.ReciveObjectVariable).GetValuePoolSmart(unit).GetValues<ObjectVariable>();
            ValuePool valuePool = new ValuePool();
            for (int i = 0; i < objectVariables.Length; i++)
            {
                Debug.Log("Varibale:" + objectVariables[i]);
                object[] store = objectVariables[i].value;
                store.Do(n => Debug.Log("store product:" + n));
                valuePool.AddRange(store);
            }
            return valuePool;
        }

        public override ValuePool GetValuePool(SavedNode savedNode, Unit unit, List<Node.Connection> connections, string[] fields)
        {
            return null;
            
        }
    }
}