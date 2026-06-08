using DM;
using Landfall.TABS;
using MonoMod.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFBGames;
using UnityEngine;

namespace IDK.AssetManaging.AssetTypes
{
    public class SoundAssetType : AssetType<string>
    {

        public override Dictionary<string, string> GetAllAssets()
        {
            Dictionary<string, string> assets = new Dictionary<string, string>();
            SoundBankCategory[] catagories = ServiceLocator.GetService<SoundPlayer>().soundBank.Categories;
            for (int i = 0; i < ServiceLocator.GetService<SoundPlayer>().soundBank.Categories.Length; i++)
            {
                for (int o = 0; o < catagories[i].soundEffects.Length; o++)
                {
                    if (ServiceLocator.GetService<SoundPlayer>().soundBank.GetSoundEffect(catagories[i].categoryName + "/" + catagories[i].soundEffects[o].soundRef) != null)
                    {
                        string name = catagories[i].categoryName + "&" + catagories[i].soundEffects[o].soundRef;
                        while (assets.ContainsKey(name))
                        {
                            name += "+";
                        }
                        assets.Add(name, name);
                    }
                }
            }
            return assets;
        }
        public override string GetName()
        {
            return "sound";
        }
    }
}
