using Landfall.TABS;
using System.Collections.Generic;

namespace IDK.NodeScripts
{
    public class WhenAbilityTriggered : ITriggerNode
    {
        public override void EveryFrame(LegacySavedNode savedNode, Unit unit, List<NodeComponent.LegacyConnection> connections, string[] fields, NodeRunner nodeRunner)
        {

        }
        public override void StartFrame(LegacySavedNode savedNode, Unit unit, List<NodeComponent.LegacyConnection> connections, string[] fields, NodeRunner nodeRunner)
        {
            bool startonCD = true;
            //startonCD = (fields[2] != "Don't");
            
            ConditionalEvent conditionalEvent = nodeRunner.gameObject.AddComponent<ConditionalEvent>();
            conditionalEvent.events = new ConditionalEventInstance[]
            {
                new ConditionalEventInstance()
                {
                    turnOffEvent = new UnityEngine.Events.UnityEvent(),
                    turnOnEvent = new UnityEngine.Events.UnityEvent(),
                        continuousEvent = new UnityEngine.Events.UnityEvent(),
                        delay = 0,
                        conditions = new EventCondition[]
                        {
                            new EventCondition()
                            {
                                conditionType = EventCondition.ConditionType.Cooldown,
                                startOnCD = startonCD,
                                alwaysResetCounter = true,
                                value = fields[0].QuickParse(),
                                counter = fields[0].QuickParse(),
                                
                            },
                            new EventCondition()
                            {
                                conditionType = EventCondition.ConditionType.UnitDistanceToTarget,
                                whichRange = EventCondition.WhichRange.UnitRange,
                                value = fields[1].QuickParse(),
                                valueType = EventCondition.ValueType.Max,
                                startOnCD = startonCD,
                            }

                        }
                }
            };
            conditionalEvent.events[0].continuousEvent.AddListener(() => nodeRunner.StartCoroutine(nodeRunner.TriggerConnection(savedNode)));
        }
    }

}