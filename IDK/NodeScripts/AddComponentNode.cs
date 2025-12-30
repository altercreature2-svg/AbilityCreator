

using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IDK.NodeScripts
{
    public class AddComponentNode : IBehaviorNode
    {
        public override ValuePool GetValuePool(SavedNode savedNode, Unit unit, List<Node.Connection> connections, string[] fields)
        {
            return savedNode.GetValuePool(unit);
        }
        public override IEnumerator RunNode(SavedNode savedNode, Unit unit, List<Node.Connection> connections, string[] fields, NodeRunner nodeRunner)
        {
            ValuePool valuePool = savedNode.GetValuePool(unit);
            GameObject[] gameObjects = connections.GetNode(NodeBlueprint.ConnectionType.ReciveGameObject).GetValuePoolSmart(unit).GetValues<GameObject>();
            foreach (var gameObj in gameObjects)
            {
                valuePool.AddValue(gameObj.AddComponent(Main.components[fields[0]])); 
            }
            savedNode.valuePools[unit] = valuePool;
            yield return savedNode.TriggerConnection(nodeRunner);

        }
    }
}