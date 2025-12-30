using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using TFBGames;
using UnityEngine;

namespace IDK.NodeScripts
{
    public class AddEffectNode : IBehaviorNode
    {
        public override IEnumerator RunNode(SavedNode savedNode, Unit unit, List<Node.Connection> connections, string[] fields, NodeRunner nodeRunner)
        {
            
            Unit[] units = connections.GetNode(NodeBlueprint.ConnectionType.ReciveUnit).GetValuePoolSmart(unit).GetValues<Unit>();
            for (int i = 0; i < units.Length; i++)
            {
                var effectt = Object.Instantiate(Main.Effects[fields[0]], units[i].transform);
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