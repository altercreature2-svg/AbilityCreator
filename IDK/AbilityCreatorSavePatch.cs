using EMPTY;
using AC;
using Landfall.TABS;
using Landfall.TABS.UnitEditor;
using Steamworks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using IDK.AbilityHandling;
using AC.Node_Related_Scripts.SavingStuff;

public class AbilityCreatorSavePatch : ISaveUnit
{
    public void Awake()
    {
        EMPTY.UnitLoaderHarmonyPatch.preUnitLoadedEvent += Pre;
        EMPTY.UnitLoaderHarmonyPatch.postUnitLoadedEvent += Post;
    }
    public void Post(UnitBlueprint unitBlueprint, ExtraSerializedUnit extraSerializedUnit)
    {
        if (!AbilityCreator.units.ContainsKey(unitBlueprint.Name))
            AbilityCreator.units.Add(unitBlueprint.Name, null);
        AbilityCreator.units[unitBlueprint.Name] = unitBlueprint;
       
        GameObject[] nodeScenes = AbilityCreator.assetManager.GetAllAssets<GameObject>("move");
        for (int i = 0; i < extraSerializedUnit.m_combatMoves.Length; i++)
        {
            GameObject @object = System.Array.Find(nodeScenes, n => n.GetComponent<SpecialAbility>().Entity.GUID == extraSerializedUnit.m_combatMoves[i]);
            if (@object)
                unitBlueprint.objectsToSpawnAsChildren[i] = @object;
        }
    }
    public void Pre(ExtraSerializedUnit extraSerializedUnit)
    {;
        DeveloperLogger.Log($"Extra serialized unit detected! {extraSerializedUnit.m_name}");
        for (int i = 0; i < extraSerializedUnit.extraFieldNames.Count; i++)
        {
            DeveloperLogger.Log($"Field's name: {extraSerializedUnit.extraFieldNames[i]}");
            string encodedString = extraSerializedUnit.extraFieldValues[i];
            byte[] decodedBytes = System.Convert.FromBase64String(encodedString);
            string decodedText = System.Text.Encoding.UTF8.GetString(decodedBytes);
            var nodeScene = Serialize.LoadJson<VirtualNodeScene>(decodedText);
            DeveloperLogger.Log("Deserialized ability!");
            BundledAbilitesManager.BundledAbility bundledAbility = BundledAbilitesManager.BundleAbility(extraSerializedUnit, nodeScene, decodedText);
            DeveloperLogger.Log($"Bundled ability!");
            DeveloperLogger.Log($"Adding ability {bundledAbility}");
            BundledAbilitesManager.bundledAbilities.Add(bundledAbility);
            StreamedSceneManager.AddBundledAbility(bundledAbility);
        }
    }
    public override ExtraSerializedUnit AddData(ExtraSerializedUnit serializedUnitBlueprint, UnitBlueprint unitBlueprint)
    {
        List<VirtualNodeScene> nodeScenes = new List<VirtualNodeScene>();
        for (int i = 0; i < unitBlueprint.objectsToSpawnAsChildren.Length; i++)
        {
            DeveloperLogger.Log("Checking if abiltiy is custom :" + unitBlueprint.objectsToSpawnAsChildren[i]);
            if (unitBlueprint.objectsToSpawnAsChildren[i].GetComponent<SpawnAbility>())
            {
                DeveloperLogger.Log("It is custom!");
                NodeRunner nodeRunner = unitBlueprint.objectsToSpawnAsChildren[i].GetComponent<SpawnAbility>().ObjectToSpawn.GetComponent<NodeRunner>();
                if (nodeScenes.Contains(nodeRunner.NodeScene))
                    continue;
                nodeScenes.Add(nodeRunner.NodeScene);
                DeveloperLogger.Log("Added field name:" + nodeRunner.NodeScene.abilityName);
                string txt = Serialize.SaveJson<VirtualNodeScene>(nodeRunner.NodeScene);
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(txt);
                string base64 = System.Convert.ToBase64String(bytes);
                serializedUnitBlueprint.AddData(nodeRunner.NodeScene.abilityName, base64);
                DeveloperLogger.Log("Added Json!");
            }
        }
        return serializedUnitBlueprint;
    }
}