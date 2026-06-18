using Landfall.TABS;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace AC.Help_Componets
{
    public class FunctionStorer : MonoBehaviour
    {
        public class NodeAction
        {
            public string name;
            public UnityEvent @event = new UnityEvent();
        }
        public List<NodeAction> actions = new List<NodeAction>();
        public void RunAction(string action)
        {
            actions.Find(n => n.name == action)?.@event?.Invoke();
        }
    }
}