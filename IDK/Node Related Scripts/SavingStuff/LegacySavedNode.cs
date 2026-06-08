using HarmonyLib;
using IDK.NodeScripts;
using Landfall.TABS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;

namespace IDK
{
    public class LegacySavedNode : MonoBehaviour
    {
        public Dictionary<Unit, ValuePool> valuePools = new Dictionary<Unit, ValuePool>();
        public Dictionary<Unit, NodeRunner> nodeRunners = new Dictionary<Unit, NodeRunner>();
        public ValuePool GetValuePool(Unit unit)
        {
            if (!valuePools.ContainsKey(unit))
                valuePools.Add(unit, new ValuePool());
            return valuePools[unit];
        }
        public NodeComponent corispondingNode;
        public string blueprintName;
        public NodeBlueprint Blueprint { get 
            {
                return AbilityCreator.nodeDatabase[blueprintName];
            } }
        public object m_instance;
        public object InstanceFunction
        {
            get
            {
                if (m_instance == null)
                    m_instance = Activator.CreateInstance(Blueprint.nodeFunction); ;
                return m_instance;
            }
        }
        public List<NodeComponent.LegacyConnection> connections = new List<NodeComponent.LegacyConnection>();
        public List<string> fields = new List<string>();
        public Vector3 position;
        public override string ToString()
        {
            return $"{Blueprint.Name} ({GetNodeInstanceID()})";
        }
        public int GetNodeInstanceID() 
        {
            var objects = FindObjectsOfType<LegacySavedNode>();
            for (int i = 0; i < objects.Length; i++)
            {
                if (objects[i] == this)
                {
                    return i + 1;
                }
            }
            return 0;
        }
        public static LegacySavedNode Instance()
        {
            var gameobj = new GameObject("Saved");
            DontDestroyOnLoad(gameobj);
            var result = gameobj.AddComponent<LegacySavedNode>();
            return result;
        }
        
    }
}