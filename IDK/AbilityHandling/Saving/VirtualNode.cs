using HarmonyLib;
using AC.Node_Related_Scripts.connection_stuff;
using AC.Node_Related_Scripts.ConnectionStuff;
using AC.Node_Related_Scripts.Field_stuff;
using AC.Node_Related_Scripts.SavingStuff;
using AC.NodeScripts;
using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

namespace AC
{
    public class VirtualNode : IRegisterable, ISaveable
    {
        public int id;
        public VirtualNode(int id = 0)
        {
            if (id == 0)
                id = Random.Range(0, int.MaxValue - 1);
            this.id = id;
        }

        public SaveableObject Save()
        {
            SaveableObject saveableObject = new SaveableObject();
            saveableObject.typeIdentfier = "%NODE%";
            List<SaveableField> saveableFields = new List<SaveableField>();
            saveableFields.Add(new SaveableField() {
                fieldName = "%BLUEPRINT%",
                fieldValue = nodeBlueprint,
            });
            
            saveableFields.Add(new SaveableField()
            {
                fieldName = "%POSITION%",
                fieldValue = editorPositon.x + "/" + editorPositon.y + "/" + editorPositon.z,
            });

            for (int i = 0; i < connectionTypes.Count; i++)
            {
                saveableFields.Add(new SaveableField()
                {
                    fieldName = "%CONNECTION%" + connectionTypes[i].first.portIndex,
                    fieldValue = Serialize.SaveJson(connectionTypes[i])
                });
            }
            for (int i = 0; i < fields.Count; i++)
            {
                saveableFields.Add(new SaveableField()
                {
                    fieldName = "%FIELD%" + fields[i].fieldID,
                    fieldValue = Serialize.SaveJson(fields[i])
                });
            }

            saveableObject.fields = saveableFields.ToArray();
            return saveableObject;
        }
        public void Load(SaveableObject saveableObject)
        {
            nodeBlueprint = saveableObject.GetSavedField("%BLUEPRINT%");
            string[] positions = saveableObject.GetSavedField("%POSITION%").Split(new string[] { "/"},options: System.StringSplitOptions.RemoveEmptyEntries);

            editorPositon = new Vector3(positions[0].QuickParse(), positions[1].QuickParse(), positions[2].QuickParse());
            List<ConnectionType> connectionTypes = new List<ConnectionType>();
            string[] connectionJsons = saveableObject.GetSavedFields("%CONNECTION%");
            for (int i = 0; connectionJsons.Length > i;i++)
            {
                connectionTypes.Add(Serialize.LoadJson<ConnectionType>(connectionJsons[i]));
            }
            connectionTypes = connectionTypes;

            List<VirtualNodeField> virtualNodeFields = new List<VirtualNodeField>();
            string[] fieldJsons = saveableObject.GetSavedFields("%FIELD%");
            for(int i = 0; fieldJsons.Length > i; i++)
            {
                virtualNodeFields.Add(Serialize.LoadJson<VirtualNodeField>(fieldJsons[i]));
            }
            fields = virtualNodeFields;
        }
        
        public string nodeBlueprint;
        public Vector3 editorPositon;
        //again, im using virtual types here because they already can be serialized
        //there isn't a need to create a serialized variant, much love <3
        public List<VirtualNodeField> fields;
        public List<ConnectionType> connectionTypes = new List<ConnectionType>();

        //some helpers for nodes
        public string GetField(int fieldID)
        {
            return fields.Find(n => n.fieldID == fieldID).value;
        }
        public int[] GetAllConnections()
        {
            return connectionTypes.Select(n => n.second.connectedNode).ToArray();
        }
        public int GetConnection(NodeBlueprint.ConnectionClass connectionType, int port = 0)
        {
            var match = connectionTypes.FirstOrDefault(n =>
                n.connectionClass.HasFlag(connectionType) && n.second.portIndex == port);

            // Safe check: If no match is found, return -1 (or your equivalent "null" node ID)
            return match != null ? match.second.connectedNode : -1;
        }

        public int[] GetConnections(NodeBlueprint.ConnectionClass connectionType)
        {
            // Combined into a single allocation-optimized LINQ query
            return connectionTypes
                .Where(n => connectionType.HasFlag(n.connectionClass))
                .Select(n => n.second.connectedNode)
                .ToArray();
        }

        public ConnectionType.VirtualPort GetPort(NodeBlueprint.ConnectionClass connectionType, int port = 0)
        {
            var match = connectionTypes.FirstOrDefault(n =>
                connectionType.HasFlag(n.connectionClass) && n.second.portIndex == port);

            // Safe check: If no match is found, return your Null port object instead of crashing
            return match != null ? match.first : ConnectionType.VirtualPort.Null;
        }
    }
}