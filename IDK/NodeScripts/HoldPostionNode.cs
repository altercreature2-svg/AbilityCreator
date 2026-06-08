using IDK.Help_Componets;
using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace IDK.NodeScripts
{
    public class HoldPostionNode : IBehaviorNode
    {
        public override IEnumerator RunNode(LegacySavedNode savedNode, Unit unit, List<NodeComponent.LegacyConnection> connections, string[] fields, NodeRunner nodeRunner)
        {
            Unit[] units = connections.GetNode(NodeBlueprint.ConnectionClass.ReciveUnit).GetValuePoolSmart(unit).GetValues<Unit>();
            GameObject[] bodyParts = connections.GetNode(NodeBlueprint.ConnectionClass.ReciveGameObject).GetValuePoolSmart(unit).GetValues<GameObject>();
            bool x = fields[1] != "Off";
            bool y = fields[2] != "Off";
            bool z = fields[3] != "Off";
            foreach (var unitIndex in units)
            {
                unitIndex.gameObject.AddComponent<HoldPosition>().Go(fields[0].QuickParse(), bodyParts.Select(n=>n.GetComponent<Rigidbody>()).ToArray(), x, y, z);
            }
            yield return savedNode.TriggerConnection(nodeRunner);

        }
    }
}