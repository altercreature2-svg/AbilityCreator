

using IDK.Help_Componets;
using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace IDK.NodeScripts
{
    public class RunFunctionNode : IBehaviorNode
    {
        public override IEnumerator RunNode(SavedNode savedNode, Unit unit, List<Node.Connection> connections, string[] fields, NodeRunner nodeRunner)
        {
            FunctionStorer functionStorer;
            if (!unit.GetComponent<FunctionStorer>())
            {
                functionStorer = unit.gameObject.AddComponent<FunctionStorer>();
            }
            else
            {
                functionStorer = unit.GetComponent<FunctionStorer>();
            }
            functionStorer.RunAction(fields[0]);
            yield return savedNode.TriggerConnection(nodeRunner);
            yield return null;
        }
    }
}