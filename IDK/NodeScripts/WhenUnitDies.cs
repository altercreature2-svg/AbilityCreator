using Landfall.TABS;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IDK.NodeScripts
{
    public class WhenUnitDies : ITriggerNode
    {
        public override void EveryFrame(SavedNode savedNode, Unit unit, List<Node.Connection> connections, string[] fields, NodeRunner nodeRunner)
        {
            
        }
        public override void StartFrame(SavedNode savedNode, Unit unit, List<Node.Connection> connections, string[] fields, NodeRunner nodeRunner)
        {
            Action action = (Action)Delegate.Combine((Action)(() => nodeRunner.StartCoroutine(nodeRunner.TriggerConnection(savedNode))),(Action)unit.data.healthHandler.GetField("DieAction"));
            unit.data.healthHandler.SetField("DieAction", action);
        }
        
    }
    
}