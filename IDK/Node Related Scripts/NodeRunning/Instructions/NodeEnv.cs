using AC.Node_Related_Scripts.connection_stuff;
using AC.Node_Related_Scripts.ConnectionStuff;
using AC.Node_Related_Scripts.NodeRunning.Instructions;
using AC.NodeScripts;
using Landfall.TABS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Unity.Properties.PropertyPath;

namespace AC.Node_Related_Scripts.NodeRunning
{
    public class NodeEnv
    {
        public NodeRunner runner;
        public ValuePool myPool;
        public Unit unit;
        public NodeContext nodeContext;
        public NodeContext.House house;
        public VirtualNode me;
        public ComponentCacheSystem cacheSystem;
        public NodeEnv(NodeRunner nodeRunner, Unit unit, global::AC.VirtualNode savedNode, NodeContext.House house, NodeContext nodeContext,ConnectionType[] connections = null)
        {
            me = savedNode;
            this.unit = unit; 
            runner = nodeRunner;
            this.nodeContext = nodeContext;
            this.house = house;
            this.cacheSystem = runner.cacheSystem;
        }
        public NodeContext.FurnitureEnumerable GetValues(ConnectionType.VirtualPort port)
        {
            return nodeContext.GetFurnitures(port);
        }
        public NodeContext.FurnitureEnumerable GetValues(NodeBlueprint.ConnectionClass connectionClass, int portName = 0)
        {
            return GetValues(me.GetPort(connectionClass, portName));
        }
        public void AddValue(ConnectionType.VirtualPort port, object obj)
        {
            if (obj is null)
                return;
            nodeContext.AddFurnitureToRoom(port, nodeContext.AllocateObject(obj));
        }
        public void AddValue(NodeBlueprint.ConnectionClass connectionClass, object val, int portName = 0)
        {
            AddValue(me.GetPort(connectionClass, portName), val);
        }
        public void AddValues(NodeBlueprint.ConnectionClass connectionClass, object[] val, int portName = 0)
        {
            for (int i = 0; i < val.Length; i++)
            {
                AddValue(me.GetPort(connectionClass, portName), val[i]);
            }
            
        }
        public void ClearValue(ConnectionType.VirtualPort port)
        {
            nodeContext.ClearFurnitureOfRoom(port);
        }
        public void ClearValue(NodeBlueprint.ConnectionClass connectionClass, int portName = 0)
        {
            ClearValue(me.GetPort(connectionClass, portName));
        }
        public void ClearValues()
        {
            for (int i = 0; i < me.connectionTypes.Count; i++)
            {
                ClearValue(me.connectionTypes[i].first);
            }
        }
        public string GetField(int index)
        {
            return me.fields[index].value;
        }
        public NodeProgram.NodeCore GetTriggerCore()
        {
            return runner.nodeProgram.nodeProccesses.Find(n => n.root == me);
        }
        public void RunTrigger()
        {
            runner.RunCore(GetTriggerCore());
        }
    }
}
