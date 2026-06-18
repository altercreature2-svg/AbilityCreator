using BepInEx;
using CASA.Tools;
using DM;
using HarmonyLib;
using AC.AssetManaging;
using AC.ExampleAbilites;
using AC.Node_Related_Scripts.Migrater;
using AC.Node_Related_Scripts.SavingStuff;
using AC.NodeScripts;
using Landfall.TABS;
using Landfall.TABS.UnitEditor;
using Sirenix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using TFBGames;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using IDK.Node_Related_Scripts;
using IDK.AbilityHandling;
namespace AC
{
    [BepInPlugin("AAC", "Alter Ability Creator", "2.6.1")]
    public class AbilityCreator : CASA.Main.CASAMod
    {
        public static GameObject sliceEffect;
        public static List<DatabaseID> nodeIDS = new List<DatabaseID>();
        public static List<BundledAbilitesManager.BundledAbility> bundledAbilitiesQueue = new List<BundledAbilitesManager.BundledAbility>();
        public static string Guide = "";
        public static VanillaAssetManager assetManager;
        public static Reapeter reapeter;
        public static Dictionary<string, UnitBlueprint> units
        {
            get
            {
                return assetManager.GetOld<UnitBlueprint>("unit");
            }
        }
        public static Dictionary<string, GameObject> abilites
        {
            get
            {
                return assetManager.GetOld<GameObject>("move");
            }
        }
        public static Dictionary<string, Type> components
        {
            get
            {
                return assetManager.GetOld<Type>("component");
            }
        }
        public static Dictionary<string, Sprite> sprites
        {
            get
            {
                return assetManager.GetOld<Sprite>("sprite");
            }
        }
        public static Dictionary<string, GameObject> effects
        {
            get
            {
                return assetManager.GetOld<GameObject>("effect");
            }
        }
        public static Dictionary<string, GameObject> explosions
        {
            get
            {
                return assetManager.GetOld<GameObject>("explosion");
            }
        }
        public static Dictionary<string, GameObject> particles
        {
            get
            {
                return assetManager.GetOld<GameObject>("particle");
            }
        }
        public static Dictionary<string, GameObject> projectiles
        {
            get
            {
                return assetManager.GetOld<GameObject>("projectile");
            }
        }
        public static Dictionary<string, GameObject> weapons
        {
            get
            {
                return assetManager.GetOld<GameObject>("weapon");
            }
        }
        public static Dictionary<string, GameObject> props
        {
            get
            {
                return assetManager.GetOld<GameObject>("prop");
            }
        }
        public static List<string> sounds
        {
            get
            {
                return assetManager.GetAllAssets<string>("sound").ToList();
            }
        }
        public override void Init()
        {
        }
        public override ModPropreties GetModPropreties()
        {
            return new ModPropreties(
                "Ability Creator",
                "AlterCreature",
                "A mod that adds a ability creator to the game, allowing you to create your own abilities and share them with others",
                "hi",
                null
            );
        }
        public void Awake()
        {
            Harmony harmony = new Harmony("Alter.AbilityCreator");
            harmony.PatchAll();
            Code.commnet = "Dear Code Viewer (pervert), prepare your self for the most unoptomized, evil, straight up SHIT code you will ever see";
            Code.commnet = "Please understand that most of this was made like 2 years ago, I'm aware how bad it is and i hope to rewrite most of it someday";
            Code.commnet = "Today is not that day though";
            if (!Directory.Exists(FilePaths.AbilityCreatorPath))
                Directory.CreateDirectory(FilePaths.AbilityCreatorPath);
            if (!Directory.Exists(FilePaths.AbilitesPath))
                Directory.CreateDirectory(FilePaths.AbilitesPath);
            
            UpdateSaveManager.Handle();
            string[] abilities = FilePaths.AbilityDirs;
            for (int i = 0; i < abilities.Length; i++)
            {
                string[] abilityFiles = Directory.GetFiles(abilities[i]);
                if (Path.GetExtension(abilityFiles[i]) == ".ability")
                {
                    // Migrate
                    VirtualNodeScene savedNodeScene = NodeSceneMigrater.GetNewSavedNodeScene(Serialize.LoadJson<LegacySavedNodeScene>(File.ReadAllText(abilityFiles[i])));
                    // Move
                    if (!Directory.Exists(FilePaths.AbilityCreatorPath + "/OldAbilites"))
                        Directory.CreateDirectory(FilePaths.AbilityCreatorPath + "/OldAbilites");
                    File.Copy(abilityFiles[i], FilePaths.AbilityCreatorPath + "/OldAbilites/" + Path.GetFileNameWithoutExtension(abilityFiles[i]));
                    File.Delete(abilityFiles[i]);
                    // Add new
                    string json = Serialize.SaveJson(savedNodeScene);
                    FileManager.WriteAbility(savedNodeScene.abilityName, json);
                }
            }
            StartCoroutine(Call());
            
            reapeter = gameObject.AddComponent<Reapeter>();
        }
        public string GetNiceName(string name)
        {
            if (Localizer.GetLanguage(Localizer.Language.LANG_EN_US).ContainsKey(name))
                return Localizer.GetLanguage(Localizer.Language.LANG_EN_US)[name];

            name = (name.Contains("_1 Prefabs_VB") ? name.Replace("_1 Prefabs_VB", "") : (name.Contains("_1 Weapons_VB") ? name.Replace("_1 Weapons_VB", "") : name));
            name = (name.Contains("_4 Moves_VB") ? name.Replace("_4 Moves_VB", "") : (name.Contains("_2 Projectiles_VB") ? name.Replace("_2 Projectiles_VB", "") : name));
            name = (name.Contains("_3 Effects_VB") ? name.Replace("_3 Effects_VB", "") : (name.Contains("_0 UnitBases_VB") ? name.Replace("_0 UnitBases_VB", "") : name));
            return name;
        }
        private IEnumerator Call()
        {
            yield return new WaitUntil(() => FindObjectOfType<ServiceLocator>() != null);
            yield return new WaitUntil(() => ServiceLocator.GetService<ISaveLoaderService>() != null);
            yield return new WaitUntil(() => GameObject.Find("Quit") != null);
            yield return new WaitUntil(() => GameObject.Find("Quit").GetComponentInChildren<LocalizeText>() != null);
            yield return new WaitUntil(() => GameObject.Find("Quit").GetComponentInChildren<LocalizeText>().LocaleID == "BUTTON_QUIT");


            GameObject sliceObj = ContentDatabase.Instance().LandfallContentDatabase.GetCombatMove(new DatabaseID(-1, 1998224969));
            sliceEffect = sliceObj.GetComponent<BlockMove>().sliceEffect;
            BundleManager.Setup();
            sceneManager = new GameObject("Scene Manager").AddComponent<StreamedSceneManager>();
            DontDestroyOnLoad(sceneManager);
            var code = new GameObject("Code").AddComponent<Code>();
            var savePatch = new GameObject("AbilityCreatorSavePatch").AddComponent<AbilityCreatorSavePatch>();
            DontDestroyOnLoad(code.gameObject);
            code.Begin();
            StartCoroutine(code.SpawnButton_Internal());

            Code.commnet = "------------------File stuff------------------------";
            assetManager = new VanillaAssetManager();
            Code.commnet = "------------------INIT------------------------";
            CreateNodeBlueprint("BATTLE_BEGIN", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {

            },  Color.grey, "When Battle Begins", "Triggers", node: typeof(WhenBattleBegins));
            CreateNodeBlueprint("UNIT_SPAWN", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {

            },  Color.grey, "When unit spawns", "Triggers", node: typeof(WhenUnitSpawned));
            CreateNewNodeBlueprint("ENDBATTLE", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger,
                NodeBlueprint.ConnectionClass.Triggered,
            }, new List<NodeBlueprint.Field>()
            {
                new NodeBlueprint.Field()
                {
                    name = "stupidDumbassNode",
                    isDropdown = true,
                    dropDowns =new string[]
                    {
                        "Win",
                        "Lose"
                    }
                }
            }, node: typeof(EndBattle),  Color.white, "End battle", "Misc");
            CreateNewNodeBlueprint("GETMAXUNITHEALTH", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.ReciveUnit,
                NodeBlueprint.ConnectionClass.GiveVariable,
            }, new List<NodeBlueprint.Field>()
            {
                new NodeBlueprint.Field()
                {
                    name = "valueType",
                    isDropdown = true,
                    dropDowns =new string[]
                    {
                        "Normal",
                        "Precentage"
                    }
                }
            }, node: typeof(GetMaxUnitHealth),  Color.white, "Get max unit health", "Data");
            CreateNewNodeBlueprint("GETUNITHEALTH", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.ReciveUnit,
                NodeBlueprint.ConnectionClass.GiveVariable,
            }, new List<NodeBlueprint.Field>()
            {
                new NodeBlueprint.Field()
                {
                    name = "valueType",
                    isDropdown = true,
                    dropDowns =new string[]
                    {
                        "Normal",
                        "Precentage"
                    }
                }
            }, node: typeof(GetUnitHealth),  Color.white, "Get unit health", "Data");
            CreateNewNodeBlueprint("SETUNITHEALTH", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Triggered,
                NodeBlueprint.ConnectionClass.ReciveUnit,
                NodeBlueprint.ConnectionClass.Trigger,
            }, new List<NodeBlueprint.Field>()
            {
                new NodeBlueprint.Field("value", TMP_InputField.ContentType.DecimalNumber),
                new NodeBlueprint.Field()
                {
                    name = "valueType",
                    isDropdown = true,
                    dropDowns =new string[]
                    {
                        "Set",
                        "Add",
                        "Set (%)",
                        "Add (%)",
                        "Multiply",

                    }
                }
            }, node: typeof(SetUnitHealth),  Color.white, "Set unit health", "Data");
            CreateNewNodeBlueprint("SETUNITDAMAGE", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Triggered,
                NodeBlueprint.ConnectionClass.ReciveUnit,
                NodeBlueprint.ConnectionClass.Trigger,
            }, new List<NodeBlueprint.Field>()
            {
                new NodeBlueprint.Field("value", TMP_InputField.ContentType.DecimalNumber),
                new NodeBlueprint.Field()
                {
                    name = "valueType",
                    isDropdown = true,
                    dropDowns =new string[]
                    {
                        "Set",
                        "Add",
                        "Set (%)",
                        "Add (%)",
                        "Multiply",

                    }
                }
            }, node: typeof(SetUnitDamage),  Color.white, "Set unit damage", "Data");
            CreateNewNodeBlueprint("SETUNITCOOLDOWN", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Triggered,
                NodeBlueprint.ConnectionClass.ReciveUnit,
                NodeBlueprint.ConnectionClass.Trigger,
            }, new List<NodeBlueprint.Field>()
            {
                new NodeBlueprint.Field("value", TMP_InputField.ContentType.DecimalNumber),
                new NodeBlueprint.Field()
                {
                    name = "valueType",
                    isDropdown = true,
                    dropDowns =new string[]
                    {
                        "Set",
                        "Add",
                        "Set (%)",
                        "Add (%)",
                        "Multiply",

                    }
                }
            }, node: typeof(SetUnitDamage),  Color.white, "Set unit cooldown", "Data");

            CreateNewNodeBlueprint("QUICKBUFF", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Triggered,
                NodeBlueprint.ConnectionClass.ReciveUnit,
                NodeBlueprint.ConnectionClass.Trigger,
            }, new List<NodeBlueprint.Field>()
            {
                new NodeBlueprint.Field("length", TMP_InputField.ContentType.DecimalNumber),
                new NodeBlueprint.Field("amount", TMP_InputField.ContentType.DecimalNumber),
                new NodeBlueprint.Field()
                {
                    name = "mode",
                    isDropdown = true,
                    dropDowns =new string[]
                    {
                        "All",
                        "Heal only",
                        "Increase damage",
                        "Speed up",
                        "Speed up attacks",
                    }
                }
            }, node: typeof(QuickBuff),  Color.white, "Quick buff", "Data");




            CreateNodeBlueprint("LOG", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Triggered,
                NodeBlueprint.ConnectionClass.Trigger,
                NodeBlueprint.ConnectionClass.ReciveVariable,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
            },  Color.white, "Log variable", "Variables", node: typeof(LogVariable));
            CreateNodeBlueprint("LOGANY", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Triggered,
                NodeBlueprint.ConnectionClass.Trigger,
                NodeBlueprint.ConnectionClass.ReciveAnything,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
            },  Color.white, "Log anything", "Gameobjects", node: typeof(LogAnything));


            CreateNewNodeBlueprint("ABILITYACTVIATE", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger
            }, new List<NodeBlueprint.Field>()
            {
                 new NodeBlueprint.Field("Delay", TMP_InputField.ContentType.DecimalNumber),
                 new NodeBlueprint.Field("Range", TMP_InputField.ContentType.DecimalNumber),
            }, node: typeof(WhenAbilityTriggered),  Color.grey, "When ability triggered...", "Triggers"); ;


            CreateNewNodeBlueprint("UNITWASATTACKED", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger
            }, new List<NodeBlueprint.Field>()
            {
                 new NodeBlueprint.Field("Cooldown", TMP_InputField.ContentType.DecimalNumber),
                 new NodeBlueprint.Field("Range", TMP_InputField.ContentType.DecimalNumber),
            }, node: typeof(UnitWasAttacked),  Color.grey, "When unit gets attacked...", "Triggers");
            CreateNodeBlueprint("INTERVAL", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
                { "Interval", TMP_InputField.ContentType.DecimalNumber },
            },  Color.grey, "Do Every...", "Triggers", node: typeof(DoEveryNode));
            CreateNodeBlueprint("WHENUNITDIE", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
            },  Color.grey, "When Unit Dies...", "Triggers", node: typeof(WhenUnitDies));
            /*CreateNodeBlueprint("WHENUNITDAMAGES", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.Trigger,
                NodeBlueprint.ConnectionType.GiveUnit
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
            },  Color.grey, "When unit damages...", "Triggers", node: typeof(WhenUnitDamages));*/
            CreateNodeBlueprint("WHENUNITDAMAGED", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger,
                NodeBlueprint.ConnectionClass.GiveUnit
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
            },  Color.grey, "When unit damaged...", "Triggers", node: typeof(WhenUnitDamaged))
            ;
            CreateNodeBlueprint("WHENUNITATTACKS", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger,
                NodeBlueprint.ConnectionClass.GiveUnit,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
            },  Color.grey, "When unit attacks...", "Triggers", node: typeof(WhenUnitAttacks));
            CreateNewNodeBlueprint("PROJECTILERANGE", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger,
                NodeBlueprint.ConnectionClass.GiveGameObject,
            }, new List<NodeBlueprint.Field>()
            {
                new NodeBlueprint.Field("Range",TMP_InputField.ContentType.DecimalNumber),
                new NodeBlueprint.Field("Block Power",TMP_InputField.ContentType.DecimalNumber),
            }, node: typeof(WhenProjectileEntersRange),  Color.grey, "When projectile in range...", "Triggers");
            CreateNodeBlueprint("IS_BATTLE", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger,
                NodeBlueprint.ConnectionClass.Triggered,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {

            },  Color.grey, "Run if in battle state", "Control", node: typeof(IsBattleState));
            CreateNodeBlueprint("IS_DEAD", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger,
                NodeBlueprint.ConnectionClass.Triggered,
                NodeBlueprint.ConnectionClass.ReciveUnit,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {

            },  Color.grey, "Run if unit is dead", "Control", node: typeof(IsDead));
            CreateNodeBlueprint("IS_NOTDEAD", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger,
                NodeBlueprint.ConnectionClass.Triggered,
                NodeBlueprint.ConnectionClass.ReciveUnit,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {

            },  Color.grey, "Run if unit is alive", "Control", node: typeof(IsAlive));
            CreateNodeBlueprint("PAUSE", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger,
                NodeBlueprint.ConnectionClass.Triggered,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
                { "Seconds", TMP_InputField.ContentType.DecimalNumber },

            },  Color.grey, "Do after pause", "Control", node: typeof(PauseNode));
            CreateNodeBlueprint("REAPET", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger,
                NodeBlueprint.ConnectionClass.Triggered,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
                { "Times", TMP_InputField.ContentType.IntegerNumber },
                { "Delay", TMP_InputField.ContentType.DecimalNumber},
            },  Color.grey, "Repeat", "Control", node: typeof(ReapetNode));
            CreateNodeBlueprint("REAPETVAR", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger,
                NodeBlueprint.ConnectionClass.Triggered,
                NodeBlueprint.ConnectionClass.ReciveVariable,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
                { "Delay", TMP_InputField.ContentType.DecimalNumber},
            },  Color.grey, "Repeat for variable", "Control", node: typeof(ReapetVarNode));
            CreateNewNodeBlueprint("FOREACH", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger,
                NodeBlueprint.ConnectionClass.Triggered,
                NodeBlueprint.ConnectionClass.ReciveAnything,
                NodeBlueprint.ConnectionClass.GiveAnything,

            }, new List<NodeBlueprint.Field>
            {
                new NodeBlueprint.Field( "Delay", TMP_InputField.ContentType.DecimalNumber),
                new NodeBlueprint.Field("filter", TMP_InputField.ContentType.Standard)
                {
                    isDropdown = true,
                    dropDowns = new string[]
                    {
                        "All",
                        "Gameobjects only",
                        "Units only",
                        "Components only",
                    }
                }
            }, node: typeof(ForEachNode),  Color.grey, "Repeat for each value", "Control");
            CreateNewNodeBlueprint("FILTER", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.ReciveAnything,
                NodeBlueprint.ConnectionClass.GiveAnything,

            }, new List<NodeBlueprint.Field>
            {
                new NodeBlueprint.Field("filter", TMP_InputField.ContentType.Standard)
                {
                    isDropdown = true,
                    dropDowns = new string[]
                    {
                        "Gameobjects only",
                        "Units only",
                        "Components only",
                        "Other",
                    }
                }
            }, node: typeof(FilterNode),  Color.grey, "Filter values", "Control");



            CreateNodeBlueprint("NEWOBJ", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.GiveGameObject
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
                { "Object name", TMP_InputField.ContentType.Standard },
            },  Color.green, "Create gameObject", "Gameobjects", node: typeof(CreateGameobject));



            CreateNodeBlueprint("ENEMY", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.GiveUnit
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
            },  Color.green, "Enemy unit", "Units", node: typeof(EnemyNode));
            CreateNodeBlueprint("SELF", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.GiveUnit
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
            },  Color.green, "Self unit", "Units", node: typeof(SelfNode));
            CreateNodeBlueprint("EXPSENSIVETEAMMATE", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.ReciveUnit,
                NodeBlueprint.ConnectionClass.GiveUnit,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
            },  Color.green, "Most expensive teammate of unit", "Units", node: typeof(MostExspensiveTeamMateUnit));
            CreateNodeBlueprint("EXPSENSIVEENEMY", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.ReciveUnit,
                NodeBlueprint.ConnectionClass.GiveUnit,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
            },  Color.green, "Most expensive enemy of unit", "Units", node: typeof(MostExspensiveEnemy));
            CreateNodeBlueprint("ALLTEAM", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.GiveUnit
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
            },  Color.green, "All units of same team", "Units", node: typeof(AllTeam));
            CreateNodeBlueprint("ALLOTHERTEAM", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.GiveUnit
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
            },  Color.green, "All units of other team", "Units", node: typeof(AllOtherTeam));


            CreateNodeBlueprint("GETUNIT", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.GiveUnit,
                NodeBlueprint.ConnectionClass.ReciveGameObject
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
            },  Color.green, "Get unit from gameobject", "Unit Tools", node: typeof(GetUnitFromGameobject));
            CreateNodeBlueprint("TEAMMATE", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.GiveUnit,
                NodeBlueprint.ConnectionClass.ReciveUnit
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
            },  Color.green, "Closest team mate of unit", "Unit Tools", node: typeof(ClosestTeamMateUnit));
            CreateNodeBlueprint("FREAZE", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger,
                NodeBlueprint.ConnectionClass.Triggered,
                NodeBlueprint.ConnectionClass.ReciveUnit
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
                { "Length", TMP_InputField.ContentType.DecimalNumber}
            },  Color.magenta, "freeze unit for", "Unit Tools", node: typeof(FreezeNode));
            CreateNodeBlueprint("DONTWALK", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger,
                NodeBlueprint.ConnectionClass.Triggered,
                NodeBlueprint.ConnectionClass.ReciveUnit
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
                { "Length", TMP_InputField.ContentType.DecimalNumber}
            },  Color.magenta, "make unit not walk for", "Unit Tools", node: typeof(DontWalkNode));
            CreateNewNodeBlueprint("HOLDPOSITION", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger,
                NodeBlueprint.ConnectionClass.Triggered,
                NodeBlueprint.ConnectionClass.ReciveUnit,
                NodeBlueprint.ConnectionClass.ReciveGameObject,
            }, new List<NodeBlueprint.Field>
            {
                new NodeBlueprint.Field( "Length", TMP_InputField.ContentType.DecimalNumber),
                new NodeBlueprint.Field( "X", TMP_InputField.ContentType.Standard)
                {
                    isDropdown = true,
                    dropDowns = new string[] {"X Axis", "Off"}
                },
                new NodeBlueprint.Field( "Y", TMP_InputField.ContentType.Standard)
                {
                    isDropdown = true,
                    dropDowns = new string[] {"Y Axis", "Off"}
                },
                new NodeBlueprint.Field("Z", TMP_InputField.ContentType.Standard)
                {
                    isDropdown = true,
                    dropDowns = new string[] {"Z Axis", "Off"}
                },
            }, node: typeof(HoldPostionNode),  Color.magenta, "hold body part positions for", "Unit Tools");

            CreateNewNodeBlueprint("GAMEOBJ", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.ReciveUnit,
                NodeBlueprint.ConnectionClass.GiveGameObject
            }
            , new List<NodeBlueprint.Field>
            {
                new NodeBlueprint.Field()
                {
                    name = "BodyPart",
                    isDropdown = true,
                    dropDowns = new string[]
                    {
                        "Root",
                        "Head",
                        "Torso",
                        "Hip",
                        "Right Arm",
                        "Left Arm",
                        "Right Hand",
                        "Left Hand",
                        "Right Knee",
                        "Left Knee",
                        "Right Foot",
                        "Left Foot",
                        "Mesh",
                        "Both Foots",
                        "Both Hands",
                        "All",
                    }
                }
            }, typeof(GetGameobject),  Color.green, "Get body part of unit", "Unit Tools");
            CreateNewNodeBlueprint("CLOTH", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.ReciveUnit,
                NodeBlueprint.ConnectionClass.GiveGameObject
            }
            , new List<NodeBlueprint.Field>
            {
                new NodeBlueprint.Field()
                {
                    name = "Cloth",
                    isDropdown = true,
                    dropDowns = new string[]
                    {
                        "All",
                        "Head",
                        "Torso",
                        "Arms",
                        "Pants",
                        "Shoes",
                    }
                }
            }, typeof(GetClothes),  Color.green, "Get clothes of unit", "Unit Tools");
            CreateNewNodeBlueprint("ABILITIES", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.ReciveUnit,
                NodeBlueprint.ConnectionClass.GiveGameObject
            }
            , new List<NodeBlueprint.Field>
            {
            }, typeof(GetAbilites),  Color.green, "Get abilites of unit", "Unit Tools");
            CreateNewNodeBlueprint("GETWEAPON", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.ReciveUnit,
                NodeBlueprint.ConnectionClass.GiveGameObject
            }
            , new List<NodeBlueprint.Field>
            {
                new NodeBlueprint.Field()
                {
                    name = "WEAPON",
                    isDropdown = true,
                    dropDowns = new string[]
                    {
                        "Both",
                        "Left Weapon",
                        "Right Weapon",
                    }
                }
            }, typeof(GetWeapon),  Color.green, "Get weapon of unit", "Weapons");
            CreateNewNodeBlueprint("SETFIST", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger,
                NodeBlueprint.ConnectionClass.Triggered,
                NodeBlueprint.ConnectionClass.ReciveUnit,
            }
            , new List<NodeBlueprint.Field>
            {
            }, typeof(SetFistNode),  Color.green, "Make unit use fists", "Weapons");
            CreateNewNodeBlueprint("GETEYE", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.ReciveUnit,
                NodeBlueprint.ConnectionClass.GiveGameObject
            }
            , new List<NodeBlueprint.Field>
            {
            }, typeof(GetEye),  Color.green, "Get eyes of unit", "Unit Tools");

            CreateNewNodeBlueprint("GETWEAPONGLOBAL", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.GiveGameObject
            }, new List<NodeBlueprint.Field>
            {
                 new NodeBlueprint.Field("Weapon name", TMP_InputField.ContentType.Standard)
                 {
                     fieldType = NodeBlueprint.Field.FieldType.Weapon
                 }
             }, node: typeof(GetWeaponOfName),  Color.magenta, "Get weapon of name", "Weapons");
            CreateNewNodeBlueprint("DEAL_DAMAGE", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger,
                NodeBlueprint.ConnectionClass.Triggered,
                NodeBlueprint.ConnectionClass.ReciveUnit,
            }, new List<NodeBlueprint.Field>()
            {
                new NodeBlueprint.Field("Damage", TMP_InputField.ContentType.DecimalNumber),
                new NodeBlueprint.Field()
                {
                    name = "Type",
                    isDropdown = true,
                    dropDowns = new string[]
                    {
                        "Normal",
                        "Precentage",
                    }
                }
            }, node: typeof(Deal_Damage),  Color.red, "Deal Damage", "Unit Tools");
            CreateNodeBlueprint("DIE", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger,
                NodeBlueprint.ConnectionClass.Triggered,
                NodeBlueprint.ConnectionClass.ReciveUnit,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
            },  Color.red, "Kill", "Unit Tools", node: typeof(DieNode));
            CreateNodeBlueprint("REVIVE", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger,
                NodeBlueprint.ConnectionClass.Triggered,
                NodeBlueprint.ConnectionClass.ReciveUnit,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
            },  Color.red, "Revive unit", "Unit Tools", node: typeof(ReDie));
            CreateNewNodeBlueprint("REVIVEFIXER", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger,
                NodeBlueprint.ConnectionClass.Triggered,
                NodeBlueprint.ConnectionClass.ReciveUnit,
            }, new List<NodeBlueprint.Field>
            {
                new NodeBlueprint.Field()
                {
                    name = "WillBeRevived",
                    isDropdown = true,
                    dropDowns = new string[]
                    {
                        "Will be revived",
                        "Wont be revived"
                    }
                }
            }, node: typeof(SetWillBeRevived),  Color.red, "Set will be revived", "Unit Tools"); ;
            CreateNodeBlueprint("SWAPTEAM", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger,
                NodeBlueprint.ConnectionClass.Triggered,
                NodeBlueprint.ConnectionClass.ReciveUnit,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
            },  Color.red, "Swap unit's team", "Unit Tools", node: typeof(SwapTeam));
            CreateNewNodeBlueprint("SPAWNPROJ", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger,
                NodeBlueprint.ConnectionClass.Triggered,
                NodeBlueprint.ConnectionClass.ReciveGameObject,
                NodeBlueprint.ConnectionClass.GiveGameObject,
            }
            , new List<NodeBlueprint.Field>
            {
                new NodeBlueprint.Field("Projectile", TMP_InputField.ContentType.Standard )
                {
                    fieldType = NodeBlueprint.Field.FieldType.Projectile,
                },
                new NodeBlueprint.Field("Spread", TMP_InputField.ContentType.DecimalNumber ),
                new NodeBlueprint.Field("Team", TMP_InputField.ContentType.DecimalNumber )
                {
                    isDropdown = true,
                    dropDowns = new string[]
                    {
                        "Both teams",
                        "Other team only",
                        "Same team only",
                    }
                },

            }, node: typeof(SpawnProjectileNode),  Color.red, "Spawn projectile", "Projectiles");
            //CreateNewNodeBlueprint("SPAWNPROJAIMED", new List<NodeBlueprint.ConnectionType>()
            //{
            //    NodeBlueprint.ConnectionType.Trigger,
            //    NodeBlueprint.ConnectionType.Triggered,
            //    NodeBlueprint.ConnectionType.ReciveGameObject,
            //    NodeBlueprint.ConnectionType.ReciveUnit,
            //    NodeBlueprint.ConnectionType.GiveGameObject,
            //}
            //, new List<NodeBlueprint.Field>
            //{
            //    new NodeBlueprint.Field("Projectile", TMP_InputField.ContentType.Standard )
            //    {
            //        fieldType = NodeBlueprint.Field.FieldType.Projectile,
            //    },
            //    new NodeBlueprint.Field("Spread", TMP_InputField.ContentType.DecimalNumber ),
            //    new NodeBlueprint.Field("Team", TMP_InputField.ContentType.DecimalNumber )
            //    {
            //        isDropdown = true,
            //        dropDowns = new string[]
            //        {
            //            "Both teams",
            //            "Other team only",
            //            "Same team only",
            //        }
            //    },

            //}, node: typeof(SpawnProjectileAimedNode),  Color.red, "Spawn projectile to target", "Projectiles");
            CreateNewNodeBlueprint("PARRYPROJECTILE", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger,
                NodeBlueprint.ConnectionClass.Triggered,
                NodeBlueprint.ConnectionClass.ReciveGameObject,
            }, new List<NodeBlueprint.Field>()
            {
                new NodeBlueprint.Field()
                {
                    name = "PlaySliceEffect",
                    isDropdown = true,
                    dropDowns = new string[]
                    {
                        "Play Slice",
                        "Don't"
                    }
                },
                new NodeBlueprint.Field()
                {
                    name = "Reflect",
                    isDropdown = true,
                    dropDowns = new string[]
                    {
                        "Reflect",
                        "Don't"
                    }
                }

            }, node: typeof(ParryProjectile),  Color.red, "Parry projectile", "Projectiles");
            CreateNewNodeBlueprint("PLAYSLICE", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger,
                NodeBlueprint.ConnectionClass.Triggered,
                NodeBlueprint.ConnectionClass.ReciveGameObject,
                NodeBlueprint.ConnectionClass.GiveGameObject,
            }, new List<NodeBlueprint.Field>()
            {

            }, node: typeof(PlaySlice),  Color.red, "Play slice effect", "Projectiles");
            CreateNewNodeBlueprint("EXPLOSION", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger,
                NodeBlueprint.ConnectionClass.Triggered,
                NodeBlueprint.ConnectionClass.ReciveGameObject,
                NodeBlueprint.ConnectionClass.GiveGameObject,
            }
            , new List<NodeBlueprint.Field>
            {
                new NodeBlueprint.Field()
                {
                    name = "Explosion name",
                    contentType = TMP_InputField.ContentType.Standard,
                    fieldType = NodeBlueprint.Field.FieldType.Explosion
                },
                new NodeBlueprint.Field("Damage", TMP_InputField.ContentType.DecimalNumber),
                new NodeBlueprint.Field()
                {
                    isDropdown = true,
                    dropDowns = new string[]
                    {
                        "My team",
                        "Other team",
                    },
                    name = "Team",
                },

            }, typeof(SpawnExplosionNode),  Color.red, "Summon Explosion", "Misc");
            CreateNewNodeBlueprint("PARTICLE", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger,
                NodeBlueprint.ConnectionClass.Triggered,
                NodeBlueprint.ConnectionClass.ReciveGameObject,
                NodeBlueprint.ConnectionClass.GiveGameObject,
            }
            , new List<NodeBlueprint.Field>
            {
                new NodeBlueprint.Field("Particle name", TMP_InputField.ContentType.Standard)
                {
                    fieldType = NodeBlueprint.Field.FieldType.Particle
                },
                new NodeBlueprint.Field("Size", TMP_InputField.ContentType.DecimalNumber),
                new NodeBlueprint.Field("Loop", TMP_InputField.ContentType.Standard)
                {
                    isDropdown = true,
                    dropDowns = new string[]
                    {
                        "Loop",
                        "Don't",
                    }
                },
                new NodeBlueprint.Field("Speed", TMP_InputField.ContentType.DecimalNumber),
                new NodeBlueprint.Field("Length", TMP_InputField.ContentType.DecimalNumber),
                new NodeBlueprint.Field("Follow", TMP_InputField.ContentType.DecimalNumber)
                {
                    isDropdown = true,
                    dropDowns = new string[]
                    {
                        "Follow",
                        "Don't"
                    }
                },
            }, typeof(SpawnParticleNode),  Color.red, "Spawn particle", "Misc");

            CreateNewNodeBlueprint("ADDEFFECT", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger,
                NodeBlueprint.ConnectionClass.Triggered,
                NodeBlueprint.ConnectionClass.ReciveUnit,
            }
            , new List<NodeBlueprint.Field>
            {
                new NodeBlueprint.Field( "Effect", TMP_InputField.ContentType.Standard)
                {
                    fieldType = NodeBlueprint.Field.FieldType.Effect,
                },
            }, node: typeof(AddEffectNode),  Color.red, "Add Effect to unit", "Unit Tools");

            CreateNodeBlueprint("STUNWEAPONS", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger,
                NodeBlueprint.ConnectionClass.Triggered,
                NodeBlueprint.ConnectionClass.ReciveUnit,
            }
            , new Dictionary<string, TMP_InputField.ContentType>
            {
                { "Time", TMP_InputField.ContentType.DecimalNumber},


            },  Color.red, "Stun weapons for unit", "Weapons", node: typeof(StunWeaponsNode));

            CreateNewNodeBlueprint("LETGO", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger,
                NodeBlueprint.ConnectionClass.Triggered,
                NodeBlueprint.ConnectionClass.ReciveUnit,
            }
            , new List<NodeBlueprint.Field>()
            {
                new NodeBlueprint.Field()
                {
                    name = "WhichHand",
                    isDropdown = true,
                    dropDowns = new string[]
                    {
                        "Both",
                        "Right hand",
                        "Left hand",
                    }
                }
            }, node: typeof(LetGoNode),  Color.red, "Let go of stuff at hand", "Unit Tools");
            CreateNewNodeBlueprint("SETWEAPON", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger,
                NodeBlueprint.ConnectionClass.Triggered,
                NodeBlueprint.ConnectionClass.ReciveUnit,
                NodeBlueprint.ConnectionClass.ReciveGameObject,
            }
            , new List<NodeBlueprint.Field>()
            {
                new NodeBlueprint.Field()
                {
                    name = "WhichHand",
                    isDropdown = true,
                    dropDowns = new string[]
                    {
                        "Both",
                        "Right",
                        "Left",
                    }
                }
            }, node: typeof(SetWeaponNode),  Color.red, "Set unit's weapon", "Weapons");
            CreateNewNodeBlueprint("SPAWNUNIT", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger,
                NodeBlueprint.ConnectionClass.Triggered,
                NodeBlueprint.ConnectionClass.ReciveGameObject,
                NodeBlueprint.ConnectionClass.GiveUnit,
            }, new List<NodeBlueprint.Field>()
            {

                new NodeBlueprint.Field("unit", TMP_InputField.ContentType.Standard)
                {
                    fieldType = NodeBlueprint.Field.FieldType.Unit,
                },
                new NodeBlueprint.Field()
                {
                    name = "Team",
                    isDropdown = true,
                    dropDowns = new string[]
                    {
                        "My Team",
                        "Other Team",
                    }
                },
            }, typeof(SpawnUnitNode),  Color.red, "Spawn unit at gameobject", "Units");



            CreateNodeBlueprint("ALLABS", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger,
                NodeBlueprint.ConnectionClass.Triggered,
                NodeBlueprint.ConnectionClass.ReciveUnit,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
            },  Color.magenta, "Play all abilites", "Unit Tools", node: typeof(PlayAllAbilites));
            CreateNodeBlueprint("IMMUNE", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger,
                NodeBlueprint.ConnectionClass.Triggered,
                NodeBlueprint.ConnectionClass.ReciveUnit,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
                { "Length", TMP_InputField.ContentType.DecimalNumber },
            },  Color.magenta, "Make Immune for", "Unit Tools", node: typeof(MakeUnitImmune));

            CreateNodeBlueprint("TOGGLEPROPS", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger,
                NodeBlueprint.ConnectionClass.Triggered,
                NodeBlueprint.ConnectionClass.ReciveUnit,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
                 { "From", TMP_InputField.ContentType.IntegerNumber},
                { "To", TMP_InputField.ContentType.IntegerNumber},
            },  Color.magenta, "Toggle Props at indexs", "Unit Tools", node: typeof(ToggleProps));
            CreateNewNodeBlueprint("ADDPROPS", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger,
                NodeBlueprint.ConnectionClass.Triggered,
                NodeBlueprint.ConnectionClass.ReciveUnit,
                NodeBlueprint.ConnectionClass.ReciveGameObject,
            }, new List<NodeBlueprint.Field>
            {
                new NodeBlueprint.Field()
                {
                    name = "cloth",
                    isDropdown = true,
                    dropDowns = new string[]
                    {
                        "Clothes",
                        "Abilites",
                    }
                }
            }, node: typeof(AddProps),  Color.magenta, "Add clothes/abilites to unit (OBSELETE DONT USE)", "Unit Tools",true);
            CreateNewNodeBlueprint("ADDABILITY", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger,
                NodeBlueprint.ConnectionClass.Triggered,
                NodeBlueprint.ConnectionClass.ReciveUnit,
                NodeBlueprint.ConnectionClass.ReciveGameObject,
                NodeBlueprint.ConnectionClass.GiveGameObject,
            }, new List<NodeBlueprint.Field>
            {
            }, node: typeof(AddAbility),  Color.magenta, "Add ability to unit", "Abilites");
            CreateNewNodeBlueprint("GETABILITYGLOBAL", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.GiveGameObject
            }, new List<NodeBlueprint.Field>
            {
                 new NodeBlueprint.Field("Ability name", TMP_InputField.ContentType.Standard)
                 {
                     fieldType = NodeBlueprint.Field.FieldType.Ability
                 }
             }, node: typeof(GetAbilityOfName),  Color.magenta, "Get ability of name", "Abilites");
            CreateNewNodeBlueprint("ADDCLOTHES", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger,
                NodeBlueprint.ConnectionClass.Triggered,
                NodeBlueprint.ConnectionClass.ReciveUnit,
                NodeBlueprint.ConnectionClass.ReciveGameObject,
                NodeBlueprint.ConnectionClass.GiveGameObject,
            }, new List<NodeBlueprint.Field>
            {
            }, node: typeof(AddCloth),  Color.magenta, "Add clothes to unit", "Clothes");
            CreateNewNodeBlueprint("GETCLOTHESGLOBAL", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.GiveGameObject
            }, new List<NodeBlueprint.Field>
            {
                 new NodeBlueprint.Field("Clothing name", TMP_InputField.ContentType.Standard)
                 {
                     fieldType = NodeBlueprint.Field.FieldType.Clothing
                 }
             }, node: typeof(GetClothesOfName),  Color.magenta, "Get clothing of name", "Clothes");
            CreateNodeBlueprint("ADDFORCE", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger,
                NodeBlueprint.ConnectionClass.Triggered,
                NodeBlueprint.ConnectionClass.ReciveGameObject,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
                { "Forward", TMP_InputField.ContentType.DecimalNumber},
                { "Upwareds", TMP_InputField.ContentType.DecimalNumber},
                { "Sideway", TMP_InputField.ContentType.DecimalNumber},
            },  Color.magenta, "Add force to gameobject", "Gameobjects", node: typeof(AddForceNode));
            CreateNodeBlueprint("ADDFORCEGLOBAL", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger,
                NodeBlueprint.ConnectionClass.Triggered,
                NodeBlueprint.ConnectionClass.ReciveGameObject,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
                { "X", TMP_InputField.ContentType.DecimalNumber},
                { "Y", TMP_InputField.ContentType.DecimalNumber},
                 { "Z", TMP_InputField.ContentType.DecimalNumber},

            },  Color.magenta, "Add global force to gameobject", "Gameobjects", node: typeof(AddGlobalForceNode));

            CreateNewNodeBlueprint("PLAYSOUND", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger,
                NodeBlueprint.ConnectionClass.Triggered,
                NodeBlueprint.ConnectionClass.ReciveGameObject,
            }, new List<NodeBlueprint.Field>
            {
                new NodeBlueprint.Field( "SoundRef", TMP_InputField.ContentType.Standard)
                {
                    fieldType = NodeBlueprint.Field.FieldType.Sound,
                },
                new NodeBlueprint.Field("Volume", TMP_InputField.ContentType.DecimalNumber),
                new NodeBlueprint.Field( "Pitch", TMP_InputField.ContentType.DecimalNumber),



            }, node: typeof(PlaySoundNode),  Color.magenta, "Play sound at gameobject position", "Gameobjects");

            CreateNewNodeBlueprint("GOTOUNIT", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger,
                NodeBlueprint.ConnectionClass.Triggered,
                NodeBlueprint.ConnectionClass.ReciveGameObject,
                NodeBlueprint.ConnectionClass.ReciveUnit,
            }, new List<NodeBlueprint.Field>()
            {
                new NodeBlueprint.Field()
                {
                    name = "IDKME",
                    isDropdown = true,
                    dropDowns = new string[]
                    {
                        "Both",
                        "Position only",
                        "Rotation only",
                    }
                },
                new NodeBlueprint.Field()
                {
                    name = "BodyPart",
                    isDropdown = true,
                    dropDowns = new string[]
                    {
                        "Head",
                        "Torso",
                        "Right Arm",
                        "Left Arm",
                        "Right Hand",
                        "Left Hand",
                        "Right Knee",
                        "Left Knee",
                        "Right Foot",
                        "Left Foot",
                    }
                }
            }, node: typeof(GoToUnitNode),  Color.magenta, "Teleport gameobject to unit", "Gameobjects");
            CreateNewNodeBlueprint("ROTATETOUNIT", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger,
                NodeBlueprint.ConnectionClass.Triggered,
                NodeBlueprint.ConnectionClass.ReciveGameObject,
                NodeBlueprint.ConnectionClass.ReciveUnit,
            }, new List<NodeBlueprint.Field>()
            {
                new NodeBlueprint.Field()
                {
                    name = "BodyPart",
                    isDropdown = true,
                    dropDowns = new string[]
                    {
                        "Head",
                        "Torso",
                        "Right Arm",
                        "Left Arm",
                        "Right Hand",
                        "Left Hand",
                        "Right Knee",
                        "Left Knee",
                        "Right Foot",
                        "Left Foot",
                    }
                }
            }, node: typeof(RotateTowardsNode),  Color.magenta, "Rotate gameobject towards unit", "Gameobjects");
            CreateNewNodeBlueprint("GETDISTANCE", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.ReciveGameObject,
                NodeBlueprint.ConnectionClass.ReciveUnit,
                NodeBlueprint.ConnectionClass.GiveVariable,
            }, new List<NodeBlueprint.Field>()
            {
            }, node: typeof(GetDistanceFrom),  Color.magenta, "Get distance from unit to gameobject", "Gameobjects");
            CreateNewNodeBlueprint("TRANSFORMPOS", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger,
                NodeBlueprint.ConnectionClass.Triggered,
                NodeBlueprint.ConnectionClass.ReciveGameObject,
            }, new List<NodeBlueprint.Field>()
            {
                new NodeBlueprint.Field ( "x", TMP_InputField.ContentType.DecimalNumber),
                new NodeBlueprint.Field ( "y", TMP_InputField.ContentType.DecimalNumber),
                new NodeBlueprint.Field ( "z", TMP_InputField.ContentType.DecimalNumber),
                new NodeBlueprint.Field()
                {
                    name = "TransformType",
                    isDropdown = true,
                    dropDowns = new string[]
                    {
                        "Global",
                        "Local",
                    }
                },
                new NodeBlueprint.Field()
                {
                    name = "TransformTypeOther",
                    isDropdown = true,
                    dropDowns = new string[]
                    {
                        "Add",
                        "Set",
                    }
                }
            }, node: typeof(TransformPositionNode),  Color.yellow, "Modify position of gameobj", "Animations");

            CreateNodeBlueprint("TRANSFORMSCALE", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger,
                NodeBlueprint.ConnectionClass.Triggered,
                NodeBlueprint.ConnectionClass.ReciveGameObject,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
                { "x", TMP_InputField.ContentType.DecimalNumber},
                { "y", TMP_InputField.ContentType.DecimalNumber},
                { "z", TMP_InputField.ContentType.DecimalNumber}
            },  Color.yellow, "Change scale of gameobj", "Animations", node: typeof(TransformScaleNode));
            CreateNewNodeBlueprint("TRANSFORMROT", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger,
                NodeBlueprint.ConnectionClass.Triggered,
                NodeBlueprint.ConnectionClass.ReciveGameObject,
            }, new List<NodeBlueprint.Field>()
            {
                new NodeBlueprint.Field ( "x", TMP_InputField.ContentType.DecimalNumber),
                new NodeBlueprint.Field ( "y", TMP_InputField.ContentType.DecimalNumber),
                new NodeBlueprint.Field ( "z", TMP_InputField.ContentType.DecimalNumber),
                new NodeBlueprint.Field()
                {
                    name = "TransformType",
                    isDropdown = true,
                    dropDowns = new string[]
                    {
                        "Global",
                        "Local",
                    }
                },
                new NodeBlueprint.Field()
                {
                    name = "TransformTypeOther",
                    isDropdown = true,
                    dropDowns = new string[]
                    {
                        "Add",
                        "Set",
                    }
                }
            }, node: typeof(TransformRotationNode),  Color.yellow, "Modify rotation of gameobj", "Animations");
            CreateNewNodeBlueprint("COLOR", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger,
                NodeBlueprint.ConnectionClass.Triggered,
                NodeBlueprint.ConnectionClass.ReciveGameObject,
            }, new List<NodeBlueprint.Field>()
            {
                new NodeBlueprint.Field("red", TMP_InputField.ContentType.DecimalNumber),
                new NodeBlueprint.Field("green", TMP_InputField.ContentType.DecimalNumber),
                new NodeBlueprint.Field("blue", TMP_InputField.ContentType.DecimalNumber),
                new NodeBlueprint.Field("alpha", TMP_InputField.ContentType.DecimalNumber),
                new NodeBlueprint.Field("index", TMP_InputField.ContentType.IntegerNumber),
                new NodeBlueprint.Field()
                {
                    name = "Color children",
                    isDropdown = true,
                    dropDowns = new string[]
                    {
                        "Color children",
                        "Don't",
                    }
                },
                new NodeBlueprint.Field()
                {
                    name = "SetOrMod",
                    isDropdown = true,
                    dropDowns = new string[]
                    {
                        "Set",
                        "Change",
                    }
                },
               

            }, node: typeof(ColorObjectNode),  Color.yellow, "Color object", "Gameobjects"); ;
            CreateNewNodeBlueprint("COLORHSV", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger,
                NodeBlueprint.ConnectionClass.Triggered,
                NodeBlueprint.ConnectionClass.ReciveGameObject,
            }, new List<NodeBlueprint.Field>()
            {
                new NodeBlueprint.Field("Hue", TMP_InputField.ContentType.DecimalNumber),
                new NodeBlueprint.Field("Saturation", TMP_InputField.ContentType.DecimalNumber),
                new NodeBlueprint.Field("Value", TMP_InputField.ContentType.DecimalNumber),
                new NodeBlueprint.Field("alpha", TMP_InputField.ContentType.DecimalNumber),
                new NodeBlueprint.Field("index", TMP_InputField.ContentType.IntegerNumber),
                new NodeBlueprint.Field()
                {
                    name = "Color children",
                    isDropdown = true,
                    dropDowns = new string[]
                    {
                        "Color children",
                        "Don't",
                    }
                },
                
            }, node: typeof(ColorObjectNodeHSV),  Color.yellow, "Tweak Color of object with HSV", "Gameobjects"); ;
            CreateNodeBlueprint("DESTROY", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger,
                NodeBlueprint.ConnectionClass.Triggered,
                NodeBlueprint.ConnectionClass.ReciveGameObject,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {

            },  Color.yellow, "Destroy", "Gameobjects", node: typeof(DestroyNode));
            CreateNodeBlueprint("CREATEFUNC", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
                {"Function name" , TMP_InputField.ContentType.Standard}
            },  Color.yellow, "Create function", "Functions", node: typeof(CreateFunctionNode));
            CreateNodeBlueprint("RUNFUNC", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Triggered,
                NodeBlueprint.ConnectionClass.Trigger,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
                {"Function name" , TMP_InputField.ContentType.Standard}
            },  Color.yellow, "Run function", "Functions", node: typeof(RunFunctionNode));
            CreateNodeBlueprint("ADDONHITACTION", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Triggered,
                NodeBlueprint.ConnectionClass.Trigger,
                NodeBlueprint.ConnectionClass.ReciveGameObject,
                NodeBlueprint.ConnectionClass.GiveGameObject,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
            },  Color.yellow, "Add projectile hit action ", "Projectiles", node: typeof(AddProjectileOnHitAction));
            CreateNodeBlueprint("ADDONCOLLISIONACTION", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Triggered,
                NodeBlueprint.ConnectionClass.Trigger,
                NodeBlueprint.ConnectionClass.ReciveGameObject,
                NodeBlueprint.ConnectionClass.GiveGameObject,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
            },  Color.yellow, "Add collision enter action ", "Gameobjects", node: typeof(AddOnCollisionAction));
            CreateNewNodeBlueprint("ADDCOMP", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger,
                NodeBlueprint.ConnectionClass.Triggered,
                NodeBlueprint.ConnectionClass.ReciveGameObject,
                NodeBlueprint.ConnectionClass.GiveComponent,
            }, new List<NodeBlueprint.Field>
            {
                new NodeBlueprint.Field( "Component name", TMP_InputField.ContentType.Standard)
                {
                    fieldType = NodeBlueprint.Field.FieldType.Component
                },
            }, node: typeof(AddComponentNode),  Color.yellow, "Add Component to gameObj", "Gameobjects");

            CreateNewNodeBlueprint("GETCOMP", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.ReciveGameObject,
                NodeBlueprint.ConnectionClass.GiveComponent,
            }, new List<NodeBlueprint.Field>
            {
                new NodeBlueprint.Field( "Component name", TMP_InputField.ContentType.Standard)
                {
                    fieldType = NodeBlueprint.Field.FieldType.Component
                },
                new NodeBlueprint.Field()
                {
                    name = "Max",
                    isDropdown = true,
                    dropDowns = new string[]
                    {
                        "First",
                        "All",
                    }
                },
                new NodeBlueprint.Field()
                {
                    name = "SearchChildren",
                    isDropdown = true,
                    dropDowns = new string[]
                    {
                        "Search for children also",
                        "Don't",
                    }
                },
            }, node: typeof(GetComponentNode),  Color.yellow, "Get Component from gameObj", "Gameobjects");
            CreateNewNodeBlueprint("SetField", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger,
                NodeBlueprint.ConnectionClass.Triggered,
                NodeBlueprint.ConnectionClass.ReciveAnything,
            }, new List<NodeBlueprint.Field>
            {
                new NodeBlueprint.Field( "Field name", TMP_InputField.ContentType.Standard),
                new NodeBlueprint.Field( "Value", TMP_InputField.ContentType.Standard),
            }, node: typeof(SetFieldNode),  Color.yellow, "Set Field for gameObj", "Gameobjects");
            CreateNodeBlueprint("GETFIELD", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.ReciveAnything,
                NodeBlueprint.ConnectionClass.GiveAnything,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
                { "Field name", TMP_InputField.ContentType.Standard},
            },  Color.yellow, "Get Field for gameObj", "Gameobjects", node: typeof(GetFieldNode));
            CreateNodeBlueprint("INVOKE", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger,
                NodeBlueprint.ConnectionClass.Triggered,
                NodeBlueprint.ConnectionClass.ReciveAnything ,
                NodeBlueprint.ConnectionClass.GiveAnything,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
                { "Method name", TMP_InputField.ContentType.Standard},

            },  Color.yellow, "Invoke Method From Component", "Gameobjects", node: typeof(InvokeMethodNode));
            CreateNodeBlueprint("DUPE", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger,
                NodeBlueprint.ConnectionClass.Triggered,
                NodeBlueprint.ConnectionClass.ReciveAnything,
                NodeBlueprint.ConnectionClass.GiveAnything,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {

            },  Color.yellow, "Duplicate object", "Gameobjects", node: typeof(Duplicate));
            CreateNewNodeBlueprint("SETACTIVE", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger,
                NodeBlueprint.ConnectionClass.Triggered,
                NodeBlueprint.ConnectionClass.ReciveGameObject,
            }, new List<NodeBlueprint.Field>()
            {
                new NodeBlueprint.Field()
                {
                    name = "banana",
                    isDropdown = true,
                    dropDowns = new string[]
                    {
                        "Toggle",
                        "False",
                        "True",
                    }
                }
            }, node: typeof(SetActive),  Color.yellow, "Set Active", "Gameobjects");
            // Variable


            CreateNodeBlueprint("VARCREATE", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.GiveVariable,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
                { "variable name", TMP_InputField.ContentType.Standard},

            },  Color.white, "Get variable", "Variables", node: typeof(CreateVariableNode));

            CreateNodeBlueprint("VARADD", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger,
                NodeBlueprint.ConnectionClass.Triggered,
                NodeBlueprint.ConnectionClass.ReciveVariable,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {

                { "value", TMP_InputField.ContentType.DecimalNumber},

            },  Color.white, "Change variable by", "Variables", node: typeof(ChangeVariableBy));
            CreateNodeBlueprint("VARSET", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger,
                NodeBlueprint.ConnectionClass.Triggered,
                NodeBlueprint.ConnectionClass.ReciveVariable,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
                { "value", TMP_InputField.ContentType.DecimalNumber},

            },  Color.white, "Set variable to", "Variables", node: typeof(SetVariableTo));
            CreateNewNodeBlueprint("VARIF", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Triggered,
                NodeBlueprint.ConnectionClass.ReciveVariable,
                NodeBlueprint.ConnectionClass.Trigger,
            }, new List<NodeBlueprint.Field>()
            {
                new NodeBlueprint.Field("value", TMP_InputField.ContentType.DecimalNumber),
                new NodeBlueprint.Field()
                {
                    name = "Operator",
                    isDropdown = true,
                    dropDowns = new string[]
                    {
                        "=",
                        ">",
                        "<",
                        "<=",
                        ">=",
                    }
                },

            }, node: typeof(IfVariableNode),  Color.white, "If variable", "Variables");
            CreateNewNodeBlueprint("RANDOM", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.GiveVariable,
            }, new List<NodeBlueprint.Field>
            {
                new NodeBlueprint.Field( "Min Value", TMP_InputField.ContentType.IntegerNumber),
                new NodeBlueprint.Field("Max Value", TMP_InputField.ContentType.IntegerNumber),
                new NodeBlueprint.Field()
                {
                    name = "TypeTarTarTart",
                    isDropdown = true,
                    dropDowns = new string[]
                    {
                        "Static",
                        "Dyanamic",
                    }
                }

            }, node: typeof(RandomNode),  Color.white, "Choose Random", "Variables");
            // Object variables
            CreateNodeBlueprint("OBJECTVARCREATE", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.GiveObjectVariable,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
                { "variable name", TMP_InputField.ContentType.Standard},

            },  new Color(.2f, .2f, .8f), "Get object variable", "Object Variables", node: typeof(CreateObjectVariableNode));
            CreateNodeBlueprint("OBJECTVARSET", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger,
                NodeBlueprint.ConnectionClass.Triggered,
                NodeBlueprint.ConnectionClass.ReciveObjectVariable,
                NodeBlueprint.ConnectionClass.ReciveAnything,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
            },  new Color(.2f, .2f, .8f), "Store object in object variable", "Object Variables", node: typeof(SetObjectVariableTo));
            CreateNodeBlueprint("OBJECTVARCLEAR", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger,
                NodeBlueprint.ConnectionClass.Triggered,
                NodeBlueprint.ConnectionClass.ReciveObjectVariable,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
            },  new Color(.2f, .2f, .8f), "Clear object variable", "Object Variables", node: typeof(ClearObjectVariable));
            CreateNodeBlueprint("OBJECTVARIABLEVALUE", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.ReciveObjectVariable,
                NodeBlueprint.ConnectionClass.GiveAnything,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
            },  new Color(.2f, .2f, .8f), "Get value of object variable", "Object Variables", node: typeof(GetValueOfObjectVariableNode));




            //Convert
            CreateNodeBlueprint("CONVERTOBJ", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.ReciveGameObject,
                NodeBlueprint.ConnectionClass.GiveAnything,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {

            },  Color.black, "Convert gameobject to anything", "Convert", node: typeof(ConvertObj));
            CreateNodeBlueprint("CONVERTUNIT", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.ReciveUnit,
                NodeBlueprint.ConnectionClass.GiveAnything,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {

            },  Color.black, "Convert unit to anything", "Convert", node: typeof(ConvertUnit));
            CreateNodeBlueprint("CONVERTCOMP", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.ReciveComponent,
                NodeBlueprint.ConnectionClass.GiveAnything,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {

            },  Color.black, "Convert component to anything", "Convert", node: typeof(ConvertComp));
            CreateNodeBlueprint("CONVERTOBJ2", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.ReciveAnything,
                NodeBlueprint.ConnectionClass.GiveGameObject,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {

            },  Color.black, "Convert anything to gameobject", "Convert", node: typeof(AnythingToObj));
            CreateNodeBlueprint("CONVERTUNIT2", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.ReciveAnything,
                NodeBlueprint.ConnectionClass.GiveUnit,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {

            },  Color.black, "Convert anything to unit", "Convert", node: typeof(AnythingToUnit));
            CreateNodeBlueprint("CONVERTCOMP2", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.ReciveAnything,
                NodeBlueprint.ConnectionClass.GiveComponent,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {

            }, Color.black, "Convert anything to component", "Convert", node: typeof(ConvertComp2));
            AbilityManager.Instance.Init();
            Code.commnet = "------------------Ability------------------------";
            yield return new WaitUntil(() => TABSSceneManager.IsInMainMenuScene());
            yield return new WaitForEndOfFrame(); ;

        }
        public static IEnumerator ForceShowModal(BundledAbilitesManager.BundledAbility bundledAbility)
        {
            if (AbilityCreator.abilites.ContainsKey(bundledAbility.abilityName))
                yield break;

            yield return new WaitUntil(() => (int)ServiceLocator.GetService<LoadingScreenHandler>().GetField("currentLoadingScreenFadeState") == 0);
            yield return MyModalPanel.ShowModal(bundledAbility);
        }
        
       
        void CreateNodeBlueprint(string key, List<NodeBlueprint.ConnectionClass> connectionsTypes, Dictionary<string, TMP_InputField.ContentType> fields, Color color, string name, string tab, Type node = null)
        {
            if (nodeDatabase.ContainsKey(key))
                key += "+";
            List<NodeBlueprint.Field> fields1 = new List<NodeBlueprint.Field>();
            var array = fields.ToArray();
            for (int i = 0; i < array.Length; i++)
            {
                fields1.Add(new NodeBlueprint.Field()
                {
                    contentType = array[i].Value,
                    name = array[i].Key,
                });
            }
            if (node != null)
            {
                var nodeBlueprint = new NodeBlueprint()
                {
                    nodeBarColor = color,
                    nodeKey = key,
                    nodeName = name,
                    nodeFields = fields1,
                    name = name,
                    nodeConnections = connectionsTypes,
                    nodeFunction = node,
                    tab = tab,
                };
                nodeDatabase.Add(key, nodeBlueprint);
                DontDestroyOnLoad(nodeDatabase[key]);
                return;
            }
            else
            {
                var NodeBlueprint = new NodeBlueprint()
                {
                    nodeBarColor = color,
                    nodeKey = key,
                    nodeName = name,
                    nodeFields = fields1,
                    name = name,
                    nodeConnections = connectionsTypes,
                    tab = tab,
                };
                nodeDatabase.Add(key, NodeBlueprint);
            }
        }
        void CreateNewNodeBlueprint(string key, List<NodeBlueprint.ConnectionClass> connectionsTypes, List<NodeBlueprint.Field> fields, Type node, Color color, string name, string tab, bool obselete = false)
        {
            if (nodeDatabase.ContainsKey(key))
                key += "+";
            var nodeBlueprint = new NodeBlueprint()
            {
                nodeBarColor = color,
                nodeKey = key,
                nodeName = name,
                nodeFields = fields,
                obselete = obselete,
                name = name,
                nodeConnections = connectionsTypes,
                nodeFunction = node,
                tab = tab,
            };
            nodeDatabase.Add(key, nodeBlueprint);
            DontDestroyOnLoad(nodeDatabase[key]);
            return;
        }
        private static void CleanUnitblueprint(UnitBlueprint unitBlueprint)
        {

            for (int i = 0; i < unitBlueprint.objectsToSpawnAsChildren.Length; i++)
            {
                try
                {
                    GameObject ability = unitBlueprint.objectsToSpawnAsChildren[i];
                    if (ability.GetComponent<NodeRunner>())
                    {
                        List<GameObject> gameObjects = unitBlueprint.objectsToSpawnAsChildren.ToList();
                        gameObjects.Remove(ability);
                        gameObjects.Add(abilites[ability.name]);
                        unitBlueprint.objectsToSpawnAsChildren = gameObjects.ToArray();
                    }
                }
                catch { }
            }
        }
        public static StreamedSceneManager sceneManager;
        public static Dictionary<string, NodeBlueprint> nodeDatabase = new Dictionary<string, NodeBlueprint>();
    }
}