using DM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AC.AssetManaging.AssetTypes
{
    public class ParticleAssetType : AssetType<GameObject>
    {

        public override Dictionary<string, GameObject> GetAllAssets()
        {
            Dictionary<string, GameObject> assets = new Dictionary<string, GameObject>();
            ParticleSystem[] particleSystems = Resources.FindObjectsOfTypeAll<ParticleSystem>();
            for (int i = 0; i < particleSystems.Length; i++)
            {
                string name = particleSystems[i].transform.root.name + "+" + particleSystems[i].gameObject.name;
                while (assets.ContainsKey(name))
                {
                    name += "+";
                }
                assets.Add(name, particleSystems[i].gameObject);
            }
            return assets;
        }
        public override string GetName()
        {
            return "particle";
        }
    }
}
