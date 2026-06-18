using AC.Node_Related_Scripts.NodeRunning;
using AC.Node_Related_Scripts.NodeRunning.Instructions.Courtines;
using Landfall.TABS;
using System.Collections.Generic;

namespace AC.NodeScripts
{
    public class WhenAbilityTriggered : ITriggerNode
    {
        public IEnumerator<CoroutineReturn> Execute(NodeEnv env)
        {
            return null;
        }
        public IEnumerator<CoroutineReturn> Cache(NodeEnv env)
        {
            float range = env.GetField(1).QuickParse();
            float cooldown = env.GetField(0).QuickParse();
            ConditionalEvent conditionalEvent = env.runner.gameObject.AddComponent<ConditionalEvent>();
            conditionalEvent.events = new ConditionalEventInstance[]
            {
                new ConditionalEventInstance()
                {
                    continuousEvent = new UnityEngine.Events.UnityEvent(),
                    turnOffEvent = new UnityEngine.Events.UnityEvent(),
                        turnOnEvent = new UnityEngine.Events.UnityEvent(),
                        delay = 0,
                        conditions = new EventCondition[]
                        {
                            new EventCondition()
                            {
                                conditionType = EventCondition.ConditionType.Cooldown,
                                startOnCD = false,
                                value = cooldown,
                                counter = cooldown,
                            },
                            new EventCondition()
                            {
                                startOnCD = false,
                                conditionType = EventCondition.ConditionType.UnitDistanceToTarget,
                                whichRange = EventCondition.WhichRange.UnitRange,
                                value = range,
                                valueType = EventCondition.ValueType.Max,
                            }

                        }
                }
            };
            conditionalEvent.events[0].continuousEvent.AddListener(env.RunTrigger);
            return null;
        }

    }
}