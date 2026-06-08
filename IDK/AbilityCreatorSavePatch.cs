using EMPTY;
using IDK;
using Landfall.TABS;
using Landfall.TABS.UnitEditor;
using Steamworks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
       
        GameObject[] nodeScenes =  AbilityCreator.assetManager.GetAllAssets<GameObject>("move");
        for (int i = 0; i < extraSerializedUnit.m_combatMoves.Length; i++)
        {
            GameObject @object = System.Array.Find(nodeScenes, n => n.GetComponent<SpecialAbility>().Entity.GUID == extraSerializedUnit.m_combatMoves[i]);
            if (@object)
                unitBlueprint.objectsToSpawnAsChildren[i] = @object;
        }
    }
    public void Pre(ExtraSerializedUnit extraSerializedUnit)
    {
        List<LegacyNodeScene> allNodeScenes = AbilityCreator.nodeScenes;
        DeveloperLogger.Log($"Extra serialized unit detected! {extraSerializedUnit.m_name}");
        for (int i = 0; i < extraSerializedUnit.extraFieldNames.Count; i++)
        {
            DeveloperLogger.Log($"Field's name: {extraSerializedUnit.extraFieldNames[i]}");
            string encodedString = extraSerializedUnit.extraFieldValues[i];
            byte[] decodedBytes = System.Convert.FromBase64String(encodedString);
            string decodedText = System.Text.Encoding.UTF8.GetString(decodedBytes);
            var nodeScene = AbilityCreator.DeserializeAbility(decodedText);
            DeveloperLogger.Log("Deserialized ability!");
            if (AbilityCreator.IsAbilityWritten(nodeScene))
                continue;
            if (BundledAbilitesManager.bundledAbilities.Exists(n => n.abilityData == decodedText))
                continue;
            BundledAbilitesManager.BundledAbility bundledAbility = BundledAbilitesManager.BundleAbility(extraSerializedUnit, nodeScene, decodedText);
            DeveloperLogger.Log($"Bundled ability!");
            DeveloperLogger.Log($"Adding ability {bundledAbility}");
            BundledAbilitesManager.bundledAbilities.Add(bundledAbility);
            StartCoroutine(AbilityCreator.ForceShowModal(bundledAbility));
        }
    }
    public override ExtraSerializedUnit AddData(ExtraSerializedUnit serializedUnitBlueprint, UnitBlueprint unitBlueprint)
    {
        List<LegacyNodeScene> nodeScenes = new List<LegacyNodeScene>();
        for (int i = 0; i < unitBlueprint.objectsToSpawnAsChildren.Length; i++)
        {
            DeveloperLogger.Log("Checking if abiltiy is custom :" + unitBlueprint.objectsToSpawnAsChildren[i]);
            if (unitBlueprint.objectsToSpawnAsChildren[i].GetComponent<NodeRunner>())
            {
                DeveloperLogger.Log("It is custom!");
                NodeRunner nodeRunner = unitBlueprint.objectsToSpawnAsChildren[i].GetComponent<NodeRunner>();
                if (nodeScenes.Contains(nodeRunner.nodeScene))
                    continue;
                nodeScenes.Add(nodeRunner.nodeScene);
                DeveloperLogger.Log("Added field name:" + nodeRunner.nodeScene.sceneName);
                string txt = nodeRunner.nodeScene.Jsonify(Newtonsoft.Json.Formatting.Indented);
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(txt);
                string base64 = System.Convert.ToBase64String(bytes);
                serializedUnitBlueprint.AddData(nodeRunner.nodeScene.sceneName, base64);
                DeveloperLogger.Log("Added Json!");
            }
        }
        return serializedUnitBlueprint;
    }
}