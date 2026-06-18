using DM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AC.AssetManaging.AssetTypes
{
    public class WeaponAssetType : AssetType<GameObject>
    {

        public override Dictionary<string, GameObject> GetAllAssets()
        {
            Dictionary<string, GameObject> assets = new Dictionary<string, GameObject>();
            GameObject[] weapons = ContentDatabase.Instance().LandfallContentDatabase.GetWeapons().ToArray();
            for (int i = 0; i < weapons.Length; i++)
            {
                string name = VanillaAssetManager.CleanName(weapons[i].gameObject.name);
                while (assets.ContainsKey(name))
                {
                    name += "+";
                }
                assets.Add(name, weapons[i].gameObject);
            }
            return assets;
        }
        public override string GetName()
        {
            return "weapon";
        }
    }
}
