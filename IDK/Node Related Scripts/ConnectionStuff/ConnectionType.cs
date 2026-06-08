using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDK.Node_Related_Scripts.ConnectionStuff
{
    [Serializable]
    public class ConnectionType
    {
        [Serializable]
        public class VirtualPort
        {
            public string portName;
            public int connectedNode;
        }
        public VirtualPort first;
        public VirtualPort second;
        public NodeBlueprint.ConnectionClass connectionType;
    }
}
