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
        public override ValuePool GetDynamicValue(LegacySavedNode savedNode, Unit unit, List<NodeComponent.LegacyConnection> connections, string[] fields)
        {
            return null;
        }
        public override ValuePool GetValuePool(LegacySavedNode savedNode, Unit unit, List<NodeComponent.LegacyConnection> connections, string[] fields)
        {
            ValuePool valuePool = savedNode.GetValuePool(unit);
            GameObject[] gameObjects = connections.GetNode(NodeBlueprint.ConnectionClass.ReciveGameObject).GetValuePoolSmart(unit).GetValues<GameObject>();
            foreach (var gameObj in gameObjects)
            {
                if (fields[0] == "First")
                {
                    if (fields[1] == "Don't")
                        valuePool.AddValue(gameObj.GetComponent(AbilityCreator.components[fields[0]]));
                    else
                        valuePool.AddValue(gameObj.GetComponentInChildren(AbilityCreator.components[fields[0]]));

                }
                else
                {
                    if (fields[1] == "Don't")
                    {
                        Component[] array = gameObj.GetComponents(AbilityCreator.components[fields[0]]);
                        foreach (Component item in array)
                        {
                            valuePool.AddValue(item);
                        }
                        
                    }
                    else
                    {
                        Component[] array = gameObj.GetComponentsInChildren(AbilityCreator.components[fields[0]]);
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