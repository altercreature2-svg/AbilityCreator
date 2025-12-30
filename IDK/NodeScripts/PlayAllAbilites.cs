using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IDK.NodeScripts
{
    public class PlayAllAbilites : IBehaviorNode
    {
        public override IEnumerator RunNode(SavedNode savedNode, Unit unit, List<Node.Connection> connections, string[] fields, NodeRunner nodeRunner)
        {
            Unit[] units = connections.GetNode(NodeBlueprint.ConnectionType.ReciveUnit).GetValuePoolSmart(unit).GetValues<Unit>();
            for (int o = 0; o < units.Length; o++)
            {
                var Condentials = units[o].GetComponentsInChildren<ConditionalEvent>();
                for (int s = 0; s < Condentials.Length; s++)
                {
                    for (int n = 0; n < Condentials[s].events.Length; n++)
                    {
                        Condentials[s].events[n].continuousEvent.Invoke();

                    }
                }
            }
            yield return savedNode.TriggerConnection(nodeRunner);

        }
    }
}