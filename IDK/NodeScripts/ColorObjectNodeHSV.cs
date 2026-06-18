

using AC.Node_Related_Scripts.NodeRunning;
using AC.Node_Related_Scripts.NodeRunning.Instructions.Courtines;
using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AC.NodeScripts
{
    public class ColorObjectNodeHSV : IBehaviorNode
    {
        public IEnumerator<CoroutineReturn> Execute(NodeEnv env)
        {
            float h = env.GetField(0).QuickParse();
            float s = env.GetField(1).QuickParse();
            float v = env.GetField(2).QuickParse();
            float a = env.GetField(3).QuickParse();
            int index = env.GetField(4).QuickParseInt();
            bool colorChildren = env.GetField(5) == "Color children";
            Color color = new Color(h, s, v, a);


            var gameObjects = env.GetValues(NodeBlueprint.ConnectionClass.ReciveGameObject);
            foreach (var obj in gameObjects)
            {
                if (!(obj.value is GameObject gameObj))
                    continue;

                if (env.cacheSystem.GetCachedComponent<Renderer>(gameObj))
                {
                    Colorer.ColorObject(gameObj, color, index, false, true);
                }
                if (colorChildren)
                {
                    Renderer[] renderers = env.cacheSystem.GetCachedComponentsInChildren<Renderer>(gameObj);
                    for (int i = 0; i < renderers.Length; i++)
                    {
                        Colorer.ColorObject(renderers[i].gameObject, color, index, false, true);
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