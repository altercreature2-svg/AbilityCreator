using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace IDK
{
    public class Colorer
    {
        public static Dictionary<string, string> shaders = new Dictionary<string, string>
        {
            { "TFBG/SimpleVertexColor", "Standard" },
            { "TFBG/SimpleTintDiffuseUnit", "Standard" },
        };

        public static void MakeTransparent(Material material)
        {
            material.SetFloat("_Mode", 3);
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            material.SetInt("_ZWrite", 0);
            material.DisableKeyword("_ALPHATEST_ON");
            material.EnableKeyword("_ALPHABLEND_ON");
            material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            material.renderQueue = 3000;
        }
        public static void MakeOpaque(Material material)
        {
            material.SetFloat("_Mode", 0);
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
            material.SetInt("_ZWrite", 1);
            material.DisableKeyword("_ALPHATEST_ON");
            material.DisableKeyword("_ALPHABLEND_ON");
            material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            material.renderQueue = -1;
        }
        public static void FixShader(Material material)
        {
            for (int i = 0; i < shaders.Count; i++)
            {
                string shaderName = shaders.Keys.ElementAt(i);
                string newShader = shaders.Values.ElementAt(i);
                if (material.shader.name == shaderName)
                {
                    material.shader = Shader.Find(newShader);
                }
            }
        }
        public static void ColorObject(GameObject gameObject, Color color, int index, bool overrideColor, bool HSV)
        {
            Renderer renderer = gameObject.GetComponent<Renderer>();
            if (renderer)
            {
                if (index < 0)
                {
                    for (int i = 0; i < renderer.materials.Length; i++)
                    {
                        if (!HSV)
                            ColorMaterial(renderer.materials[i], color, overrideColor);
                        else
                            TweakColorWithHSV(renderer.materials[i], color.r, color.g, color.b);
                    }
                }
                else if (index < renderer.materials.Length)
                {
                    if (!HSV)
                        ColorMaterial(renderer.materials[index], color, overrideColor);
                    else 
                        TweakColorWithHSV(renderer.materials[index], color.r, color.g, color.b);
                }
            }
        }
        public static void ColorMaterial(Material material, Color color, bool overrideColor)
        {
            if (material == null)
            {
                Debug.LogWarning("material is null cant tweak color");
                return;
            }

            Color realColor = color;

            material.color += realColor;
            if (overrideColor)
            {
                Color color1 = material.color;
                color1.r = realColor.r != -652 ? realColor.r : color1.r;
                color1.g =  realColor.g != -652 ? realColor.g : color1.g;
                color1.b =  realColor.b != -652 ? realColor.b : color1.b;
                color1.a =  realColor.a != -652 ? realColor.a : color1.a;
                material.color = color1;
            }
            FixShader(material);
            if (realColor.a < 1)
                MakeTransparent(material);
            else
                MakeOpaque(material);

            if (material.HasProperty("_EmissionColor"))
            {
                if (material.IsKeywordEnabled("_EMISSION"))
                {
                    material.SetColor("_EmissionColor", realColor);
                }
            }

            Debug.Log("Colored " + material.name + " to " + material.color);
        }
        public static void TweakColorWithHSV(Material material, float h, float s, float v)
        {
            if (material == null)
            {
                Debug.LogWarning("material is null cant tweak color");
                return;
            }

            Color currentColor = material.color;
            Color.RGBToHSV(currentColor, out float hue, out float saturation, out float value);

            hue = Mathf.Repeat(hue + h, 1f);
            saturation = Mathf.Clamp01(saturation + s);
            value = Mathf.Clamp01(value + v);

            Color tweakedColor = Color.HSVToRGB(hue, saturation, value);
            tweakedColor.a = currentColor.a;
            material.color = tweakedColor;

            FixShader(material);

            if (tweakedColor.a < 1)
                MakeTransparent(material);
            else
                MakeOpaque(material);

            if (material.HasProperty("_EmissionColor") && material.IsKeywordEnabled("_EMISSION"))
            {
                material.SetColor("_EmissionColor", tweakedColor);
            }

            Debug.Log("Tweaked " + material.name + " to " + material.color);
        }
    }
}
