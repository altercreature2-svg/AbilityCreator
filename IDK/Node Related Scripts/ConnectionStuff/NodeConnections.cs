using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AC.Node_Related_Scripts.connection_stuff
{
    [Serializable]
    public class NodeConnections
    {
        [SerializeField] public List<NodeBlueprint.ConnectionClass> connectionsTypes = new List<NodeBlueprint.ConnectionClass>();
        [SerializeField] public List<int> otherIDs = new List<int>();
    }
}