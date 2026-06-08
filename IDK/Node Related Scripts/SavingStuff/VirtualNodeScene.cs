using HarmonyLib;
using IDK.Node_Related_Scripts.connection_stuff;
using IDK.Node_Related_Scripts.Migrater;
using IDK.Node_Related_Scripts.SavingStuff;
using Landfall.TABS;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace IDK
{
    [System.Serializable]
    public class VirtualNodeScene : IRegisterable, ISaveable
    {
        public string abilityName;
        public string abilityDescription;
        public string abilityIcon;
        public int abilityID;
        public ISaveable[] savedObjects;
        public SaveableObject Save()
        {
            SaveableObject saveableObject = new SaveableObject();
            saveableObject.typeIdentfier = "%NODESCENE%";
            List<SaveableField> fields = new List<SaveableField>();
            fields.Add(new SaveableField()
            {
                fieldName = "%NAME%",
                fieldValue = abilityName,
            });
            fields.Add(new SaveableField()
            {
                fieldName = "%DESCRIPTION%",
                fieldValue = abilityDescription,
            });
            fields.Add(new SaveableField()
            {
                fieldName = "%ICON%",
                fieldValue = abilityIcon,
            });
            fields.Add(new SaveableField()
            {
                fieldName = "%ID%",
                fieldValue = abilityID.ToString(),
            });
            for (int i = 0; i < savedObjects.Length; i++)
            {
                fields.Add(new SaveableField()
                {
                    fieldName = "%OBJECT%" + i,
                    fieldValue = Serialize.SaveJson(savedObjects[i].Save()),
                });
            }
            
            saveableObject.fields = fields.ToArray();
            return saveableObject;
        }
        public object Load(SaveableObject saveableObject)
        {
            VirtualNodeScene savedNodeScene = new VirtualNodeScene();
            savedNodeScene.abilityName = saveableObject.GetSavedField("%NAME%");
            savedNodeScene.abilityDescription = saveableObject.GetSavedField("%DESCRIPTION%");
            savedNodeScene.abilityIcon = saveableObject.GetSavedField("%ICON%");
            int id = int.Parse(saveableObject.GetSavedField("%ID%"));
            savedNodeScene.abilityID = id;
            List<ISaveable> saveableObjects = new List<ISaveable>();
            string[] objectJsons = saveableObject.GetSavedFields("%OBJECT%");
            for (int i = 0; i < objectJsons.Length; i++)
            {
                SaveableObject obj = Serialize.LoadJson<SaveableObject>(objectJsons[i]);
                if (obj.typeIdentfier == "%NODE%")
                {
                    VirtualNode savedNode = new VirtualNode();
                    savedNode.Load(saveableObject);
                    saveableObjects.Add(savedNode);
                }
            }
            savedNodeScene.savedObjects = saveableObjects.ToArray();
            return savedNodeScene;
        }
    }
}