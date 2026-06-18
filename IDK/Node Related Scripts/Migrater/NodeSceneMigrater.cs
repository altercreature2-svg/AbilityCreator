using AC.Node_Related_Scripts.connection_stuff;
using AC.Node_Related_Scripts.ConnectionStuff;
using AC.Node_Related_Scripts.Field_stuff;
using AC.Node_Related_Scripts.SavingStuff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AC.Node_Related_Scripts.Migrater
{
    public class NodeSceneMigrater
    {

        public static VirtualNodeScene GetNewSavedNodeScene(LegacySavedNodeScene legacySavedNodeScene)
        {
            VirtualNodeScene savedNodeScene = new VirtualNodeScene();
            savedNodeScene.abilityName = legacySavedNodeScene.sceneName;
            savedNodeScene.abilityDescription = legacySavedNodeScene.sceneDescription;
            savedNodeScene.abilityIcon = legacySavedNodeScene.sceneImage.ToString();
            savedNodeScene.abilityID = legacySavedNodeScene.id;
            List<ISaveable> saveables = new List<ISaveable>();
            for (int i = 0; legacySavedNodeScene.blueprints.Count > i; i++)
            {
                SaveableObject saveableObject = new SaveableObject();
                saveableObject.typeIdentfier = "%NODE%";
                List<SaveableField> saveableFields = new List<SaveableField>();
                saveableFields.Add(new SaveableField()
                {
                    fieldName = "%BLUEPRINT%",
                    fieldValue = legacySavedNodeScene.blueprints2[i],
                });
                Vector3 pos = legacySavedNodeScene.Positions2[i];
                saveableFields.Add(new SaveableField()
                {
                    fieldName = "%POSITION%",
                    fieldValue = pos.x + "/" + pos.y + "/" + pos.z,
                });

                NodeConnections nodeConnections = legacySavedNodeScene.connections2[i];
                for (int j = 0; j < nodeConnections.connectionsTypes.Count; j++)
                {
                    
                    int nodeID = legacySavedNodeScene.blueprints.FindIndex(n => n == legacySavedNodeScene.blueprints[i]);
                    int otherNodeID = legacySavedNodeScene.blueprints.FindIndex(n => n == nodeConnections.otherIDs[j]);
                    ConnectionType connectionType = ConnectionMigrater.MigrateToNew(nodeConnections.connectionsTypes[j],otherNodeID, nodeID);

                    saveableFields.Add(new SaveableField()
                    {
                        fieldName = "%CONNECTION%" + j,
                        fieldValue = Serialize.SaveJson(connectionType)
                    });
                }
                string[] fields = legacySavedNodeScene.fields2[i].Split(new char[] { '/' }, options:StringSplitOptions.RemoveEmptyEntries);
                for (int j = 0; j < fields.Length; j++)
                {
                    VirtualNodeField virtualNodeField = new VirtualNodeField(j, fields[j]);
                    saveableFields.Add(new SaveableField()
                    {
                        fieldName = "%FIELD%" + j,
                        fieldValue = Serialize.SaveJson(virtualNodeField)
                    });
                }


                saveableObject.fields = saveableFields.ToArray();
                global::AC.VirtualNode savedNode = new global::AC.VirtualNode();
                savedNode.Load(saveableObject);
                saveables.Add(savedNode);
            }
            savedNodeScene.savedObjects = saveables.ToArray();
            return savedNodeScene;
        }
    }
}
