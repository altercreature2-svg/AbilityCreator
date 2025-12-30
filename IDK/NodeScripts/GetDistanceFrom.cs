using IDK.Node_Related_Scripts;
using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IDK.NodeScripts
{
    public class GetDistanceFrom : IValueNode
    {
        public override ValuePool GetDynamicValue(SavedNode savedNode, Unit unit, List<Node.Connection> connections, string[] fields)
        {

            ValuePool valuePool = new ValuePool();
            Unit[] units = connections.GetNode(NodeBlueprint.ConnectionType.ReciveUnit).GetValuePoolSmart(unit).GetValues<Unit>();
            GameObject[] gameObjects = connections.GetNode(NodeBlueprint.ConnectionType.ReciveGameObject).GetValuePoolSmart(unit).GetValues<GameObject>();
            foreach (var unitIndex in units)
            {
                foreach (var gameObj in gameObjects)
                {

                    valuePool.AddValue(new Variable()
                    {
                        value = Vector3.Distance(unitIndex.data.mainRig.position, gameObj.transform.position)
                    }) ;
                }
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