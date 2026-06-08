using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IDK.NodeScripts
{
    public class WhenUnitSpawned : ITriggerNode
    {
        public override void StartFrame(LegacySavedNode savedNode, Unit unit, List<NodeComponent.LegacyConnection> connections, string[] fields, NodeRunner nodeRunner)
        {
           nodeRunner.StartCoroutine(savedNode.TriggerConnection(nodeRunner));
        }
        public override void EveryFrame(LegacySavedNode savedNode, Unit unit, List<NodeComponent.LegacyConnection> connections, string[] fields, NodeRunner nodeRunner)
        {
        }
        
    }
}