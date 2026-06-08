using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace IDK.NodeScripts
{
    public class FreezeNode : IBehaviorNode
    {
        public override IEnumerator RunNode(LegacySavedNode savedNode, Unit unit, List<NodeComponent.LegacyConnection> connections, string[] fields, NodeRunner nodeRunner)
        {
            Unit[] units = connections.GetNode(NodeBlueprint.ConnectionClass.ReciveUnit).GetValuePoolSmart(unit).GetValues<Unit>();
            foreach (var unitIndex in units)
            {
                unitIndex.gameObject.AddComponent<FreezeBody>().Freeze();
                unitIndex.gameObject.AddComponent<UnitDontWalkFor>().time = fields[0].QuickParse();
                unitIndex.gameObject.GetComponent<UnitDontWalkFor>().Go();
                unitIndex.gameObject.AddComponent<StopAttacks>().StopAttacksFor(fields[0].QuickParse());
            }

            yield return savedNode.TriggerConnection(nodeRunner);
            yield return new WaitForSeconds(fields[0].QuickParse());
            foreach (var unitIndex in units)
            {
                unitIndex.gameObject.AddComponent<FreezeBody>().UnFreeze();
            }
            

        }
    }
}