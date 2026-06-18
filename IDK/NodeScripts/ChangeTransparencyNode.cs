using AC.Node_Related_Scripts.NodeRunning;
using AC.Node_Related_Scripts.NodeRunning.Instructions.Courtines;
using Landfall.TABS;
using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AC.NodeScripts
{
    public class ChangeTransparencyNode : IBehaviorNode
    {
        public IEnumerator<CoroutineReturn> Execute(NodeEnv env)
        {
            float a = env.GetField(0).QuickParse();
            int index = env.GetField(1).QuickParseInt();
            bool reality = env.GetField(2) == "Override";
            var objects = env.GetValues(NodeBlueprint.ConnectionClass.ReciveGameObject);
            foreach (var obj in objects) 
            {
                if (!(obj.value is GameObject go))
                    continue;
                Colorer.ColorObject(
                    go,
                    new Color(-652, -652, -652, a),
                    index,
                    reality,
                    false
                );
            }
            yield return new CoroutineReturn(CoroutineReturn.CourtineType.ContinueBranch);
        }
        public IEnumerator<CoroutineReturn> Cache(NodeEnv env)
        {
            return null;
        }
    }
}
