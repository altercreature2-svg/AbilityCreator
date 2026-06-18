using DM;
using HarmonyLib;
using AC.Node_Related_Scripts.NodeRunning;
using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using AC.Help;
using AC.Node_Related_Scripts.NodeRunning.Instructions.Courtines;

namespace AC.NodeScripts
{
    public class AddCloth : IBehaviorNode
    {
        public IEnumerator<CoroutineReturn> Execute(NodeEnv env)
        {
            
            var units = env.GetValues(NodeBlueprint.ConnectionClass.ReciveUnit);
            var gameObjects = env.GetValues(NodeBlueprint.ConnectionClass.ReciveGameObject);
            AssetLoader assetLoader = ContentDatabase.Instance().AssetLoader;
            foreach (var item in units)
            {
                if (!(item.value is Unit unit))
                    continue;
                UnitRig rig = env.cacheSystem.GetCachedComponent<UnitRig>(unit.gameObject);
                FixedPool<GameObject> cleanProps = new FixedPool<GameObject>(gameObjects.Length);
                foreach (var item2 in gameObjects)
                {
                    if (!(item2.value is GameObject gameObject))
                        continue;
                    cleanProps.Insert(assetLoader.GetAsset<GameObject>(env.cacheSystem.GetCachedComponent<PropItem>(gameObject).Entity.GUID));
                }
                Stitcher.TransformCatalog catalog = new Stitcher.TransformCatalog(rig.gameObject, Stitcher.TransformCatalog.RigType.Human, "M_");
                for (int i = 0; i < cleanProps.Length; i++)
                {
                    if (cleanProps[i])
                    {
                        GameObject obj = rig.SpawnProp(cleanProps[i].GetComponent<CharacterItem>(), new PropItemData(), Stitcher.TransformCatalog.RigType.Human, unit.Team, catalog);
                        env.AddValue(NodeBlueprint.ConnectionClass.GiveGameObject, obj);
                    }
                }
            }
            yield return new CoroutineReturn(CoroutineReturn.CourtineType.ContinueBranch);

        }
        public IEnumerator<CoroutineReturn> Cache(NodeEnv env)
        {
            yield break;
        }
    }
}