using Landfall.TABS;
using System.Collections.Generic;
using UnityEngine;

namespace IDK.NodeScripts
{
    public class GetComponentNode : IValueNode
    {
        public override bool IsDynamic()
        {
            return false;
        }
        public override ValuePool GetDynamicValue(SavedNode savedNode, Unit unit, List<Node.Connection> connections, string[] fields)
        {
            return null;
        }
        public override ValuePool GetValuePool(SavedNode savedNode, Unit unit, List<Node.Connection> connections, string[] fields)
        {
            ValuePool valuePool = savedNode.GetValuePool(unit);
            GameObject[] gameObjects = connections.GetNode(NodeBlueprint.ConnectionType.ReciveGameObject).GetValuePoolSmart(unit).GetValues<GameObject>();
            foreach (var gameObj in gameObjects)
            {
                if (fields[0] == "First")
                {
                    if (fields[1] == "Don't")
                        valuePool.AddValue(gameObj.GetComponent(Main.components[fields[0]]));
                    else
                        valuePool.AddValue(gameObj.GetComponentInChildren(Main.components[fields[0]]));

                }
                else
                {
                    if (fields[1] == "Don't")
                    {
                        Component[] array = gameObj.GetComponents(Main.components[fields[0]]);
                        foreach (Component item in array)
                        {
                            valuePool.AddValue(item);
                        }
                        
                    }
                    else
                    {
                        Component[] array = gameObj.GetComponentsInChildren(Main.components[fields[0]]);
                        foreach (Component item in array)
                        {
                            valuePool.AddValue(item);
                        }

                    }
                }
            }
            savedNode.valuePools[unit] = valuePool;
            return valuePool;
        }
    }
}