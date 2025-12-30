using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IDK.NodeScripts
{
    public class RotateTowardsNode : IBehaviorNode
    {
        public override IEnumerator RunNode(SavedNode savedNode, Unit unit, List<Node.Connection> connections, string[] fields, NodeRunner nodeRunner)
        {
            Unit[] units = connections.GetNode(NodeBlueprint.ConnectionType.ReciveUnit).GetValuePoolSmart(unit).GetValues<Unit>();
            GameObject[] gameObjects = connections.GetNode(NodeBlueprint.ConnectionType.ReciveGameObject).GetValuePoolSmart(unit).GetValues<GameObject>();
            foreach (var unitIndex in units)
            {
                foreach (var gameObj in gameObjects)
                {
                    if (fields[0] == "Head")
                    {
                        Rotate(gameObj, unitIndex.data.head);
                    }
                    if (fields[0] == "Torso")
                    {
                        Rotate(gameObj, unitIndex.data.head);
                    }
                    if (fields[0] == "Right Arm")
                    {
                        Rotate(gameObj, unitIndex.data.rightArm);
                    }
                    if (fields[0] == "Left Arm")
                    {
                        Rotate(gameObj, unitIndex.data.leftArm);
                    }
                    if (fields[0] == "Right Hand")
                    {
                        Rotate(gameObj, unitIndex.data.rightHand);
                    }
                    if (fields[0] == "Left Hand")
                    {
                        Rotate(gameObj, unitIndex.data.leftHand);
                    }
                    if (fields[0] == "Right Knee")
                    {
                        Rotate(gameObj, unitIndex.data.legRight);
                    }
                    if (fields[0] == "Left Knee")
                    {
                        Rotate(gameObj, unitIndex.data.legLeft);
                    }
                    if (fields[0] == "Right Foot")
                    {
                        Rotate(gameObj, unitIndex.data.footRight);
                    }
                    if (fields[0] == "Left Foot")
                    {
                        Rotate(gameObj, unitIndex.data.footLeft);
                    }

                }
            }
            yield return savedNode.TriggerConnection(nodeRunner);

        }
        public void Rotate(GameObject a, Transform b)
        {
            Vector3 dir = b.transform.position - a.transform.position;
            a.transform.rotation = Quaternion.LookRotation(dir);
        }
    }
}