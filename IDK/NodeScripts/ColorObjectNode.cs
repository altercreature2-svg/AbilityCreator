

using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IDK.NodeScripts
{
    public class ColorObjectNode : IBehaviorNode
    {
        public override IEnumerator RunNode(SavedNode savedNode, Unit unit, List<Node.Connection> connections, string[] fields, NodeRunner nodeRunner)
        {
            yield return null;
            float r = fields[0].QuickParse();
            float g = fields[1].QuickParse();
            float b = fields[2].QuickParse();
            float a = fields[3].QuickParse();
            int index = (int)fields[4].QuickParse();
            Color color = new Color(r, g, b, a);
            GameObject[] gameObjects = connections.GetNode(NodeBlueprint.ConnectionType.ReciveGameObject).GetValuePoolSmart(unit).GetValues<GameObject>();
            
            foreach (var gameObj in gameObjects)
            {
                if (gameObj.GetComponent<Renderer>())
                {
                    
                    ColorObject(gameObj,color, index, fields[1] != "Set");
                }
                if (fields[5] == "Color children")
                {
                    Renderer[] renderers = gameObj.GetComponentsInChildren<Renderer>();
                    for (int i = 0; i < renderers.Length; i++)
                    {
                        ColorObject(renderers[i].gameObject, color, index, fields[1] != "Set");
                    }
                }
            }
            yield return savedNode.TriggerConnection(nodeRunner);
        }
        public void ColorObject(GameObject gameObject, Color color, int index, bool Change)
        {
            try
            {
                if (index == -1)
                {
                    
                    Material[] materials = new Material[gameObject.GetComponent<Renderer>().materials.Length];
                    for (int i = 0; i < gameObject.GetComponent<Renderer>().materials.Length; i++)
                    {
                        
                        Material material = Object.Instantiate(gameObject.GetComponent<Renderer>().materials[i]);
                        Color color1 = color;
                        if (Change)
                            color1 = material.color + color;
                        
                        if (material.shader.name == "TFBG/SimpleVertexColor")
                        {
                            material.shader = Shader.Find("Standard");
                        }
                        material.color = color1;
                        if (material.HasProperty("_EmissionColor"))
                        {
                            if (material.IsKeywordEnabled("_EMISSION"))
                            {
                                material.SetColor("_EmissionColor", color1);
                            }
                        }
                        
                        materials[i] = material;
                        
                    }
                    gameObject.GetComponent<Renderer>().materials = materials;
                }
                else
                {
                    Material material = Object.Instantiate(gameObject.GetComponent<Renderer>().materials[index]);
                    Color color1 = color;
                    if (Change)
                        color1 = material.color + color;
                    if (material.shader.name == "TFBG/SimpleVertexColor" || material.shader.name == "SimpleVertColorUnit")
                    {
                        
                    }
                    material.color = color1;

                    if (material.HasProperty("_EmissionColor"))
                    {
                        if (material.IsKeywordEnabled("_EMISSION"))
                        {
                            material.SetColor("_EmissionColor", color1);
                        }
                    }
                    
                    Material[] materials = gameObject.GetComponent<Renderer>().materials;
                    materials[index] = material;
                    gameObject.GetComponent<Renderer>().materials = materials;
                }
            }
            catch
            {

            }
        }
    }
}