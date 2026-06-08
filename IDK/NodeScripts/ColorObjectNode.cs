

using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace IDK.NodeScripts
{
    public class ColorObjectNode : IBehaviorNode
    {
        
        public override IEnumerator RunNode(LegacySavedNode savedNode, Unit unit, List<NodeComponent.LegacyConnection> connections, string[] fields, NodeRunner nodeRunner)
        {
            yield return null;
            float r = fields[0].QuickParse();
            float g = fields[1].QuickParse();
            float b = fields[2].QuickParse();
            float a = fields[3].QuickParse();
            int index = (int)fields[4].QuickParse();
            bool overrideColor = fields[6] == "Set";
            Color color = new Color(r, g, b,a);
            
            
            GameObject[] gameObjects = connections.GetNode(NodeBlueprint.ConnectionClass.ReciveGameObject).GetValuePoolSmart(unit).GetValues<GameObject>();

            foreach (var gameObj in gameObjects)
            {
                if (gameObj.GetComponent<Renderer>())
                {
                    Colorer.ColorObject(gameObj, color, index, overrideColor, false);
                }
                if (fields[5] == "Color children")
                {
                    Renderer[] renderers = gameObj.GetComponentsInChildren<Renderer>();
                    for (int i = 0; i < renderers.Length; i++)
                    {
                        Colorer.ColorObject(renderers[i].gameObject, color, index, overrideColor, false);
                    }
                }
            }
            yield return savedNode.TriggerConnection(nodeRunner);
        }
    }
}