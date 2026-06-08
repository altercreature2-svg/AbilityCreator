using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace IDK.AssetManaging.AssetTypes
{
    public class ExplosionAssetType : AssetType<GameObject>
    {

        public override Dictionary<string, GameObject> GetAllAssets()
        {
            Dictionary<string, GameObject> assets = new Dictionary<string, GameObject>();
            Explosion[] explosions = Resources.FindObjectsOfTypeAll<Explosion>();
            for (int i = 0; i < explosions.Length; i++)
            {
                string name = VanillaAssetManager.CleanName(explosions[i].gameObject.name);
                while (assets.ContainsKey(name))
                {
                    name += "+";
                }
                assets.Add(name, explosions[i].gameObject);
            }
            return assets;
        }
        public override string GetName()
        {
            return "explosion";
        }
    }
}
