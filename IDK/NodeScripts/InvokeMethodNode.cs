

using InControl.NativeDeviceProfiles;
using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace IDK.NodeScripts
{
    public class InvokeMethodNode : IBehaviorNode
    {
        public override IEnumerator RunNode(SavedNode savedNode, Unit unit, List<Node.Connection> connections, string[] fields, NodeRunner nodeRunner)
        {
            object[] components = connections.GetNode(NodeBlueprint.ConnectionType.ReciveAnything).GetValuePoolSmart(unit).GetValues<object>();
            foreach (var component in components)
            {
                try
                {
                    ValuePool valuePool = savedNode.GetValuePool(unit);
                    valuePool.ClearValues();
                    MethodInfo methodInfo = component.GetType().GetMethod(fields[0], (BindingFlags)(-1));
                    valuePool.AddValue(methodInfo.Invoke(component, new object[0]));
                }
                catch (System.Exception)
                {
                }
            }
            yield return savedNode.TriggerConnection(nodeRunner);
        }
        public override ValuePool GetValuePool(SavedNode savedNode, Unit unit, List<Node.Connection> connections, string[] fields)
        {
            return savedNode.GetValuePool(unit);
        }
    }
}