using Landfall.TABS;
using System.Collections.Generic;

namespace IDK.NodeScripts
{
    public class UnitWasAttacked : ITriggerNode
    {
        public override void EveryFrame(SavedNode savedNode, Unit unit, List<Node.Connection> connections, string[] fields, NodeRunner nodeRunner)
        {
            
        }
        public override void StartFrame(SavedNode savedNode, Unit unit, List<Node.Connection> connections, string[] fields, NodeRunner nodeRunner)
        {
            bool startonCD = true;
            //startonCD = (fields[2] != "Don't");
            ConditionalEvent conditionalEvent = nodeRunner.gameObject.AddComponent<ConditionalEvent>();
            conditionalEvent.events = new ConditionalEventInstance[]
            {
                new ConditionalEventInstance()
                {
                    turnOnEvent = new UnityEngine.Events.UnityEvent(),
                    turnOffEvent = new UnityEngine.Events.UnityEvent(),
                        continuousEvent = new UnityEngine.Events.UnityEvent(),
                        delay = 0,
                        conditions = new EventCondition[]
                        {
                            new EventCondition()
                            {
                                conditionType = EventCondition.ConditionType.Cooldown,
                                startOnCD = startonCD,
                                value = fields[0].QuickParse(),
                                counter = fields[0].QuickParse(),
                            },
                            new EventCondition()
                            {
                                conditionType = EventCondition.ConditionType.UnitWasAttacked,
                                
                            },
                            new EventCondition()
                            {
                                startOnCD = startonCD,
                                conditionType = EventCondition.ConditionType.UnitDistanceToTarget,
                                whichRange = EventCondition.WhichRange.UnitRange,
                                value = savedNode.fields[1].QuickParse(),
                                valueType = EventCondition.ValueType.Max,
                            }

                        }
                }
            };
            conditionalEvent.events[0].continuousEvent.AddListener(() => nodeRunner.StartCoroutine(nodeRunner.TriggerConnection(savedNode)));
        }
    }

}