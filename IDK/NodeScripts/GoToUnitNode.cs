using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IDK.NodeScripts
{
    public class GoToUnitNode : IBehaviorNode
    {
        public override IEnumerator RunNode(SavedNode savedNode, Unit unit, List<Node.Connection> connections, string[] fields, NodeRunner nodeRunner)
        {
            Unit[] units = connections.GetNode(NodeBlueprint.ConnectionType.ReciveUnit).GetValuePoolSmart(unit).GetValues<Unit>();
            GameObject[] gameObjects = connections.GetNode(NodeBlueprint.ConnectionType.ReciveGameObject).GetValuePoolSmart(unit).GetValues<GameObject>();
            foreach (var unitIndex in units)
            {
                foreach (var gameObj in gameObjects)
                {
                    if (fields[0] == "Both")
                    {
                        if (fields[1] == "Head")
                        {
                            TeleportObject(gameObj, unitIndex.data.head);
                        }
                        if (fields[1] == "Torso")
                        {
                            TeleportObject(gameObj, unitIndex.data.head);
                        }
                        if (fields[1] == "Right Arm")
                        {
                            TeleportObject(gameObj, unitIndex.data.rightArm);
                        }
                        if (fields[1] == "Left Arm")
                        {
                            TeleportObject(gameObj, unitIndex.data.leftArm);
                        }
                        if (fields[1] == "Right Hand")
                        {
                            TeleportObject(gameObj, unitIndex.data.rightHand);
                        }
                        if (fields[1] == "Left Hand")
                        {
                            TeleportObject(gameObj, unitIndex.data.leftHand);
                        }
                        if (fields[1] == "Right Knee")
                        {
                            TeleportObject(gameObj, unitIndex.data.legRight);
                        }
                        if (fields[1] == "Left Knee")
                        {
                            TeleportObject(gameObj, unitIndex.data.legLeft);
                        }
                        if (fields[1] == "Right Foot")
                        {
                            TeleportObject(gameObj, unitIndex.data.footRight);
                        }
                        if (fields[1] == "Left Foot")
                        {
                            TeleportObject(gameObj, unitIndex.data.footLeft);
                        }
                    }
                    else if (fields[0] == "Position only")
                    {
                        if (fields[1] == "Head")
                        {
                            TeleportPos(gameObj, unitIndex.data.head);
                        }
                        if (fields[1] == "Torso")
                        {
                            TeleportPos(gameObj, unitIndex.data.head);
                        }
                        if (fields[1] == "Right Arm")
                        {
                            TeleportPos(gameObj, unitIndex.data.rightArm);
                        }
                        if (fields[1] == "Left Arm")
                        {
                            TeleportPos(gameObj, unitIndex.data.leftArm);
                        }
                        if (fields[1] == "Right Hand")
                        {
                            TeleportPos(gameObj, unitIndex.data.rightHand);
                        }
                        if (fields[1] == "Left Hand")
                        {
                            TeleportPos(gameObj, unitIndex.data.leftHand);
                        }
                        if (fields[1] == "Right Knee")
                        {
                            TeleportPos(gameObj, unitIndex.data.legRight);
                        }
                        if (fields[1] == "Left Knee")
                        {
                            TeleportPos(gameObj, unitIndex.data.legLeft);
                        }
                        if (fields[1] == "Right Foot")
                        {
                            TeleportPos(gameObj, unitIndex.data.footRight);
                        }
                        if (fields[1] == "Left Foot")
                        {
                            TeleportPos(gameObj, unitIndex.data.footLeft);
                        }
                    }
                    else
                    {
                        if (fields[1] == "Head")
                        {
                            TeleportRot(gameObj, unitIndex.data.head);
                        }
                        if (fields[1] == "Torso")
                        {
                            TeleportRot(gameObj, unitIndex.data.head);
                        }
                        if (fields[1] == "Right Arm")
                        {
                            TeleportRot(gameObj, unitIndex.data.rightArm);
                        }
                        if (fields[1] == "Left Arm")
                        {
                            TeleportRot(gameObj, unitIndex.data.leftArm);
                        }
                        if (fields[1] == "Right Hand")
                        {
                            TeleportRot(gameObj, unitIndex.data.rightHand);
                        }
                        if (fields[1] == "Left Hand")
                        {
                            TeleportRot(gameObj, unitIndex.data.leftHand);
                        }
                        if (fields[1] == "Right Knee")
                        {
                            TeleportRot(gameObj, unitIndex.data.legRight);
                        }
                        if (fields[1] == "Left Knee")
                        {
                            TeleportRot(gameObj, unitIndex.data.legLeft);
                        }
                        if (fields[1] == "Right Foot")
                        {
                            TeleportRot(gameObj, unitIndex.data.footRight);
                        }
                        if (fields[1] == "Left Foot")
                        {
                            TeleportRot(gameObj, unitIndex.data.footLeft);
                        }
                    }

                }
            }
            yield return savedNode.TriggerConnection(nodeRunner);

        }
        public void TeleportObject(GameObject a, Transform b)
        {
            a.transform.position = b.position;
            a.transform.rotation = b.rotation;
        }
        public void TeleportPos(GameObject a, Transform b)
        {
            a.transform.position = b.position;
        }
        public void TeleportRot(GameObject a, Transform b)
        {
            a.transform.rotation = b.rotation;
        }
    }
}