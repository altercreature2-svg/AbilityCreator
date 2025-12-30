using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IDK.Node_Related_Scripts.connection_stuff
{
    [Serializable]
    public class NodeConnections
    {
        [SerializeField] public List<NodeBlueprint.ConnectionType> connectionsTypes = new List<NodeBlueprint.ConnectionType>();
        [SerializeField] public List<int> otherIDs = new List<int>();
    }
}