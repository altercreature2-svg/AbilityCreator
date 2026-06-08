using HarmonyLib;
using IDK.Node_Related_Scripts;
using Landfall.TABS;
using Sirenix.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IDK.NodeScripts
{
    public class SetObjectVariableTo : IBehaviorNode
    {
        public override IEnumerator RunNode(LegacySavedNode savedNode, Unit unit, List<NodeComponent.LegacyConnection> connections, string[] fields, NodeRunner nodeRunner)
        {
            ObjectVariable[] variables = connections.GetNode(NodeBlueprint.ConnectionClass.ReciveObjectVariable).GetValuePoolSmart(unit).GetValues<ObjectVariable>();
            object[] objects = connections.GetNode(NodeBlueprint.ConnectionClass.ReciveAnything).GetValuePoolSmart(unit).GetValues<object>();
            for (int i = 0; i < variables.Length; i++)
            {
                Debug.Log("Varibale:" + variables[i]);
                variables[i].value = variables[i].value.AddRangeToArray(objects);
                objects.Do(n => Debug.Log("Object:" + n));
            }
            yield return savedNode.TriggerConnection(nodeRunner);

        }
    }
}