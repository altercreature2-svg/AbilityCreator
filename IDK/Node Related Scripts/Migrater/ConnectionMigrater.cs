using IDK.Node_Related_Scripts.ConnectionStuff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDK.Node_Related_Scripts.Migrater
{
    public class ConnectionMigrater
    {
        public static ConnectionType MigrateToNew(NodeBlueprint.ConnectionClass connectionTypeEnum, int other)
        {
            ConnectionType connectionType = new ConnectionType();
            connectionType.portName = "A";
            connectionType.connectedNodePortName = "A";
            connectionType.connectedNode = other;
            connectionType.connectionType = connectionTypeEnum;
            return connectionType;
        }
    }
}
