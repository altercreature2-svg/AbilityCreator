using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IDK.NodeScripts
{
    public class TransformPositionNodeAnim : IBehaviorNode
    {
        
        public override IEnumerator RunNode(LegacySavedNode savedNode, Unit unit, List<NodeComponent.LegacyConnection> connections, string[] fields, NodeRunner nodeRunner)
        {
            yield return null;
            GameObject[] gameObjects = connections.GetNode(NodeBlueprint.ConnectionClass.ReciveGameObject).GetValuePoolSmart(unit).GetValues<GameObject>();
            float time = 0;
            float end = fields[5].QuickParse();
            
            if (fields[3] == "Global")
            {
                Dictionary<GameObject, Vector3> posistions = new Dictionary<GameObject, Vector3>();
                while (end > time)
                {
                    foreach (var gameObj in gameObjects)
                    {
                        Vector3 pos;
                        if (!posistions.ContainsKey(gameObj))
                            posistions.Add(gameObj, gameObj.transform.position);
                        pos = posistions[gameObj];
                        if (fields[4] == "Add")
                        {
                            gameObj.transform.position = pos + (new Vector3(fields[0].QuickParse(), fields[1].QuickParse(), fields[2].QuickParse()) * time/end);
                        }
                        else
                        {
                            gameObj.transform.position = new Vector3(fields[0].QuickParse(), fields[1].QuickParse(), fields[2].QuickParse());
                        }
                        yield return null;
                    }
                }
            }
            else
            {
                Dictionary<GameObject, Vector3> posistions = new Dictionary<GameObject, Vector3>();
                while (end > time)
                {

                    foreach (var gameObj in gameObjects)
                    {
                        Vector3 pos;
                        if (!posistions.ContainsKey(gameObj))
                            posistions.Add(gameObj, gameObj.transform.position);
                        if (fields[4] == "Add")
                            AddLocalPosition(gameObj, new Vector3(fields[0].QuickParse(), fields[1].QuickParse(), fields[2].QuickParse()));
                        else
                            SetLocalPosition(gameObj, new Vector3(fields[0].QuickParse(), fields[1].QuickParse(), fields[2].QuickParse()));
                    }
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