using Landfall.TABC;
using Landfall.TABS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace IDK.NodeScripts
{
    public class AddGlobalForceNode : IBehaviorNode
    {
        public override IEnumerator RunNode(SavedNode savedNode, Unit unit, List<Node.Connection> connections, string[] fields, NodeRunner nodeRunner)
        {
            
            Rigidbody[] rbs = connections.GetNode(NodeBlueprint.ConnectionType.ReciveGameObject).GetValuePoolSmart(unit).GetValues<Rigidbody>();
            foreach (var rigidbody in rbs)
            {
                rigidbody.AddForce(fields[0].QuickParse() * 10,0,0);
                rigidbody.AddForce(0,fields[1].QuickParse() * 10,0);
                rigidbody.AddForce(0,0,fields[2].QuickParse() * 10);
            }
            yield return savedNode.TriggerConnection(nodeRunner);
            
        }
    }
}