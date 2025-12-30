using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IDK.NodeScripts
{
    public class GetEye : IValueNode
    {
        public override ValuePool GetDynamicValue(SavedNode savedNode, Unit unit, List<Node.Connection> connections, string[] fields)
        {
            ValuePool valuePool = new ValuePool();
            Unit[] units = connections.GetNode(NodeBlueprint.ConnectionType.ReciveUnit).GetValuePoolSmart(unit).GetValues<Unit>();
            Debug.Log("Units node:" + connections.GetNode(NodeBlueprint.ConnectionType.ReciveUnit));
            Debug.Log("Units Length: " + units.Length);
            foreach (var unitIndex in units)
            {
                Debug.Log($"Getting bodypart! (its {fields[0]}!)");
                EyeSpawner eyeSpawner = unitIndex.gameObject.GetComponentInChildren<EyeSpawner>();
                valuePool.AddRange(eyeSpawner.spawnedEyes.ToArray());
            }
            return valuePool;
        }
        public override bool IsDynamic()
        {
            return true;
        }
        public override ValuePool GetValuePool(SavedNode savedNode, Unit unit, List<Node.Connection> connections, string[] fields)
        {
            return null;
        }
    }
}