

using InControl.NativeDeviceProfiles;
using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace IDK.NodeScripts
{
    public class Duplicate : IBehaviorNode
    {
        public override IEnumerator RunNode(LegacySavedNode savedNode, Unit unit, List<NodeComponent.LegacyConnection> connections, string[] fields, NodeRunner nodeRunner)
        {
            Object[] objects = connections.GetNode(NodeBlueprint.ConnectionClass.ReciveAnything).GetValuePoolSmart(unit).GetValues<Object>();
            Debug.Log(objects.Length +" objects to dupe!");
            
            foreach (var @object in objects)
            {
                try
                {
                    Debug.Log("Duping object:" + @object.ToString());
                    ValuePool valuePool = savedNode.GetValuePool(unit);
                    valuePool.ClearValues();
                    valuePool.AddValue(Object.Instantiate(@object));
                    Debug.Log("Done!");
                }
                catch (System.Exception)
                {
                }
            }
            yield return savedNode.TriggerConnection(nodeRunner);
        }
        public override ValuePool GetValuePool(LegacySavedNode savedNode, Unit unit, List<NodeComponent.LegacyConnection> connections, string[] fields)
        {
            return savedNode.GetValuePool(unit);
        }
    }
}