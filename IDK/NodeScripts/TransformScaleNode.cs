

using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IDK.NodeScripts
{
    public class TransformScaleNode : IBehaviorNode
    {
        public override IEnumerator RunNode(LegacySavedNode savedNode, Unit unit, List<NodeComponent.LegacyConnection> connections, string[] fields, NodeRunner nodeRunner)
        {
            GameObject[] gameObjects = connections.GetNode(NodeBlueprint.ConnectionClass.ReciveGameObject).GetValuePoolSmart(unit).GetValues<GameObject>();
            foreach (var gameObj in gameObjects)
            {
                float x = gameObj.transform.localScale.x * fields[0].QuickParse();
                float y = gameObj.transform.localScale.y * fields[1].QuickParse();
                float z = gameObj.transform.localScale.z * fields[2].QuickParse();
                gameObj.transform.localScale = new Vector3(x, y, z);
            }
            yield return savedNode.TriggerConnection(nodeRunner);
        }
    }
}