using AC.Node_Related_Scripts.connection_stuff;
using AC.Node_Related_Scripts.ConnectionStuff;
using AC.Node_Related_Scripts.Field_stuff;
using AC.Node_Related_Scripts.SavingStuff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Networking.Types;

namespace AC.Node_Related_Scripts.Migrater
{
    public class NodeMigrater
    {

        public static VirtualNode GetNewNode(LegacySavedNode legacySavedNode)
        {
            VirtualNode virtualNode = new VirtualNode();
            virtualNode.nodeBlueprint = legacySavedNode.blueprintName;
            virtualNode.editorPositon = legacySavedNode.position;
            List<VirtualNodeField> virtualNodeFields = new List<VirtualNodeField>(legacySavedNode.fields.Count);
            for (int i = 0; i < legacySavedNode.fields.Count; i++)
            {
                virtualNodeFields.Add(new VirtualNodeField(i, legacySavedNode.fields[i]));
            }
            virtualNode.fields = virtualNodeFields;
            List<ConnectionType> connectionTypes = new List<ConnectionType>(legacySavedNode.connections.Count);
            for (int i = 0; i < legacySavedNode.connections.Count; i++)
            {
                connectionTypes.Add(ConnectionMigrater.MigrateToNew(legacySavedNode.connections[i].connectionsType, legacySavedNode.connections[i].savedNode.GetNodeInstanceID(), legacySavedNode.GetNodeInstanceID()));
            }
            virtualNode.connectionTypes = connectionTypes;

            
            return virtualNode;
        }
    }
}
