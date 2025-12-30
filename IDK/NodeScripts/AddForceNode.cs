using Landfall.TABC;
using Landfall.TABS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace IDK.NodeScripts
{
    public class AddForceNode : IBehaviorNode
    {
        public override IEnumerator RunNode(SavedNode savedNode, Unit unit, List<Node.Connection> connections, string[] fields, NodeRunner nodeRunner)
        {

            try
            {
                Rigidbody[] rbs = connections.GetNode(NodeBlueprint.ConnectionType.ReciveGameObject).GetValuePoolSmart(unit).GetValues<Rigidbody>();
                foreach (var rigidbody in rbs)
                {
                    rigidbody.AddForce(rigidbody.transform.forward * fields[0].QuickParse() * 10);
                    rigidbody.AddForce(rigidbody.transform.up * fields[1].QuickParse() * 10);
                    rigidbody.AddForce(rigidbody.transform.right * fields[2].QuickParse() * 10);
                }
                //

            }
            catch
            {

            }
            yield return savedNode.TriggerConnection(nodeRunner);
            
        }
    }
}