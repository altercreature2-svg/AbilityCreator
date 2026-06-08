using Landfall.TABS;
using System.Collections.Generic;
using UnityEngine;

namespace IDK.NodeScripts
{
    public class WhenUnitDamaged : ITriggerNode
    {
        public override void EveryFrame(LegacySavedNode savedNode, Unit unit, List<NodeComponent.LegacyConnection> connections, string[] fields, NodeRunner nodeRunner)
        {

        }
        public override void StartFrame(LegacySavedNode savedNode, Unit unit, List<NodeComponent.LegacyConnection> connections, string[] fields, NodeRunner nodeRunner)
        {
            void Assign()
            {
                ValuePool valuePool = savedNode.GetValuePool(unit);
                valuePool.ClearValues();
                Unit unit1 = unit.data.targetData.unit;
                valuePool.AddValue(unit1);
                valuePool.AddValue(unit1.data.mainRig);
                valuePool.AddValue(unit1.gameObject);
                nodeRunner.StartCoroutine(nodeRunner.TriggerConnection(savedNode));
            }
            unit.data.healthHandler.AssignDamageAction(Assign);
        }
        public override ValuePool GetValuePool(LegacySavedNode savedNode, Unit unit, List<NodeComponent.LegacyConnection> connections, string[] fields)
        {
            return savedNode.GetValuePool(unit);
        }
    }

}