using IDK.Node_Related_Scripts.ConnectionStuff;
using IDK.Node_Related_Scripts.NodeRunning;
using Sirenix.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace IDK.Node_Related_Scripts
{
    public static class NodeUtility
    {
        public static T[] GetAllValues<T>(NodeEnv env, ConnectionType connectionType)
        {
            return new T[0];
        }
        public static Task ContinueBranch(NodeEnv env)
        {
            return env.runner.TriggerConnections(env.me);
        }
        public static void StoreInConnection<T>(NodeEnv env, ConnectionType connectionType , T obj)
        {
            env.nodeContext.InsertFurniture(connectionType, obj);
        }
        public static ConnectionType GetConnectionType(NodeEnv env, NodeBlueprint.ConnectionClass connectionClass )
        {
            return env.nodeContext.GetConnection(env.house, connectionClass);
        }
        public static ConnectionType GetConnectionType(NodeEnv env, string portName)
        {
            return env.nodeContext.GetConnection(env.house, portName);
        }
    }
}