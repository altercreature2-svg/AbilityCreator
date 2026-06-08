using DM;
using HarmonyLib;
using Landfall.TABS;
using Landfall.TABS.UnitEditor;
using Sirenix.Serialization;
using Sirenix.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using UnityEngine;
using static Stitcher.TransformCatalog;

namespace IDK.NodeScripts
{
    public class AddProps : IBehaviorNode
    {
        public override IEnumerator RunNode(LegacySavedNode savedNode, Unit unit, List<NodeComponent.LegacyConnection> connections, string[] fields, NodeRunner nodeRunner)
        {
            Unit[] units = connections.GetNode(NodeBlueprint.ConnectionClass.ReciveUnit).GetValuePoolSmart(unit).GetValues<Unit>();
            GameObject[] gameObjects = connections.GetNode(NodeBlueprint.ConnectionClass.ReciveGameObject).GetValuePoolSmart(unit).GetValues<GameObject>();
            AssetLoader assetLoader = ContentDatabase.Instance().AssetLoader;
            foreach (var unitIndex in units)
            {
                if (fields[0] == "Clothes")
                {
                    UnitRig rig = unitIndex.GetComponentInChildren<UnitRig>();
                    CharacterItem[] characterItems = gameObjects.Where(n => n.GetComponent<PropItem>()).Select(n => n.GetComponent<CharacterItem>()).ToArray();
                    GameObject[] cleanProps = characterItems.Select(n => assetLoader.GetAsset<GameObject>(n.Entity.GUID)).ToArray();
                    cleanProps.Do(n => Debug.Log("Prop:" + n));
                    rig.SpawnProps(cleanProps, characterItems.Select(n => n.PropData).ToArray(), unitIndex.RigType, unitIndex.Team);
                }

                else
                {
                    SpecialAbility[] specialAbilities = gameObjects.Where(n => n.GetComponent<SpecialAbility>()).Select(n => n.GetComponent<SpecialAbility>()).ToArray();
                    GameObject[] cleanAbilites = specialAbilities.Select(n => assetLoader.GetAsset<GameObject>(n.Entity.GUID)).Where(n=>n).ToArray();
                    cleanAbilites.Do(n => AddAbility(n));
                    void AddAbility(GameObject gameObject)
                    {
                        Debug.Log("Adding ability");
                        GameObject ability = Object.Instantiate(gameObject, unitIndex.transform);
                        unitIndex.CallMethod("SetTeamColors", new object[] { ability, unitIndex.Team });
                    }
                }
            }
            
            yield return savedNode.TriggerConnection(nodeRunner);

        }
    }
}