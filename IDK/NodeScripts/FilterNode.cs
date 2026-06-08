using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace IDK.NodeScripts
{
    public class FilterNode : IValueNode
    {
        public override bool IsDynamic()
        {
            return true;
        }
        public override ValuePool GetDynamicValue(LegacySavedNode savedNode, Unit unit, List<NodeComponent.LegacyConnection> connections, string[] fields)
        {
            object[] everything = connections.GetNode(NodeBlueprint.ConnectionClass.ReciveAnything).GetValuePoolSmart(unit).GetValues<object>();
            ValuePool valuePool = new ValuePool();
            if (fields[0] == "Gameobjects only")
                valuePool.AddRange(everything.Where(n => n is GameObject).ToArray());
            if (fields[0] == "Units only")
                valuePool.AddRange(everything.Where(n => n is Unit).ToArray());
            if (fields[0] == "Components only")
                valuePool.AddRange(everything.Where(n => n is Component).ToArray());
            if (fields[0] == "Other")
                valuePool.AddRange(everything.Where(n => !(n is Component)).Where(n => !(n is Unit)).Where(n => !(n is GameObject)).ToArray());
            return valuePool;
        }
        public override ValuePool GetValuePool(LegacySavedNode savedNode, Unit unit, List<NodeComponent.LegacyConnection> connections, string[] fields)
        {
            return null;
        }

    }
}