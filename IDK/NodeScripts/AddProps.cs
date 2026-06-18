using AC.Help;
using AC.Node_Related_Scripts.NodeRunning;
using AC.Node_Related_Scripts.NodeRunning.Instructions.Courtines;
using DM;
using HarmonyLib;
using IDK.Help;
using Landfall.TABS;
using Landfall.TABS.UnitEditor;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AC.NodeScripts
{
    public class AddProps : IBehaviorNode
    {
        public IEnumerator<CoroutineReturn> Execute(NodeEnv env)
        {
            var unitEnum = env.GetValues(NodeBlueprint.ConnectionClass.ReciveUnit);
            var goEnum = env.GetValues(NodeBlueprint.ConnectionClass.ReciveGameObject);
            AssetLoader assetLoader = ContentDatabase.Instance().AssetLoader;
            bool isClothes = env.GetField(0) == "Clothes";
            if (isClothes)
            {
                foreach (var item in unitEnum)
                {
                    if (!(item.value is Unit unit))
                        continue;
                    UnitRig rig = env.cacheSystem.GetCachedComponent<UnitRig>(unit.gameObject);
                    FixedPool<GameObject> cleanProps = new FixedPool<GameObject>(goEnum.Length);
                    foreach (var item2 in goEnum)
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
            }
            else
            {
                foreach (var item in unitEnum)
                {
                    if (!(item.value is Unit unit))
                        continue;
                    foreach (var item2 in goEnum)
                    {
                        if (!(item2.value is GameObject ability))
                            continue;
                        GameObject abiltiy = Object.Instantiate(ability, unit.api.GetHipPosition(), Quaternion.identity, unit.transform);
                        FastTeamColor.SetTeamColors(abiltiy, unit.Team);
                        env.AddValue(NodeBlueprint.ConnectionClass.GiveGameObject, abiltiy);
                    }
                }
            }
            yield return new CoroutineReturn(CoroutineReturn.CourtineType.ContinueBranch);
        }
        public IEnumerator<CoroutineReturn> Cache(NodeEnv env)
        {
            return null;
        }
    }
}