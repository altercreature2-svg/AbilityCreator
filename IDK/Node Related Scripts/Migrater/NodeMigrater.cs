using IDK.Node_Related_Scripts.ConnectionStuff;
using IDK.Node_Related_Scripts.Field_stuff;
using IDK.Node_Related_Scripts.SavingStuff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDK.Node_Related_Scripts.Migrater
{
    public class NodeMigrater
    {

        public static VirtualNode GetNewNode(LegacySavedNode legacySavedNode)
        {
            VirtualNode virtualNode = new VirtualNode();
            virtualNode.nodeBlueprint = legacySavedNode.blueprintName;
            virtualNode.editorPostion = legacySavedNode.position;
            List<VirtualNodeField> virtualNodeFields = new List<VirtualNodeField>();
            for (int i = 0; i < legacySavedNode.fields.Count; i++)
            {
                virtualNodeFields.Add(new VirtualNodeField(i, legacySavedNode.fields[i]));
            }
            virtualNode.fields = virtualNodeFields.ToArray();
            List<ConnectionType> connectionTypes = new List<ConnectionType>();
            for (int i = 0; i < legacySavedNode.connections.Count; i++)
            {
                connectionTypes.Add(ConnectionMigrater.MigrateToNew(
                    legacySavedNode.connections[i].connectionsType,
                    legacySavedNode.connections[i].savedNode.GetInstanceID()
                ));
            }
            virtualNode.connectionTypes = connectionTypes.ToArray();

            
            return virtualNode;
        }
    }
}
