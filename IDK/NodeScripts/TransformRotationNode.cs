using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IDK.NodeScripts
{
    public class TransformRotationNode : IBehaviorNode
    {
        public override IEnumerator RunNode(SavedNode savedNode, Unit unit, List<Node.Connection> connections, string[] fields, NodeRunner nodeRunner)
        {
            yield return null;
            GameObject[] gameObjects = connections.GetNode(NodeBlueprint.ConnectionType.ReciveGameObject).GetValuePoolSmart(unit).GetValues<GameObject>();
            
            foreach (var gameObj in gameObjects)
            {
                Vector3 rotation = gameObj.transform.rotation.eulerAngles;
                Vector3 localRotation = gameObj.transform.localRotation.eulerAngles;
                if (fields[3] == "Global")
                {
                    if (fields[4] == "Add")
                        gameObj.transform.rotation = Quaternion.Euler(rotation + new Vector3(fields[0].QuickParse(), fields[1].QuickParse(), fields[2].QuickParse()));
                    else
                        gameObj.transform.rotation = Quaternion.Euler(new Vector3(fields[0].QuickParse(), fields[1].QuickParse(), fields[2].QuickParse()));
                }
                else
                {
                    if (fields[4] == "Add")
                        gameObj.transform.localRotation = Quaternion.Euler(localRotation + new Vector3(fields[0].QuickParse(), fields[1].QuickParse(), fields[2].QuickParse()));
                    else
                        gameObj.transform.localRotation = Quaternion.Euler(new Vector3(fields[0].QuickParse(), fields[1].QuickParse(), fields[2].QuickParse()));
                }

            }
            yield return savedNode.TriggerConnection(nodeRunner);

        }
    }
}