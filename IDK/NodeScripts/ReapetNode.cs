using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IDK.NodeScripts
{
    public class ReapetNode : IBehaviorNode
    {
        public override IEnumerator RunNode(SavedNode savedNode, Unit unit, List<Node.Connection> connections, string[] fields, NodeRunner nodeRunner)
        {
            for (int i = 0; i < (int)fields[0].QuickParse(); i++)
            {
                if (savedNode.fields[1].QuickParse() == 0)
                    yield return null;
                else if (savedNode.fields[1].QuickParse() < 0) { }
                else
                    yield return new WaitForSeconds(savedNode.fields[1].QuickParse());
                yield return savedNode.TriggerConnection(nodeRunner);
            }
            
            
        }
    }
}