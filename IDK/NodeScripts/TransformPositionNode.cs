using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using UnityEngine;

namespace IDK.NodeScripts
{
    public class TransformPositionNode : IBehaviorNode
    {
        public override IEnumerator RunNode(LegacySavedNode savedNode, Unit unit, List<NodeComponent.LegacyConnection> connections, string[] fields, NodeRunner nodeRunner)
        {
            yield return null;
            GameObject[] gameObjects = connections.GetNode(NodeBlueprint.ConnectionClass.ReciveGameObject).GetValuePoolSmart(unit).GetValues<GameObject>();
            if (fields[3] == "Global")
            {
                foreach (var gameObj in gameObjects)
                {

                    if (fields[4] == "Add")
                        gameObj.transform.position += new Vector3(fields[0].QuickParse(), fields[1].QuickParse(), fields[2].QuickParse());
                    else
                        gameObj.transform.position = new Vector3(fields[0].QuickParse(), fields[1].QuickParse(), fields[2].QuickParse());
                }
            }
            else
            {
                foreach (var gameObj in gameObjects)
                {
                    if (fields[4] == "Add")
                        AddLocalPosition(gameObj, new Vector3(fields[0].QuickParse(), fields[1].QuickParse(), fields[2].QuickParse()));
                    else
                        SetLocalPosition(gameObj, new Vector3(fields[0].QuickParse(), fields[1].QuickParse(), fields[2].QuickParse()));
                }

            }

            yield return savedNode.TriggerConnection(nodeRunner);
        }
        public void SetLocalPosition(GameObject me, Vector3 vector3)
        {
            Vector3 forward = vector3.z * me.transform.forward;
            Vector3 upward = vector3.y * me.transform.up;
            Vector3 right = vector3.x * me.transform.right;
            me.transform.localPosition = forward + upward + right;
        }
        public void AddLocalPosition(GameObject me, Vector3 vector3)
        {
            Vector3 forward = vector3.z * me.transform.forward;
            Vector3 upward = vector3.y * me.transform.up;
            Vector3 right = vector3.x * me.transform.right;
            me.transform.localPosition += forward + upward + right;
        }
    }
}