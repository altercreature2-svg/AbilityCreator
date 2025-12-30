using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IDK.NodeScripts
{
    public class DoEveryNode : ITriggerNode
    {
        public override void EveryFrame(SavedNode savedNode, Unit unit, List<Node.Connection> connections, string[] fields, NodeRunner nodeRunner)
        {
            
        }
        public override void StartFrame(SavedNode savedNode, Unit unit, List<Node.Connection> connections, string[] fields, NodeRunner nodeRunner)
        {
            nodeRunner.StartCoroutine(IntervalLoop(nodeRunner, savedNode, fields[0].QuickParse()));
        }
        public IEnumerator IntervalLoop(NodeRunner nodeRunner,SavedNode savedNode, float seconds)
        {

            yield return new WaitForSeconds(seconds);
            nodeRunner.StartCoroutine(nodeRunner.TriggerConnection(savedNode));
            nodeRunner.StartCoroutine(IntervalLoop(nodeRunner,savedNode, seconds));
        }

    }
    
}