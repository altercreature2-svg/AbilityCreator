using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IDK.NodeScripts
{
    public class ForEachNode : IBehaviorNode
    {
        
        public override IEnumerator RunNode(SavedNode savedNode, Unit unit, List<Node.Connection> connections, string[] fields, NodeRunner nodeRunner)
        {
            object[] objects;
            if (fields[1] == "All")
                objects = connections.GetNode(NodeBlueprint.ConnectionType.ReciveAnything).GetValuePoolSmart(unit).GetValues<object>();
            else if (fields[1] == "Gameobjects only")
                objects = connections.GetNode(NodeBlueprint.ConnectionType.ReciveAnything).GetValuePoolSmart(unit).GetValues<GameObject>();
            else if (fields[1] == "Units only")
                objects = connections.GetNode(NodeBlueprint.ConnectionType.ReciveAnything).GetValuePoolSmart(unit).GetValues<Unit>();
            else
                objects = connections.GetNode(NodeBlueprint.ConnectionType.ReciveAnything).GetValuePoolSmart(unit).GetValues<Component>();

            ValuePool valuePool = savedNode.GetValuePool(unit);
            for (int i = 0; i < objects.Length; i++)
            {
                valuePool.ClearValues();
                if (savedNode.fields[0].QuickParse() <= 0)
                    yield return null;
                else
                    yield return new WaitForSeconds(savedNode.fields[0].QuickParse());
                valuePool.AddValue(objects[i]);
                yield return savedNode.TriggerConnection(nodeRunner);
            }
        }
        public override ValuePool GetValuePool(SavedNode savedNode, Unit unit, List<Node.Connection> connections, string[] fields)
        {
            return savedNode.GetValuePool(unit);
        }
    }
}