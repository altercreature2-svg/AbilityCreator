using DM;
using HarmonyLib;
using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace IDK.NodeScripts
{
    public class AddCloth : IBehaviorNode
    {
        public override IEnumerator RunNode(LegacySavedNode savedNode, Unit unit, List<NodeComponent.LegacyConnection> connections, string[] fields, NodeRunner nodeRunner)
        {
            Unit[] units = connections.GetNode(NodeBlueprint.ConnectionClass.ReciveUnit).GetValuePoolSmart(unit).GetValues<Unit>();
            GameObject[] gameObjects = connections.GetNode(NodeBlueprint.ConnectionClass.ReciveGameObject).GetValuePoolSmart(unit).GetValues<GameObject>();
            AssetLoader assetLoader = ContentDatabase.Instance().AssetLoader;
            ValuePool valuePool = savedNode.GetValuePool(unit);
            valuePool.ClearValues();
            foreach (var unitIndex in units)
            {
                UnitRig rig = unitIndex.GetComponentInChildren<UnitRig>();
                CharacterItem[] characterItems = gameObjects.Where(n => n.GetComponent<PropItem>()).Select(n => n.GetComponent<CharacterItem>()).ToArray();
                GameObject[] cleanProps = characterItems.Select(n => assetLoader.GetAsset<GameObject>(n.Entity.GUID)).ToArray();
                cleanProps.Do(n => Debug.Log("Prop:" + n));
                Stitcher.TransformCatalog catalog = new Stitcher.TransformCatalog(rig.gameObject, Stitcher.TransformCatalog.RigType.Human, "M_");
                for (int i = 0; i < cleanProps.Length; i++)
                {
                    if (cleanProps[i])
                    {
                        GameObject obj = rig.SpawnProp(cleanProps[i].GetComponent<CharacterItem>(), new PropItemData(), Stitcher.TransformCatalog.RigType.Human, unit.Team, catalog);
                        valuePool.AddValue(obj);
                    }
                }

            }
            

            yield return savedNode.TriggerConnection(nodeRunner);

        }
    }
}