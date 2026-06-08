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
    public class UnitAssetType : AssetType<UnitBlueprint>
    {

        public override Dictionary<string, UnitBlueprint> GetAllAssets()
        {
            Dictionary<string, UnitBlueprint> assets = new Dictionary<string, UnitBlueprint>();
            UnitBlueprint[] unitBlueprints = Resources.FindObjectsOfTypeAll<UnitBlueprint>();
            var langauge = Localizer.GetLanguage(Localizer.Language.LANG_EN_US);

            for (int i = 0; i < unitBlueprints.Length; i++)
            {
                string name;
                if (langauge.ContainsKey(unitBlueprints[i].Entity.Name))
                    name = langauge[unitBlueprints[i].Entity.Name];
                else
                    name = unitBlueprints[i].Entity.Name;
                while (assets.ContainsKey(name))
                {
                    name += "+";
                }
                assets.Add(name, unitBlueprints[i]);

            }
            return assets;
        }
        public override string GetName()
        {
            return "unit";
        }
    }
}
