using IDK.NodeScripts;
using Landfall.TABS;
using MonoMod.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static RootMotion.FinalIK.IKSolver;
namespace IDK
{
    public static class Utility
    {
        public static string[] SplitSafeSafe(this string s, string c)
        {
            List<string> strings = new List<string>();
            string curr = "";
            for (int i = 0; i < s.ToCharArray().Length; i++)
            {
                if (s.ToCharArray()[i] == c.ToCharArray()[0])
                {
                    strings.Add(curr);
                    curr = "";
                }
                else
                {
                    curr += s.ToCharArray()[i];
                }
            }
            if (curr != "")
            {
                strings.Add(curr);
            }
            return strings.ToArray();

        }
        
        public static LegacySavedNode GetNode(this List<NodeComponent.LegacyConnection> ts, NodeBlueprint.ConnectionClass connectionType)
        {
            for (int i = 0; i < ts.Count; i++)
            {
                if (ts[i].connectionsType == connectionType)
                    return ts[i].savedNode;
            }
            return null;
        }
        public static float QuickParse(this string str)
        {
            bool b = float.TryParse(str, out float s);
            if (b)
                return s;
            else
                return 0;
        }
        public static ValuePool GetValuePoolSmart (this LegacySavedNode savedNode, Unit unit)
        {
            try
            {
                Debug.Log($"Accsessing value of node {savedNode?.Blueprint?.Name} with function of {savedNode.Blueprint?.nodeFunction} ...");
                if (savedNode == null)
                {
                    Debug.Log("NULL ALERT");
                    return null;
                }
                if (savedNode.Blueprint.nodeFunction == null)
                {
                    Debug.Log("No nodeFunction :(");
                }
                if (savedNode.Blueprint.nodeFunction.BaseType == typeof(IValueNode))
                {
                    IValueNode valueNode = (IValueNode)savedNode.InstanceFunction;
                    if (valueNode.IsDynamic())
                    {
                        Debug.Log("Handing value! (its dynamic!)");
                        ValuePool vp = valueNode.GetDynamicValue(savedNode, unit, savedNode.connections, savedNode.fields.ToArray());
                        Debug.Log(vp.GetValues<object>().Length + " objects");
                        return vp;
                    }
                    else
                    {
                        Debug.Log("Handing value! (its static!)");
                        if (savedNode.valuePools.ContainsKey(unit))
                        {
                            return savedNode.GetValuePool(unit);
                        }
                        else
                        {
                            ValuePool valuePool = valueNode.GetValuePool(savedNode, unit, savedNode.connections, savedNode.fields.ToArray());
                            if (!savedNode.valuePools.ContainsKey(unit))
                                savedNode.valuePools.Add(unit, valuePool);
                            return valuePool;
                        }
                    }
                }
                else if (savedNode.Blueprint.nodeFunction.BaseType == typeof(IBehaviorNode))
                {
                    IBehaviorNode behaviorNode = (IBehaviorNode)savedNode.InstanceFunction;
                    return behaviorNode.GetValuePool(savedNode, unit, savedNode.connections, savedNode.fields.ToArray());
                }
                else if (savedNode.Blueprint.nodeFunction.BaseType == typeof(ITriggerNode))
                {
                    ITriggerNode behaviorNode = (ITriggerNode)savedNode.InstanceFunction; ;
                    return behaviorNode.GetValuePool(savedNode, unit, savedNode.connections, savedNode.fields.ToArray());
                }
            }

            catch (Exception e) 
            {
                e.LogDetailed();
                Debug.Log("(Utility GetValuePoolSmart) Something went wrong!" + "\n" + $" Extra info: saved node:{savedNode.Blueprint.Name} ({savedNode.GetNodeInstanceID()}) , unit: {unit}"); return null; 
            }
            return null;
        }
        public static IEnumerator TriggerConnection(this LegacySavedNode savedNode, NodeRunner nodeRunner, int pause = 0)
        {
            for (int i = 0; i < pause; i++)
            {
                yield return null;
            }
            LegacySavedNode nodeToTrigger = savedNode.connections.GetNode(NodeBlueprint.ConnectionClass.Trigger);
            if (nodeToTrigger == null)
                yield break;
            yield return nodeRunner.RunNode(nodeToTrigger);
        }
        public static bool IsChildOf(this Transform transform, Transform other)
        {
            if (transform.parent == null)
            {
                return false;
            }
            Transform parent = transform.parent;
            while (true)
            {
                if (parent == null)
                    return false;
                if (parent == other)
                    return true;
                parent = parent.parent;
            }

        }
    }
}