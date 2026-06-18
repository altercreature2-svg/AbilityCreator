using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx;
using BepInEx.Configuration;
using Landfall.TABS;
using Pathfinding;
using TGCore;
using TGCore.Localization;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HiddenUnits
{
	// Token: 0x0200001C RID: 28
	[BepInPlugin("teamgrad.hiddenunits", "Hidden Units", "1.2.2")]
	[BepInDependency("teamgrad.core", BepInDependency.DependencyFlags.HardDependency)]
	public class HULauncher : TGMod
	{
		// Token: 0x060000A9 RID: 169 RVA: 0x00007CE3 File Offset: 0x00005EE3
		public override void Launch()
		{
			new HUMain();
		}

		// Token: 0x060000AA RID: 170 RVA: 0x00007CEC File Offset: 0x00005EEC
		public override void AddSettings()
		{
			HULauncher.ConfigInfiniteScalingEnabled = base.Config.Bind<bool>("Bug", "InfiniteScalingEnabled", true, "Enables/disables Mathematician/Philosopher projectiles infinitely scaling unit parts.");
			SettingsInstance settingsInstance = TGAddons.CreateSetting(SettingsInstance.SettingsType.Options, "Toggle infinite projectile scaling", "Enables/disables Mathematician/Philosopher projectiles infinitely scaling unit parts.", "BUG", 0f, (float)(HULauncher.ConfigInfiniteScalingEnabled.Value ? 0 : 1), new string[]
			{
				"Disabled",
				"Enabled"
			}, 0f, 1f);
			settingsInstance.OnValueChanged += delegate(int value)
			{
				HULauncher.ConfigInfiniteScalingEnabled.Value = (value == 1);
			};
		}

		// Token: 0x060000AB RID: 171 RVA: 0x00007D8C File Offset: 0x00005F8C
		public override void SceneManager(Scene scene, LoadSceneMode laodSceneMode)
		{
			bool flag = scene.path == "Assets/11 Scenes/MainMenu.unity";
			if (flag)
			{
				bool flag2 = !ServiceLocator.GetService<ISaveLoaderService>().HasUnlockedSecret("SECRET_EGYPT");
				if (flag2)
				{
					ServiceLocator.GetService<ISaveLoaderService>().UnlockSecret("SECRET_EGYPT");
					ServiceLocator.GetService<ModalPanel>().OpenUnlockPanel("You unlocked the Egypt faction!", HUMain.hiddenUnits.LoadAsset<Sprite>("egypt"));
				}
			}
			else
			{
				bool flag3 = scene.name.Contains("SG_");
				if (flag3)
				{
					bool flag4 = scene.name == "SG_Egypt" && ServiceLocator.GetService<ISaveLoaderService>().HasUnlockedSecret("BILLY_SWORD");
					if (flag4)
					{
						Object.Instantiate<GameObject>(HUMain.hiddenUnits.LoadAsset<GameObject>("BillyKey_Unlock4"), null, true);
					}
					GameObject gameObject = null;
					GameObject gameObject2 = null;
					foreach (GameObject gameObject3 in scene.GetRootGameObjects())
					{
						bool flag5 = gameObject3.name == "AStar_Lvl1_Grid";
						if (flag5)
						{
							gameObject = gameObject3;
						}
						bool flag6 = gameObject3.name == "Map";
						if (flag6)
						{
							gameObject2 = gameObject3;
							List<MeshRenderer> list = new List<MeshRenderer>(gameObject3.GetComponentsInChildren<MeshRenderer>(true).ToList<MeshRenderer>().FindAll((MeshRenderer x) => x.name.Contains("_ReplaceMe")));
							foreach (MeshRenderer meshRenderer in list)
							{
								meshRenderer.material.shader = (Shader.Find(meshRenderer.material.shader.name) ?? meshRenderer.material.shader);
								bool flag7 = meshRenderer.GetComponent<PiratePlacementTransparency>();
								if (flag7)
								{
									meshRenderer.GetComponent<PiratePlacementTransparency>().Materials[0].m_oldMaterial.shader = (Shader.Find(meshRenderer.GetComponent<PiratePlacementTransparency>().Materials[0].m_oldMaterial.shader.name) ?? meshRenderer.GetComponent<PiratePlacementTransparency>().Materials[0].m_oldMaterial.shader);
								}
							}
						}
						bool flag8 = gameObject3.name.Contains("_ReplaceMe");
						if (flag8)
						{
							gameObject3.GetComponent<MeshRenderer>().material.shader = (Shader.Find(gameObject3.GetComponent<MeshRenderer>().material.shader.name) ?? Shader.Find(gameObject3.GetComponent<MeshRenderer>().material.shader.name));
						}
						bool flag9 = gameObject3.name == "WaterManager";
						if (flag9)
						{
							gameObject3.GetComponent<PirateWaterManager>().WaterMaterial = gameObject3.GetComponent<MeshRenderer>().material;
						}
					}
					bool flag10 = gameObject != null && gameObject2 != null;
					if (flag10)
					{
						AstarPath componentInChildren = gameObject.GetComponentInChildren<AstarPath>(true);
						gameObject.SetActive(true);
						bool flag11 = componentInChildren.data.graphs.Length != 0;
						if (flag11)
						{
							componentInChildren.data.RemoveGraph(componentInChildren.data.graphs[0]);
						}
						componentInChildren.data.AddGraph(typeof(RecastGraph));
						componentInChildren.data.recastGraph.minRegionSize = 0.1f;
						componentInChildren.data.recastGraph.characterRadius = 0.3f;
						componentInChildren.data.recastGraph.cellSize = 0.2f;
						componentInChildren.data.recastGraph.forcedBoundsSize = new Vector3(gameObject2.GetComponent<MapSettings>().m_mapRadius * 2f, gameObject2.GetComponent<MapSettings>().m_mapRadius * gameObject2.GetComponent<MapSettings>().mapRadiusYMultiplier * 2f, gameObject2.GetComponent<MapSettings>().m_mapRadius * 2f);
						componentInChildren.data.recastGraph.rasterizeMeshes = false;
						componentInChildren.data.recastGraph.rasterizeColliders = true;
						componentInChildren.data.recastGraph.mask = HUMain.hiddenUnits.LoadAsset<GameObject>("AStarDummy").GetComponent<Explosion>().layerMask;
						componentInChildren.Scan(null);
					}
				}
				else
				{
					bool flag12 = scene.name == "00_Simulation_Day_VC";
					if (flag12)
					{
						GameObject gameObject4 = new GameObject
						{
							name = "Secrets"
						};
						Object.Instantiate<GameObject>(HUMain.hiddenUnits.LoadAsset<GameObject>("Saitama_Unlock"), gameObject4.transform, true);
					}
					else
					{
						bool flag13 = scene.name == "00_Lvl2_Halloween_VC";
						if (flag13)
						{
							GameObject gameObject5 = new GameObject
							{
								name = "Secrets"
							};
							Object.Instantiate<GameObject>(HUMain.hiddenUnits.LoadAsset<GameObject>("Hadez_Unlock"), gameObject5.transform, true);
							Object.Instantiate<GameObject>(HUMain.hiddenUnits.LoadAsset<GameObject>("EmpSword_Unlock1"), gameObject5.transform, true);
							Object.Instantiate<GameObject>(HUMain.hiddenUnits.LoadAsset<GameObject>("EmpSword_Unlock2"), gameObject5.transform, true);
							Object.Instantiate<GameObject>(HUMain.hiddenUnits.LoadAsset<GameObject>("EmpSword_Unlock3"), gameObject5.transform, true);
							Object.Instantiate<GameObject>(HUMain.hiddenUnits.LoadAsset<GameObject>("EmpSword_Unlock4"), gameObject5.transform, true);
							Object.Instantiate<GameObject>(HUMain.hiddenUnits.LoadAsset<GameObject>("EmpSword_Unlock5"), gameObject5.transform, true);
							Object.Instantiate<GameObject>(HUMain.hiddenUnits.LoadAsset<GameObject>("EmpSword_Unlock6"), gameObject5.transform, true);
							Object.Instantiate<GameObject>(HUMain.hiddenUnits.LoadAsset<GameObject>("GrievingTitan_Unlock"), gameObject5.transform, true);
						}
						else
						{
							bool flag14 = scene.name == "01_Lvl1_Tribal_VC";
							if (flag14)
							{
								GameObject gameObject6 = new GameObject
								{
									name = "Secrets"
								};
								Object.Instantiate<GameObject>(HUMain.hiddenUnits.LoadAsset<GameObject>("WD_Unlock"), gameObject6.transform, true);
							}
							else
							{
								bool flag15 = scene.name == "01_Lvl2_Tribal_VC";
								if (flag15)
								{
									GameObject gameObject7 = new GameObject
									{
										name = "Secrets"
									};
									Object.Instantiate<GameObject>(HUMain.hiddenUnits.LoadAsset<GameObject>("Shaman_Unlock"), gameObject7.transform, true);
									bool flag16 = ServiceLocator.GetService<ISaveLoaderService>().HasUnlockedSecret("BILLY_SWORD");
									if (flag16)
									{
										Object.Instantiate<GameObject>(HUMain.hiddenUnits.LoadAsset<GameObject>("BillyKey_Unlock2"), gameObject7.transform, true);
									}
								}
								else
								{
									bool flag17 = scene.name == "01_Sandbox_Tribal_01_VC";
									if (flag17)
									{
										GameObject gameObject8 = new GameObject
										{
											name = "Secrets"
										};
										Object.Instantiate<GameObject>(HUMain.hiddenUnits.LoadAsset<GameObject>("Gatherer_Unlock"), gameObject8.transform, true);
										Object.Instantiate<GameObject>(HUMain.hiddenUnits.LoadAsset<GameObject>("Clubmaster_Unlock"), gameObject8.transform, true);
									}
									else
									{
										bool flag18 = scene.name == "02_Lvl1_Farmer_VC";
										if (flag18)
										{
											GameObject gameObject9 = new GameObject
											{
												name = "Secrets"
											};
											Object.Instantiate<GameObject>(HUMain.hiddenUnits.LoadAsset<GameObject>("Butcher_Unlock"), gameObject9.transform, true);
										}
										else
										{
											bool flag19 = scene.name == "02_Lvl2_Farmer_VC";
											if (flag19)
											{
												GameObject gameObject10 = new GameObject
												{
													name = "Secrets"
												};
												Object.Instantiate<GameObject>(HUMain.hiddenUnits.LoadAsset<GameObject>("Chicken_Unlock1"), gameObject10.transform, true);
												Object.Instantiate<GameObject>(HUMain.hiddenUnits.LoadAsset<GameObject>("Chicken_Unlock2"), gameObject10.transform, true);
												Object.Instantiate<GameObject>(HUMain.hiddenUnits.LoadAsset<GameObject>("Chicken_Unlock3"), gameObject10.transform, true);
												Object.Instantiate<GameObject>(HUMain.hiddenUnits.LoadAsset<GameObject>("Chicken_Unlock4"), gameObject10.transform, true);
												Object.Instantiate<GameObject>(HUMain.hiddenUnits.LoadAsset<GameObject>("Chicken_Unlock5"), gameObject10.transform, true);
											}
											else
											{
												bool flag20 = scene.name == "03_Lvl1_Ancient_VC";
												if (flag20)
												{
													GameObject gameObject11 = new GameObject
													{
														name = "Secrets"
													};
													Object.Instantiate<GameObject>(HUMain.hiddenUnits.LoadAsset<GameObject>("Helicopter_Unlock"), gameObject11.transform, true);
												}
												else
												{
													bool flag21 = scene.name == "03_Lvl2_Ancient_VC";
													if (flag21)
													{
														GameObject gameObject12 = new GameObject
														{
															name = "Secrets"
														};
														Object.Instantiate<GameObject>(HUMain.hiddenUnits.LoadAsset<GameObject>("Mathematician_Unlock"), gameObject12.transform, true);
														Object.Instantiate<GameObject>(HUMain.hiddenUnits.LoadAsset<GameObject>("Philosopher_Unlock"), gameObject12.transform, true);
														Object.Instantiate<GameObject>(HUMain.hiddenUnits.LoadAsset<GameObject>("Apollo_Unlock"), gameObject12.transform, true);
													}
													else
													{
														bool flag22 = scene.name == "03_Sandbox_Ancient_01_VC";
														if (flag22)
														{
															GameObject gameObject13 = new GameObject
															{
																name = "Secrets"
															};
															Object.Instantiate<GameObject>(HUMain.hiddenUnits.LoadAsset<GameObject>("TrojanChicken_Unlock"), gameObject13.transform, true);
															Object.Instantiate<GameObject>(HUMain.hiddenUnits.LoadAsset<GameObject>("Ares_Unlock"), gameObject13.transform, true);
															Object.Instantiate<GameObject>(HUMain.hiddenUnits.LoadAsset<GameObject>("Centaur_Unlock"), gameObject13.transform, true);
															Object.Instantiate<GameObject>(HUMain.hiddenUnits.LoadAsset<GameObject>("AncientTank_Unlock"), gameObject13.transform, true);
														}
														else
														{
															bool flag23 = scene.name == "04_Lvl1_Viking_VC";
															if (flag23)
															{
																GameObject gameObject14 = new GameObject
																{
																	name = "Secrets"
																};
																Object.Instantiate<GameObject>(HUMain.hiddenUnits.LoadAsset<GameObject>("Warlord_Unlock"), gameObject14.transform, true);
																Object.Instantiate<GameObject>(HUMain.hiddenUnits.LoadAsset<GameObject>("TheReaver_Unlock"), gameObject14.transform, true);
																Object.Instantiate<GameObject>(HUMain.hiddenUnits.LoadAsset<GameObject>("RuneMage_Unlock"), gameObject14.transform, true);
															}
															else
															{
																bool flag24 = scene.name == "04_Sandbox_Viking_VC";
																if (flag24)
																{
																	GameObject gameObject15 = new GameObject
																	{
																		name = "Secrets"
																	};
																	Object.Instantiate<GameObject>(HUMain.hiddenUnits.LoadAsset<GameObject>("DreadKing_Unlock"), gameObject15.transform, true);
																	Object.Instantiate<GameObject>(HUMain.hiddenUnits.LoadAsset<GameObject>("Thor_Unlock"), gameObject15.transform, true);
																	Object.Instantiate<GameObject>(HUMain.hiddenUnits.LoadAsset<GameObject>("Odin_Unlock"), gameObject15.transform, true);
																}
																else
																{
																	bool flag25 = scene.name == "05_Lvl1_Medieval_VC";
																	if (flag25)
																	{
																		GameObject gameObject16 = new GameObject
																		{
																			name = "Secrets"
																		};
																		Object.Instantiate<GameObject>(HUMain.hiddenUnits.LoadAsset<GameObject>("Tower_Unlock"), gameObject16.transform, true);
																		Object.Instantiate<GameObject>(HUMain.hiddenUnits.LoadAsset<GameObject>("Thief_Unlock"), gameObject16.transform, true);
																	}
																	else
																	{
																		bool flag26 = scene.name == "05_Lvl2_Medieval_VC";
																		if (flag26)
																		{
																			GameObject gameObject17 = new GameObject
																			{
																				name = "Secrets"
																			};
																			Object.Instantiate<GameObject>(HUMain.hiddenUnits.LoadAsset<GameObject>("Ignislasher_Unlock"), gameObject17.transform, true);
																			Object.Instantiate<GameObject>(HUMain.hiddenUnits.LoadAsset<GameObject>("Templar_Unlock"), gameObject17.transform, true);
																			Object.Instantiate<GameObject>(HUMain.hiddenUnits.LoadAsset<GameObject>("Bishop_Unlock"), gameObject17.transform, true);
																		}
																		else
																		{
																			bool flag27 = scene.name == "08_Lvl1_Pirate_VC";
																			if (flag27)
																			{
																				GameObject gameObject18 = new GameObject
																				{
																					name = "Secrets"
																				};
																				bool flag28 = ServiceLocator.GetService<ISaveLoaderService>().HasUnlockedSecret("BILLY_SWORD");
																				if (flag28)
																				{
																					Object.Instantiate<GameObject>(HUMain.hiddenUnits.LoadAsset<GameObject>("BillyKey_Unlock1"), gameObject18.transform, true);
																				}
																			}
																			else
																			{
																				bool flag29 = scene.name == "09_Lvl1_Western_VC";
																				if (flag29)
																				{
																					GameObject gameObject19 = new GameObject
																					{
																						name = "Secrets"
																					};
																					Object.Instantiate<GameObject>(HUMain.hiddenUnits.LoadAsset<GameObject>("Prospector_Unlock"), gameObject19.transform, true);
																				}
																				else
																				{
																					bool flag30 = scene.name == "05_Sandbox_Medieval_VC";
																					if (flag30)
																					{
																						GameObject gameObject20 = new GameObject
																						{
																							name = "Secrets"
																						};
																						Object.Instantiate<GameObject>(HUMain.hiddenUnits.LoadAsset<GameObject>("FlailMaster_Unlock"), gameObject20.transform, true);
																						Object.Instantiate<GameObject>(HUMain.hiddenUnits.LoadAsset<GameObject>("SpiderMage_Unlock"), gameObject20.transform, true);
																						Object.Instantiate<GameObject>(HUMain.hiddenUnits.LoadAsset<GameObject>("MayhemGunner_Unlock"), gameObject20.transform, true);
																						Object.Instantiate<GameObject>(HUMain.hiddenUnits.LoadAsset<GameObject>("Billy_Unlock"), gameObject20.transform, true);
																					}
																					else
																					{
																						bool flag31 = scene.name == "09_Lvl1_Fantasy_Evil_VC";
																						if (flag31)
																						{
																							GameObject gameObject21 = new GameObject
																							{
																								name = "Secrets"
																							};
																							Object.Instantiate<GameObject>(HUMain.hiddenUnits.LoadAsset<GameObject>("BusinessMan_Unlock"), gameObject21.transform, true);
																							bool flag32 = ServiceLocator.GetService<ISaveLoaderService>().HasUnlockedSecret("BILLY_SWORD");
																							if (flag32)
																							{
																								Object.Instantiate<GameObject>(HUMain.hiddenUnits.LoadAsset<GameObject>("BillyKey_Unlock3"), gameObject21.transform, true);
																							}
																						}
																						else
																						{
																							bool flag33 = scene.name == "09_Lvl1_Fantasy_Good_VC";
																							if (flag33)
																							{
																								GameObject gameObject22 = new GameObject
																								{
																									name = "Secrets"
																								};
																								Object.Instantiate<GameObject>(HUMain.hiddenUnits.LoadAsset<GameObject>("Aetherian_Unlock"), gameObject22.transform, true);
																								Object.Instantiate<GameObject>(HUMain.hiddenUnits.LoadAsset<GameObject>("Angel_Unlock"), gameObject22.transform, true);
																								Object.Instantiate<GameObject>(HUMain.hiddenUnits.LoadAsset<GameObject>("Pegasus_Unlock"), gameObject22.transform, true);
																								Object.Instantiate<GameObject>(HUMain.hiddenUnits.LoadAsset<GameObject>("Seraphim_Unlock"), gameObject22.transform, true);
																							}
																						}
																					}
																				}
																			}
																		}
																	}
																}
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x060000AC RID: 172 RVA: 0x00008BAC File Offset: 0x00006DAC
		public override void Localize(LocalizationHolder holder)
		{
			holder.languages.AddRange(HUMain.hiddenUnits.LoadAsset<GameObject>("Lang").GetComponent<LocalizationHolder>().languages);
		}

		// Token: 0x04000121 RID: 289
		public static ConfigEntry<bool> ConfigInfiniteScalingEnabled;
	}
}
