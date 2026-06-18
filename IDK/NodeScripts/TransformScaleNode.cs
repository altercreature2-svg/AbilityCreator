

using AC.Node_Related_Scripts.NodeRunning;
using AC.Node_Related_Scripts.NodeRunning.Instructions.Courtines;
using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AC.NodeScripts
{
    public class TransformScaleNode : IBehaviorNode
    {
        public float x, y, z;
        public IEnumerator<CoroutineReturn> Execute(NodeEnv env)
        {
            var gameObjects = env.GetValues(NodeBlueprint.ConnectionClass.ReciveGameObject);
            Vector3 vector = new Vector3(x, y, z);
            foreach (var item in gameObjects)
            {
                if (!(item.value is GameObject go))
                    continue;
                Transform transform = go.transform;
                transform.localScale = Vector3.Scale(transform.localScale, vector);
            }
            yield return new CoroutineReturn(CoroutineReturn.CourtineType.ContinueBranch);
        }
        public IEnumerator<CoroutineReturn> Cache(NodeEnv env)
        {
            x = env.GetField(0).QuickParse();
            y = env.GetField(1).QuickParse();
            z = env.GetField(2).QuickParse();
            return null;
        }
    }
}