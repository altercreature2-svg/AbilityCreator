using DM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AC.AssetManaging.AssetTypes
{
    public class ClothingAssetType : AssetType<GameObject>
    {

        public override Dictionary<string, GameObject> GetAllAssets()
        {
            Dictionary<string, GameObject> assets = new Dictionary<string, GameObject>();
            GameObject[] clothes = ContentDatabase.Instance().LandfallContentDatabase.GetCharacterProps().ToArray();
            for (int i = 0; i < clothes.Length; i++)
            {
                string name = VanillaAssetManager.CleanName(clothes[i].gameObject.name);
                while (assets.ContainsKey(name))
                {
                    name += "+";
                }
                assets.Add(name, clothes[i].gameObject);
            }
            return assets;
        }
        public override string GetName()
        {
            return "prop";
        }
    }
}
