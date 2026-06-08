using IDK.AssetManaging.AssetTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDK.AssetManaging
{
    public class VanillaAssetManager
    {
        public static string CleanName(string name)
        {
            name = (name.Contains("_1 Prefabs_VB") ? name.Replace("_1 Prefabs_VB", "") : (name.Contains("_1 Weapons_VB") ? name.Replace("_1 Weapons_VB", "") : name));
            name = (name.Contains("_4 Moves_VB") ? name.Replace("_4 Moves_VB", "") : (name.Contains("_2 Projectiles_VB") ? name.Replace("_2 Projectiles_VB", "") : name));
            name = (name.Contains("_3 Effects_VB") ? name.Replace("_3 Effects_VB", "") : (name.Contains("_0 UnitBases_VB") ? name.Replace("_0 UnitBases_VB", "") : name));
            return name;
        }
        public Dictionary<AssetBaseType, Dictionary<string,object>> assets;
        private Dictionary<string, Dictionary<string, object>> cache = new Dictionary<string, Dictionary<string, object>>();

        public VanillaAssetManager()
        {
            List<AssetBaseType> assetTypes = new List<AssetBaseType>
            {
                new ExplosionAssetType(),
                new ParticleAssetType(),
                new ProjectileAssetType(),
                new SoundAssetType(),
                new SpriteAssetType(),
                new UnitAssetType(),
                new WeaponAssetType(),
                new AbilityAssetType(),
                new ClothingAssetType(),
                new ComponentAssetType(),
                new EffectAssetType(),
            };
            assets = new Dictionary<AssetBaseType, Dictionary<string, object>>();
            for (int i = 0; i < assetTypes.Count; i++)
            {
                Dictionary<string, object> data = assetTypes[i].UntypedData();
                assets.Add(assetTypes[i], data);
            }
            RefreshCache();
            for (int i = 0; i < assets.Count; i++)
            {
                string fileText = "";
                AssetBaseType type = assets.Keys.ElementAt(i);

                for (int o = 0; o < assets[type].Count; o++)
                {
                    KeyValuePair<string, object> pair = assets.Values.ElementAt(i).ElementAt(o);
                    fileText += ">" + pair.Key + "\n";
                }
                File.WriteAllText(Path.Combine(AbilityCreator.path, type.GetName() + ".txt"), fileText);
            }
        }
        public void RefreshCache()         
        {
            cache.Clear();
            for (int i = 0; i < assets.Count; i++)
            {
                AssetBaseType type = assets.Keys.ElementAt(i);
                Dictionary<string, object> dict = assets.First(x => x.Key.GetName() == type.GetName()).Value;
                cache.Add(type.GetName(), dict);
            }
        }
        public T GetAsset<T>(string assetType, string assetName)
        {
            return (T)assets.Where(x => x.Key.GetName() == assetType).FirstOrDefault().Value.Where(x => x.Key == assetName).FirstOrDefault().Value;
        }
        public T[] GetAllAssets<T>(string assetType)
        {
            return assets.Where(x => x.Key.GetName() == assetType).FirstOrDefault().Value.Select(n => (T)n.Value).ToArray();
        }
        public Dictionary<string,T> GetOld<T>(string assetType)
        {
            if (cache.ContainsKey(assetType))
            {
                return cache[assetType].ToDictionary(kvp => kvp.Key, kvp => (T)kvp.Value);
            }
            Dictionary<string, object> dict = assets.First(x => x.Key.GetName() == assetType).Value;

            var result = dict.ToDictionary(
                kvp => kvp.Key,
                kvp => (T)kvp.Value
            );
            return result;
        }
    }
}
