using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using TFBGames;
using UnityEngine;

namespace IDK.NodeScripts
{
    public class AddEffectNode : IBehaviorNode
    {
        public override IEnumerator RunNode(LegacySavedNode savedNode, Unit unit, List<NodeComponent.LegacyConnection> connections, string[] fields, NodeRunner nodeRunner)
        {
            
            Unit[] units = connections.GetNode(NodeBlueprint.ConnectionClass.ReciveUnit).GetValuePoolSmart(unit).GetValues<Unit>();
            for (int i = 0; i < units.Length; i++)
            {
                var effectt = UnitEffectBase.AddEffectToTarget(units[i].gameObject, AbilityCreator.effects[fields[0]].GetComponent<UnitEffectBase>());
                effectt.transform.position = units[i].transform.position;
                if (effectt.GetComponent<UnitEffectBase>())
                {
                    effectt.GetComponent<UnitEffectBase>().DoEffect();
                }
            }
            yield return savedNode.TriggerConnection(nodeRunner);

        }
    }
}