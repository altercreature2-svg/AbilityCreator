

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
            bool change = fields[6] != "Set";
            bool reuse = fields[7] == "Reuse Material";
            Color color = new Color(r, g, b, a);
            GameObject[] gameObjects = connections.GetNode(NodeBlueprint.ConnectionType.ReciveGameObject).GetValuePoolSmart(unit).GetValues<GameObject>();

            foreach (var gameObj in gameObjects)
            {
                if (gameObj.GetComponent<Renderer>())
                {
                    ColorObject(gameObj, color, index, change, reuse);
                }
                if (fields[5] == "Color children")
                {
                    Renderer[] renderers = gameObj.GetComponentsInChildren<Renderer>();
                    for (int i = 0; i < renderers.Length; i++)
                    {
                        ColorObject(renderers[i].gameObject, color, index, change, reuse);
                    }
                }
            }
            yield return savedNode.TriggerConnection(nodeRunner);
        }
        public Material ColorMaterial(Material material, Color color, bool Change)
        {
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
            return material;
        }
        public void ColorObject(GameObject gameObject, Color color, int index, bool change, bool reuse)
        {
            Renderer renderer = gameObject.GetComponent<Renderer>();
            if (index == -1)
            {
                Material[] materials = new Material[renderer.materials.Length];
                for (int i = 0; i < renderer.materials.Length; i++)
                {

                    Material material = renderer.materials[i];
                    if (!reuse)
                        material = Object.Instantiate(renderer.materials[i]);
                    materials[i] = ColorMaterial(material, color, change);

                }
                renderer.materials = materials;
            }
            else
            {
                Material material = renderer.materials[index];
                if (!reuse)
                    material = Object.Instantiate(renderer.materials[index]);
                Material[] materials = renderer.materials;
                materials[index] = ColorMaterial(material, color, change);
                renderer.materials = materials;
            }
        }
    }
}