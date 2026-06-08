using IDK.Node_Related_Scripts.connection_stuff;
using IDK.Node_Related_Scripts.ConnectionStuff;
using IDK.NodeScripts;
using Landfall.TABS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDK.Node_Related_Scripts.NodeRunning
{
    public class NodeEnv
    {
        public NodeRunner runner;
        public ValuePool myPool;
        public Unit unit;
        public NodeContext nodeContext;
        public NodeContext.House house;
        public IDK.VirtualNode me;
        public NodeEnv(NodeRunner nodeRunner, Unit unit, IDK.VirtualNode savedNode, NodeContext.House house, NodeContext nodeContext,ConnectionType[] connections = null)
        {
            me = savedNode;
            this.unit = unit; 
            runner = nodeRunner;
            this.nodeContext = nodeContext;
            this.house = house;
        }
        public List<NodeContext.Furniture> GetValues(ConnectionType.VirtualPort port)
        {
            return house.rooms.First(n => n.connectionType.first == port).furnitures;
        }
    }
}
