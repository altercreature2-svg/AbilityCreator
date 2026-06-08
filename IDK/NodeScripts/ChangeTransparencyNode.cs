using Landfall.TABS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace IDK.NodeScripts
{
    public class ChangeTransparencyNode : IBehaviorNode
    {
        public override ValuePool GetValuePool(LegacySavedNode savedNode, Unit unit, List<NodeComponent.LegacyConnection> connections, string[] fields)
        {
            return base.GetValuePool(savedNode, unit, connections, fields);
        }
        public override IEnumerator RunNode(LegacySavedNode savedNode, Unit unit, List<NodeComponent.LegacyConnection> connections, string[] fields, NodeRunner nodeRunner)
        {
            float a = fields[0].QuickParse();
            int index = (int)fields[1].QuickParse();
            bool reality = fields[2] == "Override";
            GameObject[] objectsIn = connections.GetNode(NodeBlueprint.ConnectionClass.ReciveGameObject).GetValuePool(unit).GetValues<GameObject>();
            for (int i = 0; i < objectsIn.Length; i++)
            {
                GameObject obj = objectsIn[i];
                Colorer.ColorObject(
                    obj,
                    new Color(-652, -652, -652, a),
                    index,
                    reality,
                    false
                );
            }
            yield return savedNode.TriggerConnection(nodeRunner);
        }
    }
}
