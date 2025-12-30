using Landfall.TABS;
using System.Collections.Generic;

namespace IDK.NodeScripts
{
    public class WhenUnitDamages : ITriggerNode
    {
        public override void EveryFrame(SavedNode savedNode, Unit unit, List<Node.Connection> connections, string[] fields, NodeRunner nodeRunner)
        {

        }
        public override void StartFrame(SavedNode savedNode, Unit unit, List<Node.Connection> connections, string[] fields, NodeRunner nodeRunner)
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
        public override ValuePool GetValuePool(SavedNode savedNode, Unit unit, List<Node.Connection> connections, string[] fields)
        {
            return savedNode.GetValuePool(unit);
        }
    }

}