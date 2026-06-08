using Landfall.TABC;
using Landfall.TABS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace IDK.NodeScripts
{
    public class AddGlobalForceNode : IBehaviorNode
    {
        public override IEnumerator RunNode(LegacySavedNode savedNode, Unit unit, List<NodeComponent.LegacyConnection> connections, string[] fields, NodeRunner nodeRunner)
        {
            ValuePool valuePool = connections.GetNode(NodeBlueprint.ConnectionClass.ReciveGameObject).GetValuePoolSmart(unit);

            Rigidbody[] rbs = valuePool.GetValues<Rigidbody>();
            foreach (var rigidbody in rbs)
            {
                rigidbody.AddForce(fields[0].QuickParse() * 10, fields[1].QuickParse() * 10, fields[2].QuickParse() * 10);
            }
            MoveTransform[] moveTransforms = valuePool.GetValues<GameObject>().Select(n => n.GetComponent<MoveTransform>()).Where(n => n).ToArray();
            foreach (var moveTransform in moveTransforms)
            {
                moveTransform.velocity += new Vector3(fields[0].QuickParse(), fields[1].QuickParse(), fields[2].QuickParse());
            }
            yield return savedNode.TriggerConnection(nodeRunner);
            
        }
    }
}