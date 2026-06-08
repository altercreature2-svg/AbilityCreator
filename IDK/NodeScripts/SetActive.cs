using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IDK.NodeScripts
{
    public class SetActive : IBehaviorNode
    {
        public override IEnumerator RunNode(LegacySavedNode savedNode, Unit unit, List<NodeComponent.LegacyConnection> connections, string[] fields, NodeRunner nodeRunner)
        {
            GameObject[] objects = connections.GetNode(NodeBlueprint.ConnectionClass.ReciveGameObject).GetValuePoolSmart(unit).GetValues<GameObject>();

            foreach (var @object in objects)
            {
                try
                {
                    if (fields[0] == "Toggle")
                        @object.SetActive(!@object.activeSelf);
                    if (fields[0] == "True")
                        @object.SetActive(true);
                    if (fields[0] == "False")
                        @object.SetActive(false);

                }
                catch (System.Exception)
                {
                }
            }
            yield return savedNode.TriggerConnection(nodeRunner);
        }
    }
}