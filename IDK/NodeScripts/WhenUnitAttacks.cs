using Landfall.TABS;
using System.Collections.Generic;

namespace IDK.NodeScripts
{
    public class WhenUnitAttacks : ITriggerNode
    {
        public override ValuePool GetValuePool(LegacySavedNode savedNode, Unit unit, List<NodeComponent.LegacyConnection> connections, string[] fields)
        {
            return savedNode.GetValuePool(unit);
        }
        public override void EveryFrame(LegacySavedNode savedNode, Unit unit, List<NodeComponent.LegacyConnection> connections, string[] fields, NodeRunner nodeRunner)
        {

        }
        public override void StartFrame(LegacySavedNode savedNode, Unit unit, List<NodeComponent.LegacyConnection> connections, string[] fields, NodeRunner nodeRunner)
        {
            void AttackStarted(Unit unit2)
            {
                savedNode.GetValuePool(unit).ClearValues();
                savedNode.GetValuePool(unit).AddValue(unit2);
                savedNode.GetValuePool(unit).AddValue(unit2.gameObject);
                savedNode.GetValuePool(unit).AddValue(unit2.data.mainRig);
                nodeRunner.StartCoroutine(nodeRunner.TriggerConnection(savedNode));
            }
            unit.data.weaponHandler.AttackStarted += ((n, d4, d3, d2, d) => AttackStarted(n.data.targetData.unit));
        }
    }

}