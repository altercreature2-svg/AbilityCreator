using DM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace IDK.AssetManaging.AssetTypes
{
    public class AbilityAssetType : AssetType<GameObject>
    {

        public override Dictionary<string, GameObject> GetAllAssets()
        {
            Dictionary<string, GameObject> assets = new Dictionary<string, GameObject>();
            GameObject[] abilties = ContentDatabase.Instance().LandfallContentDatabase.GetCombatMoves().ToArray();
            for (int i = 0; i < abilties.Length; i++)
            {
                string name = VanillaAssetManager.CleanName(abilties[i].gameObject.name);
                while (assets.ContainsKey(name))
                {
                    name += "+";
                }
                assets.Add(name, abilties[i].gameObject);
            }
            return assets;
        }
        public override string GetName()
        {
            return "move";
        }
    }
}
