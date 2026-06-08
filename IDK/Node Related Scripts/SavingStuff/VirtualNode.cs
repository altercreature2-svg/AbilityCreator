using HarmonyLib;
using IDK.Node_Related_Scripts.connection_stuff;
using IDK.Node_Related_Scripts.ConnectionStuff;
using IDK.Node_Related_Scripts.Field_stuff;
using IDK.Node_Related_Scripts.SavingStuff;
using IDK.NodeScripts;
using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

namespace IDK
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
                    fieldName = "%CONNECTION%" + connectionTypes[i].portName,
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
        public object Load(SaveableObject saveableObject)
        {
            VirtualNode savedNode = new VirtualNode();
            savedNode.nodeBlueprint = saveableObject.GetSavedField("%BLUEPRINT%");
            string[] positions = saveableObject.GetSavedField("%POSITION%").Split(new string[] { "/"},options: StringSplitOptions.RemoveEmptyEntries);

            savedNode.editorPositon = new Vector3(positions[0].QuickParse(), positions[1].QuickParse(), positions[2].QuickParse());
            List<ConnectionType> connectionTypes = new List<ConnectionType>();
            string[] connectionJsons = saveableObject.GetSavedFields("%CONNECTION%");
            for (int i = 0; connectionJsons.Length > i;i++)
            {
                connectionTypes.Add(Serialize.LoadJson<ConnectionType>(connectionJsons[i]));
            }
            savedNode.connectionTypes = connectionTypes;

            List<VirtualNodeField> virtualNodeFields = new List<VirtualNodeField>();
            string[] fieldJsons = saveableObject.GetSavedFields("%FIELD%");
            for(int i = 0; fieldJsons.Length > i; i++)
            {
                virtualNodeFields.Add(Serialize.LoadJson<VirtualNodeField>(fieldJsons[i]));
            }
            savedNode.fields = virtualNodeFields;
            return savedNode;
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
            return connectionTypes.Select(n => n.connectedNode).ToArray();
        }
        public int GetConnection(NodeBlueprint.ConnectionClass connectionType, string port = "0")
        {
            return connectionTypes.FirstOrDefault(n => n.connectionType == connectionType && n.portName == port).connectedNode;
        }
        public int[] GetConnections(NodeBlueprint.ConnectionClass connectionType)
        {
            return connectionTypes.FindAll(n => n.connectionType == connectionType).Select(n => n.connectedNode).ToArray();
        }
    }
}