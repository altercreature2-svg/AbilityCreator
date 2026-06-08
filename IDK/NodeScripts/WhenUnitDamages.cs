using Landfall.TABS;
using System.Collections.Generic;

namespace IDK.NodeScripts
{
    public class WhenUnitDamages : ITriggerNode
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
                valuePool.AddValue(unit.data.targetData.unit);
                valuePool.AddValue(unit.data.targetMainRig);
                valuePool.AddValue(unit.data.targetData.unit.gameObject);
                nodeRunner.StartCoroutine(nodeRunner.TriggerConnection(savedNode));
            }
            
        }
        public override ValuePool GetValuePool(LegacySavedNode savedNode, Unit unit, List<NodeComponent.LegacyConnection> connections, string[] fields)
        {
            return savedNode.GetValuePool(unit);
        }
    }

}