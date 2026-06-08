using DM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace IDK.AssetManaging.AssetTypes
{
    public class EffectAssetType : AssetType<GameObject>
    {
       

        public override Dictionary<string, GameObject> GetAllAssets()
        {
            Dictionary<string, GameObject> assets = new Dictionary<string, GameObject>();
            UnitEffectBase[] effect = Resources.FindObjectsOfTypeAll<UnitEffectBase>();
            for (int i = 0; i < effect.Length; i++)
            {
                string name = VanillaAssetManager.CleanName(effect[i].gameObject.name);
                while (assets.ContainsKey(name))
                {
                    name += "+";
                }
                assets.Add(name, effect[i].gameObject);
            }
            return assets;
        }
        public override string GetName()
        {
            return "effect";
        }
    }
}
