using AC.Node_Related_Scripts.ConnectionStuff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AC.Node_Related_Scripts.Migrater
{
    public class ConnectionMigrater
    {
        public static ConnectionType MigrateToNew(NodeBlueprint.ConnectionClass connectionTypeEnum, int other, int parent)
        {
            ConnectionType connectionType = new ConnectionType()
            {
                first = new ConnectionType.VirtualPort()
                {
                    connectedNode = parent,
                    portIndex = 0, // cause the old ones dont have more than dat
                },
                second = new ConnectionType.VirtualPort()
                {
                    connectedNode = other,
                    portIndex = 0, // cause the old ones dont have more than dat
                },
                connectionClass = connectionTypeEnum,
            };
            return connectionType;
        }
    }
}
