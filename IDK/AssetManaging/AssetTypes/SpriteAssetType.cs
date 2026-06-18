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

namespace AC.AssetManaging.AssetTypes
{
    public class SpriteAssetType : AssetType<Sprite>
    {

        public override Dictionary<string, Sprite> GetAllAssets()
        {
            Dictionary<string, Sprite> assets = new Dictionary<string, Sprite>();
            List<Sprite> allSprites = new List<Sprite>();
            allSprites.AddRange(ContentDatabase.Instance().GetFactionIcons().Select(n => n.Entity.LargeSpriteIcon).Where(n => n));
            allSprites.AddRange(ContentDatabase.Instance().GetAllCombatMoves().Select(n => n.GetComponent<CharacterItem>().Entity.LargeSpriteIcon).Where(n => n));
            allSprites.AddRange(ContentDatabase.Instance().GetAllUnitBlueprints().Where(n => n.IsCustomUnit).Select(n => n.Entity.LargeSpriteIcon).Where(n => n));
            for (int i = 0; i < allSprites.Count; i++)
            {
                string name = allSprites[i].name;
                while (assets.ContainsKey(name))
                {
                    name += "+";
                }
                assets.Add(name, allSprites[i]);
            }
            return assets;
        }
        public override string GetName()
        {
            return "sprite";
        }
    }
}
