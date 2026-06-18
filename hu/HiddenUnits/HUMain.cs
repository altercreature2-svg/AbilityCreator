using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DM;
using HarmonyLib;
using HiddenUnits.Properties;
using Landfall.TABS;
using Landfall.TABS.GameMode;
using Landfall.TABS.UnitEditor;
using Landfall.TABS.Workshop;
using TGCore;
using UnityEngine;

namespace HiddenUnits
{
	// Token: 0x0200001D RID: 29
	public class HUMain
	{
		// Token: 0x060000AE RID: 174 RVA: 0x00008BDC File Offset: 0x00006DDC
		public HUMain()
		{
			AssetBundle.LoadFromMemory(HiddenUnits.Properties.Resources.egyptmap);
			AssetBundle.LoadFromMemory(HiddenUnits.Properties.Resources.egyptmap2);
			List<MapAsset> list = new List<MapAsset>();
			Dictionary<DatabaseID, int> dictionary = new Dictionary<DatabaseID, int>();
			FieldInfo field = typeof(LandfallContentDatabase).GetField("m_orderedMapAssets", BindingFlags.Instance | BindingFlags.NonPublic);
			List<MapAsset> list2 = ((MapAsset[])((field != null) ? field.GetValue(TGMain.landfallDb) : null)).ToList<MapAsset>();
			for (int i = 0; i < 29; i++)
			{
				list.Add(list2[i]);
			}
			list.Add(HUMain.huMaps.LoadAsset<MapAsset>("Egypt1"));
			list.Add(HUMain.huMaps.LoadAsset<MapAsset>("Egypt2"));
			list2.RemoveRange(0, 29);
			list.AddRange(list2);
			foreach (MapAsset mapAsset in HUMain.huMaps.LoadAllAssets<MapAsset>())
			{
				bool flag = !mapAsset.name.Contains("Egypt");
				if (flag)
				{
					list.Add(mapAsset);
				}
			}
			foreach (MapAsset mapAsset2 in list)
			{
				dictionary.Add(mapAsset2.Entity.GUID, list.IndexOf(mapAsset2));
			}
			FieldInfo field2 = typeof(LandfallContentDatabase).GetField("m_orderedMapAssets", BindingFlags.Instance | BindingFlags.NonPublic);
			if (field2 != null)
			{
				field2.SetValue(TGMain.landfallDb, list.ToArray());
			}
			FieldInfo field3 = typeof(LandfallContentDatabase).GetField("m_mapAssetIndexLookup", BindingFlags.Instance | BindingFlags.NonPublic);
			if (field3 != null)
			{
				field3.SetValue(TGMain.landfallDb, dictionary);
			}
			new Harmony("HiddenUnis").PatchAll();
			List<SecretUnlockCondition> list3 = new List<SecretUnlockCondition>(UnityEngine.Resources.FindObjectsOfTypeAll<SecretUnlockConditions>()[0].m_unlockConditions);
			list3.AddRange(HUMain.hiddenUnits.LoadAsset<SecretUnlockConditions>("HUUnlockConditions").m_unlockConditions);
			UnityEngine.Resources.FindObjectsOfTypeAll<SecretUnlockConditions>()[0].m_unlockConditions = list3.ToArray();
			foreach (Material material in HUMain.hiddenUnits.LoadAllAssets<Material>())
			{
				bool flag2 = Shader.Find(material.shader.name);
				if (flag2)
				{
					material.shader = Shader.Find(material.shader.name);
				}
			}
			using (IEnumerator<UnitBlueprint> enumerator2 = (from x in HUMain.hiddenUnits.LoadAllAssets<UnitBlueprint>()
			where x.UnitBase != null
			select x).GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					UnitBlueprint unit = enumerator2.Current;
					IEnumerable<GameObject> source = TGMain.landfallDb.GetUnitBases().ToList<GameObject>();
					Func<GameObject, bool> predicate;
					Func<GameObject, bool> <>9__6;
					if ((predicate = <>9__6) == null)
					{
						predicate = (<>9__6 = ((GameObject unitBase) => unitBase.name == unit.UnitBase.name));
					}
					foreach (GameObject unitBase2 in source.Where(predicate))
					{
						unit.UnitBase = unitBase2;
					}
					foreach (GameObject gameObject in TGMain.landfallDb.GetWeapons().ToList<GameObject>())
					{
						bool flag3 = unit.RightWeapon && gameObject.name == unit.RightWeapon.name;
						if (flag3)
						{
							unit.RightWeapon = gameObject;
						}
						bool flag4 = unit.LeftWeapon && gameObject.name == unit.LeftWeapon.name;
						if (flag4)
						{
							unit.LeftWeapon = gameObject;
						}
					}
				}
			}
			foreach (Faction faction in HUMain.hiddenUnits.LoadAllAssets<Faction>())
			{
				UnitBlueprint[] source2 = (from x in faction.Units
				where x
				orderby x.GetUnitCost(true)
				select x).ToArray<UnitBlueprint>();
				faction.Units = source2.ToArray<UnitBlueprint>();
				foreach (Faction faction2 in TGMain.landfallDb.GetFactions().ToList<Faction>())
				{
					bool flag5 = faction.Entity.Name == faction2.Entity.Name + "_NEW";
					if (flag5)
					{
						List<UnitBlueprint> list4 = new List<UnitBlueprint>(faction2.Units);
						list4.AddRange(faction.Units);
						faction2.Units = (from x in list4
						where x
						orderby x.GetUnitCost(true)
						select x).ToArray<UnitBlueprint>();
						Object.DestroyImmediate(faction);
					}
				}
			}
			TABSCampaignLevelAsset[] array4 = HUMain.hiddenUnits.LoadAllAssets<TABSCampaignLevelAsset>();
			for (int m = 0; m < array4.Length; m++)
			{
				TABSCampaignLevelAsset lvl = array4[m];
				Faction faction3 = HUMain.hiddenUnits.LoadAllAssets<Faction>().ToList<Faction>().Find((Faction x) => x.name.Contains("Egypt"));
				Faction faction4 = TGMain.landfallDb.GetFactions().ToList<Faction>().Find((Faction x) => x.name.Contains("Secret"));
				List<UnitBlueprint> list5 = new List<UnitBlueprint>();
				List<Faction> list6 = new List<Faction>();
				bool flag6 = lvl.name.Contains("EgyptLevel");
				if (flag6)
				{
					list6.AddRange(from x in TGMain.landfallDb.GetFactions().ToList<Faction>()
					where x.m_displayFaction
					select x);
					list6.Remove(faction4);
				}
				else
				{
					bool flag7 = lvl.name.Contains("EgyptMiscLevel");
					if (flag7)
					{
						list6.Add(faction3);
						list6.Add(faction4);
						list5.AddRange(faction3.Units);
						list5.Add(faction4.Units.ToList<UnitBlueprint>().Find((UnitBlueprint x) => x.name.Contains("BoomerangThrower")));
						list5.Add(faction4.Units.ToList<UnitBlueprint>().Find((UnitBlueprint x) => x.name.Contains("PotThrower")));
						list5.Add(faction4.Units.ToList<UnitBlueprint>().Find((UnitBlueprint x) => x.name.Contains("Sarcophagus")));
						list5.Add(faction4.Units.ToList<UnitBlueprint>().Find((UnitBlueprint x) => x.name.Contains("Selket")));
						list5.Add(faction4.Units.ToList<UnitBlueprint>().Find((UnitBlueprint x) => x.name.Contains("RaWarrior")));
					}
				}
				bool flag8 = lvl.name.Contains("MapEquals");
				if (flag8)
				{
					MapAsset mapAsset3 = TGMain.landfallDb.GetMapAssetsOrdered().ToList<MapAsset>().Find((MapAsset x) => x.name.Contains(lvl.name.Split(new string[]
					{
						"MapEquals_"
					}, StringSplitOptions.RemoveEmptyEntries).Last<string>()));
					bool flag9 = mapAsset3;
					if (flag9)
					{
						lvl.MapAsset = mapAsset3;
					}
				}
				List<TABSCampaignLevelAsset.TABSLayoutUnit> list7 = new List<TABSCampaignLevelAsset.TABSLayoutUnit>();
				list7.AddRange(lvl.BlueUnits);
				list7.AddRange(lvl.RedUnits);
				using (List<TABSCampaignLevelAsset.TABSLayoutUnit>.Enumerator enumerator6 = list7.GetEnumerator())
				{
					while (enumerator6.MoveNext())
					{
						TABSCampaignLevelAsset.TABSLayoutUnit unit = enumerator6.Current;
						bool flag10 = unit.m_unitBlueprint && unit.m_unitBlueprint.name.Contains("_VANILLA");
						if (flag10)
						{
							UnitBlueprint unitBlueprint = TGMain.landfallDb.GetUnitBlueprints().ToList<UnitBlueprint>().Find((UnitBlueprint x) => x.name == unit.m_unitBlueprint.name.Replace("_VANILLA", ""));
							bool flag11 = unitBlueprint;
							if (flag11)
							{
								unit.m_unitBlueprint = unitBlueprint;
							}
						}
					}
				}
				lvl.AllowedFactions = list6.ToArray();
				lvl.AllowedUnits = list5.ToArray();
			}
			foreach (PropItem propItem in from x in HUMain.hiddenUnits.LoadAllAssets<GameObject>()
			where x.GetComponent<PropItem>()
			select x.GetComponent<PropItem>())
			{
				bool flag12 = !propItem;
				if (!flag12)
				{
					int num = propItem.GetComponentsInChildren<MeshFilter>().Where((MeshFilter rend) => rend && rend.gameObject.activeSelf && rend.gameObject.activeInHierarchy && rend.mesh.subMeshCount > 0 && rend.GetComponent<MeshRenderer>() && rend.GetComponent<MeshRenderer>().enabled).Sum((MeshFilter rend) => rend.mesh.subMeshCount) + propItem.GetComponentsInChildren<SkinnedMeshRenderer>().Where((SkinnedMeshRenderer rend) => rend && rend.gameObject.activeSelf && rend.sharedMesh.subMeshCount > 0 && rend.enabled).Sum((SkinnedMeshRenderer rend) => rend.sharedMesh.subMeshCount);
					bool flag13 = num > 0;
					if (flag13)
					{
						float item = 1f / (float)num;
						List<float> list8 = new List<float>();
						for (int n = 0; n < num - 1; n++)
						{
							list8.Add(item);
						}
						propItem.SubmeshArea = list8.ToArray();
					}
				}
			}
			foreach (WeaponItem weaponItem in from x in HUMain.hiddenUnits.LoadAllAssets<GameObject>()
			where x.GetComponent<WeaponItem>()
			select x.GetComponent<WeaponItem>())
			{
				bool flag14 = !weaponItem;
				if (!flag14)
				{
					int num2 = weaponItem.GetComponentsInChildren<MeshFilter>().Where((MeshFilter rend) => rend && rend.gameObject.activeSelf && rend.gameObject.activeInHierarchy && rend.mesh.subMeshCount > 0 && rend.GetComponent<MeshRenderer>() && rend.GetComponent<MeshRenderer>().enabled).Sum((MeshFilter rend) => rend.mesh.subMeshCount) + weaponItem.GetComponentsInChildren<SkinnedMeshRenderer>().Where((SkinnedMeshRenderer rend) => rend && rend.gameObject.activeSelf && rend.sharedMesh.subMeshCount > 0 && rend.enabled).Sum((SkinnedMeshRenderer rend) => rend.sharedMesh.subMeshCount);
					bool flag15 = num2 > 0;
					if (flag15)
					{
						float item2 = 1f / (float)num2;
						List<float> list9 = new List<float>();
						for (int num3 = 0; num3 < num2 - 1; num3++)
						{
							list9.Add(item2);
						}
						weaponItem.SubmeshArea = list9.ToArray();
					}
				}
			}
			foreach (AudioSource audioSource in HUMain.hiddenUnits.LoadAllAssets<AudioSource>())
			{
				audioSource.outputAudioMixerGroup = ServiceLocator.GetService<GameModeService>().AudioSettings.AudioMixer.outputAudioMixerGroup;
			}
			TGAddons.AddItems(HUMain.hiddenUnits.LoadAllAssets<UnitBlueprint>(), HUMain.hiddenUnits.LoadAllAssets<Faction>(), HUMain.hiddenUnits.LoadAllAssets<TABSCampaignAsset>(), HUMain.hiddenUnits.LoadAllAssets<TABSCampaignLevelAsset>(), HUMain.hiddenUnits.LoadAllAssets<VoiceBundle>(), HUMain.hiddenUnits.LoadAllAssets<FactionIcon>(), from x in HUMain.hiddenUnits.LoadAllAssets<GameObject>()
			select x.GetComponent<Unit>(), HUMain.hiddenUnits.LoadAllAssets<GameObject>().Select((GameObject x) => x.GetComponent<PropItem>()), HUMain.hiddenUnits.LoadAllAssets<GameObject>().Select((GameObject x) => x.GetComponent<SpecialAbility>()), HUMain.hiddenUnits.LoadAllAssets<GameObject>().Select((GameObject x) => x.GetComponent<WeaponItem>()), HUMain.hiddenUnits.LoadAllAssets<GameObject>().Select((GameObject x) => x.GetComponent<ProjectileEntity>()));
			TGMain.newSounds.AddRange(HUMain.hiddenUnits.LoadAllAssets<SoundBank>());
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x060000AF RID: 175 RVA: 0x00009ABC File Offset: 0x00007CBC
		public static bool InfiniteScalingEnabled
		{
			get
			{
				return HULauncher.ConfigInfiniteScalingEnabled.Value;
			}
		}

		// Token: 0x04000122 RID: 290
		public static AssetBundle hiddenUnits = AssetBundle.LoadFromMemory(HiddenUnits.Properties.Resources.hiddenunits);

		// Token: 0x04000123 RID: 291
		public static AssetBundle huMaps = AssetBundle.LoadFromMemory(HiddenUnits.Properties.Resources.humaps);
	}
}
