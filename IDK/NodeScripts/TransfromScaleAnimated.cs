

using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IDK.NodeScripts
{
    public class TransfromScaleAnimated : IBehaviorNode
    {
        public override IEnumerator RunNode(SavedNode savedNode, Unit unit, List<Node.Connection> connections, string[] fields, NodeRunner nodeRunner)
        {
            GameObject[] gameObjects = connections.GetNode(NodeBlueprint.ConnectionType.ReciveGameObject).GetValuePoolSmart(unit).GetValues<GameObject>();
            float time = 0;
            float end = fields[3].QuickParse();
            AnimationCurve curve;
            if (fields[4] == "Constant")
                curve = AnimationCurve.Constant(0, end, 1);
            else
                curve = AnimationCurve.EaseInOut(0,0,end,1);
            while (end > time)
            {
                foreach (var gameObj in gameObjects)
                {
                    float x = gameObj.transform.localScale.x * fields[0].QuickParse();
                    float y = gameObj.transform.localScale.y * fields[1].QuickParse();
                    float z = gameObj.transform.localScale.z * fields[2].QuickParse();
                    gameObj.transform.localScale = new Vector3(x, y, z);
                }
                time += Time.deltaTime;
                yield return null;
            }
            yield return savedNode.TriggerConnection(nodeRunner);
        }
    }
}