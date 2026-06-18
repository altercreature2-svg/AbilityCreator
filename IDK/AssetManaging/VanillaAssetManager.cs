using AC.AssetManaging.AssetTypes;
using IDK.Node_Related_Scripts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AC.AssetManaging
{
    public class VanillaAssetManager
    {
        // Optimized with String.Replace for better readability and performance
        private static readonly string[] SuffixedToRemove = {
            "_1 Prefabs_VB", "_1 Weapons_VB", "_4 Moves_VB",
            "_2 Projectiles_VB", "_3 Effects_VB", "_0 UnitBases_VB"
        };

        public static string CleanName(string name)
        {
            if (string.IsNullOrEmpty(name)) return name;

            foreach (var suffix in SuffixedToRemove)
            {
                if (name.Contains(suffix))
                {
                    return name.Replace(suffix, "");
                }
            }
            return name;
        }

        // Changed outer key to 'string' (AssetType Name) for O(1) direct lookups
        public Dictionary<string, Dictionary<string, object>> assets = new Dictionary<string, Dictionary<string, object>>();

        public VanillaAssetManager()
        {
            var assetTypes = new List<AssetBaseType>
            {
                new ExplosionAssetType(), new ParticleAssetType(), new ProjectileAssetType(),
                new SoundAssetType(), new SpriteAssetType(), new UnitAssetType(),
                new WeaponAssetType(), new AbilityAssetType(), new ClothingAssetType(),
                new ComponentAssetType(), new EffectAssetType(),
            };

            // 1. Populate the dictionary efficiently
            foreach (var type in assetTypes)
            {
                assets.Add(type.GetName(), type.UntypedData());
            }

            // 2. Write files using StringBuilder (avoids massive GC allocations)
            StringBuilder sb = new StringBuilder();
            foreach (var typePair in assets)
            {
                sb.Clear();
                foreach (var assetPair in typePair.Value)
                {
                    sb.Append('>').Append(assetPair.Key).AppendLine();
                }

                string targetPath = Path.Combine(FilePaths.AbilityCreatorPath, $"{typePair.Key}.txt");
                File.WriteAllText(targetPath, sb.ToString());
            }
        }

        // Direct O(1) dictionary lookup. No loops, instantly fast.
        public T GetAsset<T>(string assetType, string assetName)
        {
            if (assets.TryGetValue(assetType, out var typeDict))
            {
                if (typeDict.TryGetValue(assetName, out var asset))
                {
                    return (T)asset;
                }
            }
            return default;
        }

        // Drastically faster array conversion using casting copies
        public T[] GetAllAssets<T>(string assetType)
        {
            if (assets.TryGetValue(assetType, out var typeDict))
            {
                return typeDict.Values.Cast<T>().ToArray();
            }
            return Array.Empty<T>();
        }

        // Cleaned up type casting lookup
        public Dictionary<string, T> GetOld<T>(string assetType)
        {
            if (assets.TryGetValue(assetType, out var typeDict))
            {
                return typeDict.ToDictionary(kvp => kvp.Key, kvp => (T)kvp.Value);
            }
            return new Dictionary<string, T>();
        }
    }
}