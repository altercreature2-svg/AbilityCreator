

using AC.Node_Related_Scripts.NodeRunning;
using AC.Node_Related_Scripts.NodeRunning.Instructions.Courtines;
using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AC.NodeScripts
{
    public class ColorObjectNode : IBehaviorNode
    {
        
        public IEnumerator<CoroutineReturn> Execute(NodeEnv env)
        {
            float r = env.GetField(0).QuickParse();
            float g = env.GetField(1).QuickParse();
            float b = env.GetField(2).QuickParse();
            float a = env.GetField(3).QuickParse();
            int index = env.GetField(4).QuickParseInt();
            bool colorChildren = env.GetField(5) == "Color children";
            bool overrideColor = env.GetField(6) == "Set";
            Color color = new Color(r, g, b,a);
            
            
            var gameObjects = env.GetValues(NodeBlueprint.ConnectionClass.ReciveGameObject);
            foreach (var obj in gameObjects)
            {
                if (!(obj.value is GameObject gameObj))
                    continue;

                if (env.cacheSystem.GetCachedComponent<Renderer>(gameObj))
                {
                    Colorer.ColorObject(gameObj, color, index, overrideColor, false);
                }
                if (colorChildren)
                {
                    Renderer[] renderers = env.cacheSystem.GetCachedComponentsInChildren<Renderer>(gameObj);
                    for (int i = 0; i < renderers.Length; i++)
                    {
                        Colorer.ColorObject(renderers[i].gameObject, color, index, overrideColor, false);
                    }
                }
            }
            yield return new CoroutineReturn(CoroutineReturn.CourtineType.ContinueBranch);
        }
        public IEnumerator<CoroutineReturn> Cache(NodeEnv env)
        {
            return null;
        }
    }
}