using Landfall.TABC;
using Landfall.TABS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using static RootMotion.FinalIK.RagdollUtility;

namespace IDK.NodeScripts
{
    public class AddForceNode : IBehaviorNode
    {
        public override IEnumerator RunNode(SavedNode savedNode, Unit unit, List<Node.Connection> connections, string[] fields, NodeRunner nodeRunner)
        {
            
            try
            {
                ValuePool valuePool = connections.GetNode(NodeBlueprint.ConnectionType.ReciveGameObject).GetValuePoolSmart(unit);
                Rigidbody[] rbs = valuePool.GetValues<Rigidbody>();
                foreach (var rigidbody in rbs)
                {
                    rigidbody.AddForce(rigidbody.transform.forward * fields[0].QuickParse() * 10);
                    rigidbody.AddForce(rigidbody.transform.up * fields[1].QuickParse() * 10);
                    rigidbody.AddForce(rigidbody.transform.right * fields[2].QuickParse() * 10);
                }
                MoveTransform[] moveTransforms = valuePool.GetValues<GameObject>().Select(n => n.GetComponent<MoveTransform>()).Where(n => n).ToArray();
                foreach (var moveTransform in moveTransforms)
                {
                    moveTransform.velocity += moveTransform.transform.forward * fields[0].QuickParse();
                    moveTransform.velocity += moveTransform.transform.up * fields[1].QuickParse();
                    moveTransform.velocity += moveTransform.transform.right * fields[2].QuickParse();
                }

            }
            catch
            {

            }
            yield return savedNode.TriggerConnection(nodeRunner);
            
        }
    }
}