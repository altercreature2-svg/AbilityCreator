

using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IDK.NodeScripts
{
    public class ColorObjectNodeHSV : IBehaviorNode
    {
        public override IEnumerator RunNode(LegacySavedNode savedNode, Unit unit, List<NodeComponent.LegacyConnection> connections, string[] fields, NodeRunner nodeRunner)
        {
            yield return null;
            float h = fields[0].QuickParse();
            float s = fields[1].QuickParse();
            float v = fields[2].QuickParse();
            float a = fields[3].QuickParse();
            int index = (int)fields[4].QuickParse();
            Color color = new Color(h,s,v,a);
            GameObject[] gameObjects = connections.GetNode(NodeBlueprint.ConnectionClass.ReciveGameObject).GetValuePoolSmart(unit).GetValues<GameObject>();

            foreach (var gameObj in gameObjects)
            {
                if (gameObj.GetComponent<Renderer>())
                {
                    Colorer.ColorObject(gameObj, color, index, false, true);
                }
                if (fields[5] == "Color children")
                {
                    Renderer[] renderers = gameObj.GetComponentsInChildren<Renderer>();
                    for (int i = 0; i < renderers.Length; i++)
                    {
                        Colorer.ColorObject(renderers[i].gameObject, color, index, false, true);
                    }
                }
            }
            yield return savedNode.TriggerConnection(nodeRunner);
        }
        
        
    }
}