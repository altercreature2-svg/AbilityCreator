using BepInEx;
using CASA.Tools;
using DM;
using HarmonyLib;
using IDK.AssetManaging;
using IDK.ExampleAbilites;
using IDK.Node_Related_Scripts.Migrater;
using IDK.Node_Related_Scripts.SavingStuff;
using IDK.NodeScripts;
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
namespace IDK
{
    [BepInPlugin("AAC", "Alter Ability Creator", "2.6.1")]
    public class AbilityCreator : CASA.Main.CASAMod
    {
        public static GameObject sliceEffect;
        public static List<DatabaseID> nodeIDS = new List<DatabaseID>();
        public static List<BundledAbilitesManager.BundledAbility> bundledAbilitiesQueue = new List<BundledAbilitesManager.BundledAbility>();
        public static string path = Path.Combine(GamePaths.DataPath + "/Abilty Creator");
        public static string Guide = "";
        public static VanillaAssetManager assetManager;
        public static IDK.Reapeter reapeter;
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

            if (Application.platform == RuntimePlatform.OSXPlayer)
            {
                path = GamePaths.PersistentDataPath + "/Abilty Creator";
                abilitespath = path + "/Abilites";
            }
            Harmony harmony = new Harmony("Alter.AbilityCreator");
            harmony.PatchAll();
            Code.commnet = "Dear Code Viewer (pervert), prepare your self for the most unoptomized, evil, straight up SHIT code you will ever see";
            Code.commnet = "Please understand that most of this was made like 2 years ago, I'm aware how bad it is and i hope to rewrite most of it someday";
            Code.commnet = "Today is not that day though";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            if (!Directory.Exists(abilitespath))
            {
                Directory.CreateDirectory(abilitespath);
            }
            UpdateSaveManager.Handle();
            string[] abilities = Directory.GetDirectories(abilitespath);
            for (int i = 0; i < abilities.Length; i++)
            {
                string[] abilityFiles = Directory.GetFiles(abilities[i]);
                if (Path.GetExtension(abilityFiles[i]) == ".ability")
                {
                    // Migrate
                    VirtualNodeScene savedNodeScene = NodeSceneMigrater.GetNewSavedNodeScene(Serialize.LoadJson<LegacySavedNodeScene>(File.ReadAllText(abilityFiles[i])));
                    // Move
                    if (!Directory.Exists(abilitespath + "/OldAbilites"))
                        Directory.CreateDirectory(abilitespath + "/OldAbilites");
                    File.Copy(abilityFiles[i], abilitespath + "/OldAbilites/" + Path.GetFileName(abilityFiles[i]));
                    File.Delete(abilityFiles[i]);
                    // Add new
                    string json = Serialize.SaveJson(savedNodeScene);
                    File.WriteAllText(abilityFiles[i], json);
                }
            }
            StartCoroutine(Call());
            
            reapeter = gameObject.AddComponent<Reapeter>();
            string[] files = Directory.GetFiles(abilitespath);
            for (int i = 0; i < files.Length; i++)
            {
                Debug.Log("File Extension! " + Path.GetExtension(files[i]));
                if (Path.GetExtension(files[i]) == ".newnodescene")
                {
                    LegacySavedNodeScene nodeScene = Serialize.LoadJson<LegacySavedNodeScene>(File.ReadAllText(files[i]));
                    if (!FixNodeSystem.IsValid(nodeScene))
                    {
                        continue;
                    }
                    string p = abilitespath + "/" + nodeScene.sceneName + "/";
                    if (!Directory.Exists(p))
                        Directory.CreateDirectory(p);
                    File.Copy(files[i], p + nodeScene.id + ".abilityx");
                    if (!Directory.Exists(abilitespath + "/OldAbilites"))
                        Directory.CreateDirectory(abilitespath + "/OldAbilites");
                    File.Copy(files[i], abilitespath + "/OldAbilites/" + Path.GetFileName(files[i]));
                    File.Delete(files[i]);
                }
            }

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

            }, NodeBlueprint.Type.Function, Color.grey, "When Battle Begins", "Triggers", node: typeof(WhenBattleBegins));
            CreateNodeBlueprint("UNIT_SPAWN", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {

            }, NodeBlueprint.Type.Function, Color.grey, "When unit spawns", "Triggers", node: typeof(WhenUnitSpawned));
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
            }, node: typeof(EndBattle), NodeBlueprint.Type.Function, Color.white, "End battle", "Misc");
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
            }, node: typeof(GetMaxUnitHealth), NodeBlueprint.Type.Function, Color.white, "Get max unit health", "Data");
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
            }, node: typeof(GetUnitHealth), NodeBlueprint.Type.Function, Color.white, "Get unit health", "Data");
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
            }, node: typeof(SetUnitHealth), NodeBlueprint.Type.Function, Color.white, "Set unit health", "Data");
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
            }, node: typeof(SetUnitDamage), NodeBlueprint.Type.Function, Color.white, "Set unit damage", "Data");
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
            }, node: typeof(SetUnitDamage), NodeBlueprint.Type.Function, Color.white, "Set unit cooldown", "Data");

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
            }, node: typeof(QuickBuff), NodeBlueprint.Type.Function, Color.white, "Quick buff", "Data");




            CreateNodeBlueprint("LOG", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Triggered,
                NodeBlueprint.ConnectionClass.Trigger,
                NodeBlueprint.ConnectionClass.ReciveVariable,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
            }, NodeBlueprint.Type.Function, Color.white, "Log variable", "Variables", node: typeof(LogVariable));
            CreateNodeBlueprint("LOGANY", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Triggered,
                NodeBlueprint.ConnectionClass.Trigger,
                NodeBlueprint.ConnectionClass.ReciveAnything,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
            }, NodeBlueprint.Type.Function, Color.white, "Log anything", "Gameobjects", node: typeof(LogAnything));


            CreateNewNodeBlueprint("ABILITYACTVIATE", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger
            }, new List<NodeBlueprint.Field>()
            {
                 new NodeBlueprint.Field("Delay", TMP_InputField.ContentType.DecimalNumber),
                 new NodeBlueprint.Field("Range", TMP_InputField.ContentType.DecimalNumber),
                 /*new NodeBlueprint.Field()
                 {
                     name = "StartCD",
                     isDropdown = true,
                     dropDowns = new string[]
                     {
                         "Start on cooldown",
                         "Don't",
                     }
                 }*/
            }, node: typeof(WhenAbilityTriggered), NodeBlueprint.Type.Input, Color.grey, "When ability triggered...", "Triggers"); ;


            CreateNewNodeBlueprint("UNITWASATTACKED", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger
            }, new List<NodeBlueprint.Field>()
            {
                 new NodeBlueprint.Field("Delay", TMP_InputField.ContentType.DecimalNumber),
                 new NodeBlueprint.Field("Range", TMP_InputField.ContentType.DecimalNumber),
            }, node: typeof(UnitWasAttacked), NodeBlueprint.Type.Input, Color.grey, "When unit gets attacked...", "Triggers");
            CreateNodeBlueprint("INTERVAL", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
                { "Interval", TMP_InputField.ContentType.DecimalNumber },
            }, NodeBlueprint.Type.Input, Color.grey, "Do Every...", "Triggers", node: typeof(DoEveryNode));
            CreateNodeBlueprint("WHENUNITDIE", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
            }, NodeBlueprint.Type.Input, Color.grey, "When Unit Dies...", "Triggers", node: typeof(WhenUnitDies));
            /*CreateNodeBlueprint("WHENUNITDAMAGES", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.Trigger,
                NodeBlueprint.ConnectionType.GiveUnit
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
            }, NodeBlueprint.Type.Input, Color.grey, "When unit damages...", "Triggers", node: typeof(WhenUnitDamages));*/
            CreateNodeBlueprint("WHENUNITDAMAGED", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger,
                NodeBlueprint.ConnectionClass.GiveUnit
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
            }, NodeBlueprint.Type.Input, Color.grey, "When unit damaged...", "Triggers", node: typeof(WhenUnitDamaged))
            ;
            CreateNodeBlueprint("WHENUNITATTACKS", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger,
                NodeBlueprint.ConnectionClass.GiveUnit,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
            }, NodeBlueprint.Type.Input, Color.grey, "When unit attacks...", "Triggers", node: typeof(WhenUnitAttacks));
            CreateNewNodeBlueprint("PROJECTILERANGE", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger,
                NodeBlueprint.ConnectionClass.GiveGameObject,
            }, new List<NodeBlueprint.Field>()
            {
                new NodeBlueprint.Field("Range",TMP_InputField.ContentType.DecimalNumber),
                new NodeBlueprint.Field("Block Power",TMP_InputField.ContentType.DecimalNumber),
            }, node: typeof(WhenProjectileEntersRange), NodeBlueprint.Type.Input, Color.grey, "When projectile in range...", "Triggers");
            CreateNodeBlueprint("IS_BATTLE", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger,
                NodeBlueprint.ConnectionClass.Triggered,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {

            }, NodeBlueprint.Type.Function, Color.grey, "Run if in battle state", "Control", node: typeof(IsBattleState));
            CreateNodeBlueprint("IS_DEAD", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger,
                NodeBlueprint.ConnectionClass.Triggered,
                NodeBlueprint.ConnectionClass.ReciveUnit,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {

            }, NodeBlueprint.Type.Function, Color.grey, "Run if unit is dead", "Control", node: typeof(IsDead));
            CreateNodeBlueprint("IS_NOTDEAD", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger,
                NodeBlueprint.ConnectionClass.Triggered,
                NodeBlueprint.ConnectionClass.ReciveUnit,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {

            }, NodeBlueprint.Type.Function, Color.grey, "Run if unit is alive", "Control", node: typeof(IsAlive));
            CreateNodeBlueprint("PAUSE", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger,
                NodeBlueprint.ConnectionClass.Triggered,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
                { "Seconds", TMP_InputField.ContentType.DecimalNumber },

            }, NodeBlueprint.Type.Function, Color.grey, "Do after pause", "Control", node: typeof(PauseNode));
            CreateNodeBlueprint("REAPET", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger,
                NodeBlueprint.ConnectionClass.Triggered,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
                { "Times", TMP_InputField.ContentType.IntegerNumber },
                { "Delay", TMP_InputField.ContentType.DecimalNumber},
            }, NodeBlueprint.Type.Input, Color.grey, "Repeat", "Control", node: typeof(ReapetNode));
            CreateNodeBlueprint("REAPETVAR", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger,
                NodeBlueprint.ConnectionClass.Triggered,
                NodeBlueprint.ConnectionClass.ReciveVariable,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
                { "Delay", TMP_InputField.ContentType.DecimalNumber},
            }, NodeBlueprint.Type.Input, Color.grey, "Repeat for variable", "Control", node: typeof(ReapetVarNode));
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
            }, node: typeof(ForEachNode), NodeBlueprint.Type.Input, Color.grey, "Repeat for each value", "Control");
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
            }, node: typeof(FilterNode), NodeBlueprint.Type.Input, Color.grey, "Filter values", "Control");



            CreateNodeBlueprint("NEWOBJ", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.GiveGameObject
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
                { "Object name", TMP_InputField.ContentType.Standard },
            }, NodeBlueprint.Type.Function, Color.green, "Create gameObject", "Gameobjects", node: typeof(CreateGameobject));



            CreateNodeBlueprint("ENEMY", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.GiveUnit
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
            }, NodeBlueprint.Type.Function, Color.green, "Enemy unit", "Units", node: typeof(EnemyNode));
            CreateNodeBlueprint("SELF", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.GiveUnit
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
            }, NodeBlueprint.Type.Function, Color.green, "Self unit", "Units", node: typeof(SelfNode));
            CreateNodeBlueprint("EXPSENSIVETEAMMATE", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.ReciveUnit,
                NodeBlueprint.ConnectionClass.GiveUnit,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
            }, NodeBlueprint.Type.Function, Color.green, "Most expensive teammate of unit", "Units", node: typeof(MostExspensiveTeamMateUnit));
            CreateNodeBlueprint("EXPSENSIVEENEMY", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.ReciveUnit,
                NodeBlueprint.ConnectionClass.GiveUnit,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
            }, NodeBlueprint.Type.Function, Color.green, "Most expensive enemy of unit", "Units", node: typeof(MostExspensiveEnemy));
            CreateNodeBlueprint("ALLTEAM", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.GiveUnit
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
            }, NodeBlueprint.Type.Function, Color.green, "All units of same team", "Units", node: typeof(AllTeam));
            CreateNodeBlueprint("ALLOTHERTEAM", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.GiveUnit
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
            }, NodeBlueprint.Type.Function, Color.green, "All units of other team", "Units", node: typeof(AllOtherTeam));


            CreateNodeBlueprint("GETUNIT", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.GiveUnit,
                NodeBlueprint.ConnectionClass.ReciveGameObject
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
            }, NodeBlueprint.Type.Function, Color.green, "Get unit from gameobject", "Unit Tools", node: typeof(GetUnitFromGameobject));
            CreateNodeBlueprint("TEAMMATE", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.GiveUnit,
                NodeBlueprint.ConnectionClass.ReciveUnit
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
            }, NodeBlueprint.Type.Function, Color.green, "Closest team mate of unit", "Unit Tools", node: typeof(ClosestTeamMateUnit));
            CreateNodeBlueprint("FREAZE", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger,
                NodeBlueprint.ConnectionClass.Triggered,
                NodeBlueprint.ConnectionClass.ReciveUnit
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
                { "Length", TMP_InputField.ContentType.DecimalNumber}
            }, NodeBlueprint.Type.Function, Color.magenta, "freeze unit for", "Unit Tools", node: typeof(FreezeNode));
            CreateNodeBlueprint("DONTWALK", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger,
                NodeBlueprint.ConnectionClass.Triggered,
                NodeBlueprint.ConnectionClass.ReciveUnit
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
                { "Length", TMP_InputField.ContentType.DecimalNumber}
            }, NodeBlueprint.Type.Function, Color.magenta, "make unit not walk for", "Unit Tools", node: typeof(DontWalkNode));
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
            }, node: typeof(HoldPostionNode), NodeBlueprint.Type.Function, Color.magenta, "hold body part positions for", "Unit Tools");

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
            }, typeof(GetGameobject), NodeBlueprint.Type.Function, Color.green, "Get body part of unit", "Unit Tools");
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
            }, typeof(GetClothes), NodeBlueprint.Type.Function, Color.green, "Get clothes of unit", "Unit Tools");
            CreateNewNodeBlueprint("ABILITIES", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.ReciveUnit,
                NodeBlueprint.ConnectionClass.GiveGameObject
            }
            , new List<NodeBlueprint.Field>
            {
            }, typeof(GetAbilites), NodeBlueprint.Type.Function, Color.green, "Get abilites of unit", "Unit Tools");
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
            }, typeof(GetWeapon), NodeBlueprint.Type.Function, Color.green, "Get weapon of unit", "Weapons");
            CreateNewNodeBlueprint("SETFIST", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger,
                NodeBlueprint.ConnectionClass.Triggered,
                NodeBlueprint.ConnectionClass.ReciveUnit,
            }
            , new List<NodeBlueprint.Field>
            {
            }, typeof(SetFistNode), NodeBlueprint.Type.Function, Color.green, "Make unit use fists", "Weapons");
            CreateNewNodeBlueprint("GETEYE", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.ReciveUnit,
                NodeBlueprint.ConnectionClass.GiveGameObject
            }
            , new List<NodeBlueprint.Field>
            {
            }, typeof(GetEye), NodeBlueprint.Type.Function, Color.green, "Get eyes of unit", "Unit Tools");

            CreateNewNodeBlueprint("GETWEAPONGLOBAL", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.GiveGameObject
            }, new List<NodeBlueprint.Field>
            {
                 new NodeBlueprint.Field("Weapon name", TMP_InputField.ContentType.Standard)
                 {
                     fieldType = NodeBlueprint.Field.FieldType.Weapon
                 }
             }, node: typeof(GetWeaponOfName), NodeBlueprint.Type.Function, Color.magenta, "Get weapon of name", "Weapons");
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
            }, node: typeof(Deal_Damage), NodeBlueprint.Type.Function, Color.red, "Deal Damage", "Unit Tools");
            CreateNodeBlueprint("DIE", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger,
                NodeBlueprint.ConnectionClass.Triggered,
                NodeBlueprint.ConnectionClass.ReciveUnit,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
            }, NodeBlueprint.Type.Function, Color.red, "Kill", "Unit Tools", node: typeof(DieNode));
            CreateNodeBlueprint("REVIVE", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger,
                NodeBlueprint.ConnectionClass.Triggered,
                NodeBlueprint.ConnectionClass.ReciveUnit,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
            }, NodeBlueprint.Type.Function, Color.red, "Revive unit", "Unit Tools", node: typeof(ReDie));
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
            }, node: typeof(SetWillBeRevived), NodeBlueprint.Type.Function, Color.red, "Set will be revived", "Unit Tools"); ;
            CreateNodeBlueprint("SWAPTEAM", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger,
                NodeBlueprint.ConnectionClass.Triggered,
                NodeBlueprint.ConnectionClass.ReciveUnit,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
            }, NodeBlueprint.Type.Function, Color.red, "Swap unit's team", "Unit Tools", node: typeof(SwapTeam));
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

            }, node: typeof(SpawnProjectileNode), NodeBlueprint.Type.Function, Color.red, "Spawn projectile", "Projectiles");
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

            //}, node: typeof(SpawnProjectileAimedNode), NodeBlueprint.Type.Function, Color.red, "Spawn projectile to target", "Projectiles");
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

            }, node: typeof(ParryProjectile), NodeBlueprint.Type.Function, Color.red, "Parry projectile", "Projectiles");
            CreateNewNodeBlueprint("PLAYSLICE", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger,
                NodeBlueprint.ConnectionClass.Triggered,
                NodeBlueprint.ConnectionClass.ReciveGameObject,
                NodeBlueprint.ConnectionClass.GiveGameObject,
            }, new List<NodeBlueprint.Field>()
            {

            }, node: typeof(PlaySlice), NodeBlueprint.Type.Function, Color.red, "Play slice effect", "Projectiles");
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

            }, typeof(SpawnExplosionNode), NodeBlueprint.Type.Function, Color.red, "Summon Explosion", "Misc");
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
            }, typeof(SpawnParticleNode), NodeBlueprint.Type.Function, Color.red, "Spawn particle", "Misc");

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
            }, node: typeof(AddEffectNode), NodeBlueprint.Type.Function, Color.red, "Add Effect to unit", "Unit Tools");

            CreateNodeBlueprint("STUNWEAPONS", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger,
                NodeBlueprint.ConnectionClass.Triggered,
                NodeBlueprint.ConnectionClass.ReciveUnit,
            }
            , new Dictionary<string, TMP_InputField.ContentType>
            {
                { "Time", TMP_InputField.ContentType.DecimalNumber},


            }, NodeBlueprint.Type.Function, Color.red, "Stun weapons for unit", "Weapons", node: typeof(StunWeaponsNode));

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
            }, node: typeof(LetGoNode), NodeBlueprint.Type.Function, Color.red, "Let go of stuff at hand", "Unit Tools");
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
            }, node: typeof(SetWeaponNode), NodeBlueprint.Type.Function, Color.red, "Set unit's weapon", "Weapons");
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
            }, typeof(SpawnUnitNode), NodeBlueprint.Type.Function, Color.red, "Spawn unit at gameobject", "Units");



            CreateNodeBlueprint("ALLABS", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger,
                NodeBlueprint.ConnectionClass.Triggered,
                NodeBlueprint.ConnectionClass.ReciveUnit,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
            }, NodeBlueprint.Type.Function, Color.magenta, "Play all abilites", "Unit Tools", node: typeof(PlayAllAbilites));
            CreateNodeBlueprint("IMMUNE", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger,
                NodeBlueprint.ConnectionClass.Triggered,
                NodeBlueprint.ConnectionClass.ReciveUnit,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
                { "Length", TMP_InputField.ContentType.DecimalNumber },
            }, NodeBlueprint.Type.Function, Color.magenta, "Make Immune for", "Unit Tools", node: typeof(MakeUnitImmune));

            CreateNodeBlueprint("TOGGLEPROPS", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger,
                NodeBlueprint.ConnectionClass.Triggered,
                NodeBlueprint.ConnectionClass.ReciveUnit,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
                 { "From", TMP_InputField.ContentType.IntegerNumber},
                { "To", TMP_InputField.ContentType.IntegerNumber},
            }, NodeBlueprint.Type.Function, Color.magenta, "Toggle Props at indexs", "Unit Tools", node: typeof(ToggleProps));
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
            }, node: typeof(AddProps), NodeBlueprint.Type.Function, Color.magenta, "Add clothes/abilites to unit (OBSELETE DONT USE)", "Unit Tools",true);
            CreateNewNodeBlueprint("ADDABILITY", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger,
                NodeBlueprint.ConnectionClass.Triggered,
                NodeBlueprint.ConnectionClass.ReciveUnit,
                NodeBlueprint.ConnectionClass.ReciveGameObject,
                NodeBlueprint.ConnectionClass.GiveGameObject,
            }, new List<NodeBlueprint.Field>
            {
            }, node: typeof(AddAbility), NodeBlueprint.Type.Function, Color.magenta, "Add ability to unit", "Abilites");
            CreateNewNodeBlueprint("GETABILITYGLOBAL", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.GiveGameObject
            }, new List<NodeBlueprint.Field>
            {
                 new NodeBlueprint.Field("Ability name", TMP_InputField.ContentType.Standard)
                 {
                     fieldType = NodeBlueprint.Field.FieldType.Ability
                 }
             }, node: typeof(GetAbilityOfName), NodeBlueprint.Type.Function, Color.magenta, "Get ability of name", "Abilites");
            CreateNewNodeBlueprint("ADDCLOTHES", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger,
                NodeBlueprint.ConnectionClass.Triggered,
                NodeBlueprint.ConnectionClass.ReciveUnit,
                NodeBlueprint.ConnectionClass.ReciveGameObject,
                NodeBlueprint.ConnectionClass.GiveGameObject,
            }, new List<NodeBlueprint.Field>
            {
            }, node: typeof(AddCloth), NodeBlueprint.Type.Function, Color.magenta, "Add clothes to unit", "Clothes");
            CreateNewNodeBlueprint("GETCLOTHESGLOBAL", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.GiveGameObject
            }, new List<NodeBlueprint.Field>
            {
                 new NodeBlueprint.Field("Clothing name", TMP_InputField.ContentType.Standard)
                 {
                     fieldType = NodeBlueprint.Field.FieldType.Clothing
                 }
             }, node: typeof(GetClothesOfName), NodeBlueprint.Type.Function, Color.magenta, "Get clothing of name", "Clothes");
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
            }, NodeBlueprint.Type.Function, Color.magenta, "Add force to gameobject", "Gameobjects", node: typeof(AddForceNode));
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

            }, NodeBlueprint.Type.Function, Color.magenta, "Add global force to gameobject", "Gameobjects", node: typeof(AddGlobalForceNode));

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



            }, node: typeof(PlaySoundNode), NodeBlueprint.Type.Function, Color.magenta, "Play sound at gameobject position", "Gameobjects");

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
            }, node: typeof(GoToUnitNode), NodeBlueprint.Type.Function, Color.magenta, "Teleport gameobject to unit", "Gameobjects");
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
            }, node: typeof(RotateTowardsNode), NodeBlueprint.Type.Function, Color.magenta, "Rotate gameobject towards unit", "Gameobjects");
            CreateNewNodeBlueprint("GETDISTANCE", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.ReciveGameObject,
                NodeBlueprint.ConnectionClass.ReciveUnit,
                NodeBlueprint.ConnectionClass.GiveVariable,
            }, new List<NodeBlueprint.Field>()
            {
            }, node: typeof(GetDistanceFrom), NodeBlueprint.Type.Function, Color.magenta, "Get distance from unit to gameobject", "Gameobjects");
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
            }, node: typeof(TransformPositionNode), NodeBlueprint.Type.Function, Color.yellow, "Modify position of gameobj", "Animations");

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
            }, NodeBlueprint.Type.Function, Color.yellow, "Change scale of gameobj", "Animations", node: typeof(TransformScaleNode));
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
            }, node: typeof(TransformRotationNode), NodeBlueprint.Type.Function, Color.yellow, "Modify rotation of gameobj", "Animations");
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
               

            }, node: typeof(ColorObjectNode), NodeBlueprint.Type.Function, Color.yellow, "Color object", "Gameobjects"); ;
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
                
            }, node: typeof(ColorObjectNodeHSV), NodeBlueprint.Type.Function, Color.yellow, "Tweak Color of object with HSV", "Gameobjects"); ;
            CreateNodeBlueprint("DESTROY", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger,
                NodeBlueprint.ConnectionClass.Triggered,
                NodeBlueprint.ConnectionClass.ReciveGameObject,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {

            }, NodeBlueprint.Type.Function, Color.yellow, "Destroy", "Gameobjects", node: typeof(DestroyNode));
            CreateNodeBlueprint("CREATEFUNC", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
                {"Function name" , TMP_InputField.ContentType.Standard}
            }, NodeBlueprint.Type.Function, Color.yellow, "Create function", "Functions", node: typeof(CreateFunctionNode));
            CreateNodeBlueprint("RUNFUNC", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Triggered,
                NodeBlueprint.ConnectionClass.Trigger,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
                {"Function name" , TMP_InputField.ContentType.Standard}
            }, NodeBlueprint.Type.Function, Color.yellow, "Run function", "Functions", node: typeof(RunFunctionNode));
            CreateNodeBlueprint("ADDONHITACTION", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Triggered,
                NodeBlueprint.ConnectionClass.Trigger,
                NodeBlueprint.ConnectionClass.ReciveGameObject,
                NodeBlueprint.ConnectionClass.GiveGameObject,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
            }, NodeBlueprint.Type.Function, Color.yellow, "Add projectile hit action ", "Projectiles", node: typeof(AddProjectileOnHitAction));
            CreateNodeBlueprint("ADDONCOLLISIONACTION", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Triggered,
                NodeBlueprint.ConnectionClass.Trigger,
                NodeBlueprint.ConnectionClass.ReciveGameObject,
                NodeBlueprint.ConnectionClass.GiveGameObject,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
            }, NodeBlueprint.Type.Function, Color.yellow, "Add collision enter action ", "Gameobjects", node: typeof(AddOnCollisionAction));
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
            }, node: typeof(AddComponentNode), NodeBlueprint.Type.Function, Color.yellow, "Add Component to gameObj", "Gameobjects");

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
            }, node: typeof(GetComponentNode), NodeBlueprint.Type.Function, Color.yellow, "Get Component from gameObj", "Gameobjects");
            CreateNewNodeBlueprint("SetField", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger,
                NodeBlueprint.ConnectionClass.Triggered,
                NodeBlueprint.ConnectionClass.ReciveAnything,
            }, new List<NodeBlueprint.Field>
            {
                new NodeBlueprint.Field( "Field name", TMP_InputField.ContentType.Standard),
                new NodeBlueprint.Field( "Value", TMP_InputField.ContentType.Standard),
            }, node: typeof(SetFieldNode), NodeBlueprint.Type.Function, Color.yellow, "Set Field for gameObj", "Gameobjects");
            CreateNodeBlueprint("GETFIELD", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.ReciveAnything,
                NodeBlueprint.ConnectionClass.GiveAnything,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
                { "Field name", TMP_InputField.ContentType.Standard},
            }, NodeBlueprint.Type.Function, Color.yellow, "Get Field for gameObj", "Gameobjects", node: typeof(GetFieldNode));
            CreateNodeBlueprint("INVOKE", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger,
                NodeBlueprint.ConnectionClass.Triggered,
                NodeBlueprint.ConnectionClass.ReciveAnything ,
                NodeBlueprint.ConnectionClass.GiveAnything,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
                { "Method name", TMP_InputField.ContentType.Standard},

            }, NodeBlueprint.Type.Function, Color.yellow, "Invoke Method From Component", "Gameobjects", node: typeof(InvokeMethodNode));
            CreateNodeBlueprint("DUPE", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger,
                NodeBlueprint.ConnectionClass.Triggered,
                NodeBlueprint.ConnectionClass.ReciveAnything,
                NodeBlueprint.ConnectionClass.GiveAnything,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {

            }, NodeBlueprint.Type.Function, Color.yellow, "Duplicate object", "Gameobjects", node: typeof(Duplicate));
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
            }, node: typeof(SetActive), NodeBlueprint.Type.Function, Color.yellow, "Set Active", "Gameobjects");
            // Variable


            CreateNodeBlueprint("VARCREATE", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.GiveVariable,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
                { "variable name", TMP_InputField.ContentType.Standard},

            }, NodeBlueprint.Type.Function, Color.white, "Get variable", "Variables", node: typeof(CreateVariableNode));

            CreateNodeBlueprint("VARADD", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger,
                NodeBlueprint.ConnectionClass.Triggered,
                NodeBlueprint.ConnectionClass.ReciveVariable,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {

                { "value", TMP_InputField.ContentType.DecimalNumber},

            }, NodeBlueprint.Type.Function, Color.white, "Change variable by", "Variables", node: typeof(ChangeVariableBy));
            CreateNodeBlueprint("VARSET", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger,
                NodeBlueprint.ConnectionClass.Triggered,
                NodeBlueprint.ConnectionClass.ReciveVariable,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
                { "value", TMP_InputField.ContentType.DecimalNumber},

            }, NodeBlueprint.Type.Function, Color.white, "Set variable to", "Variables", node: typeof(SetVariableTo));
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

            }, node: typeof(IfVariableNode), NodeBlueprint.Type.Function, Color.white, "If variable", "Variables");
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

            }, node: typeof(RandomNode), NodeBlueprint.Type.Function, Color.white, "Choose Random", "Variables");
            // Object variables
            CreateNodeBlueprint("OBJECTVARCREATE", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.GiveObjectVariable,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
                { "variable name", TMP_InputField.ContentType.Standard},

            }, NodeBlueprint.Type.Function, new Color(.2f, .2f, .8f), "Get object variable", "Object Variables", node: typeof(CreateObjectVariableNode));
            CreateNodeBlueprint("OBJECTVARSET", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger,
                NodeBlueprint.ConnectionClass.Triggered,
                NodeBlueprint.ConnectionClass.ReciveObjectVariable,
                NodeBlueprint.ConnectionClass.ReciveAnything,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
            }, NodeBlueprint.Type.Function, new Color(.2f, .2f, .8f), "Store object in object variable", "Object Variables", node: typeof(SetObjectVariableTo));
            CreateNodeBlueprint("OBJECTVARCLEAR", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.Trigger,
                NodeBlueprint.ConnectionClass.Triggered,
                NodeBlueprint.ConnectionClass.ReciveObjectVariable,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
            }, NodeBlueprint.Type.Function, new Color(.2f, .2f, .8f), "Clear object variable", "Object Variables", node: typeof(ClearObjectVariable));
            CreateNodeBlueprint("OBJECTVARIABLEVALUE", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.ReciveObjectVariable,
                NodeBlueprint.ConnectionClass.GiveAnything,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
            }, NodeBlueprint.Type.Function, new Color(.2f, .2f, .8f), "Get value of object variable", "Object Variables", node: typeof(GetValueOfObjectVariableNode));




            //Convert
            CreateNodeBlueprint("CONVERTOBJ", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.ReciveGameObject,
                NodeBlueprint.ConnectionClass.GiveAnything,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {

            }, NodeBlueprint.Type.Function, Color.black, "Convert gameobject to anything", "Convert", node: typeof(ConvertObj));
            CreateNodeBlueprint("CONVERTUNIT", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.ReciveUnit,
                NodeBlueprint.ConnectionClass.GiveAnything,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {

            }, NodeBlueprint.Type.Function, Color.black, "Convert unit to anything", "Convert", node: typeof(ConvertUnit));
            CreateNodeBlueprint("CONVERTCOMP", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.ReciveComponent,
                NodeBlueprint.ConnectionClass.GiveAnything,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {

            }, NodeBlueprint.Type.Function, Color.black, "Convert component to anything", "Convert", node: typeof(ConvertComp));
            CreateNodeBlueprint("CONVERTOBJ2", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.ReciveAnything,
                NodeBlueprint.ConnectionClass.GiveGameObject,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {

            }, NodeBlueprint.Type.Function, Color.black, "Convert anything to gameobject", "Convert", node: typeof(AnythingToObj));
            CreateNodeBlueprint("CONVERTUNIT2", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.ReciveAnything,
                NodeBlueprint.ConnectionClass.GiveUnit,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {

            }, NodeBlueprint.Type.Function, Color.black, "Convert anything to unit", "Convert", node: typeof(AnythingToUnit));
            CreateNodeBlueprint("CONVERTCOMP2", new List<NodeBlueprint.ConnectionClass>()
            {
                NodeBlueprint.ConnectionClass.ReciveAnything,
                NodeBlueprint.ConnectionClass.GiveComponent,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {

            }, NodeBlueprint.Type.Function, Color.black, "Convert anything to component", "Convert", node: typeof(ConvertComp2));

            string[] directories = Directory.GetDirectories(abilitespath);
            for (int i = 0; i < directories.Length; i++)
            {
                string[] files = Directory.GetFiles(directories[i]);
                for (int i2 = 0; i2 < files.Length; i2++)
                {
                    string path = files[i2];
                    if (System.IO.Path.GetExtension(path) != ".ability")
                    {
                        continue;
                    }
                    var n = File (File.ReadAllText(path));
                    AddAbility(n.SavedNodeSceneToNodeScene());
                }

            }


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
        public static GameObject CreateAbility(VirtualNodeScene nodeScene)
        {
            if (!nodeScenes.Contains(nodeScene))
                nodeScenes.Add(nodeScene);
            GameObject abilityObject = new GameObject(nodeScene.abilityName);
            abilityObject.hideFlags = HideFlags.HideAndDontSave;
            SpecialAbility specialAbility = abilityObject.AddComponent<SpecialAbility>();
            specialAbility.SetField("m_entity", new Landfall.TABS.DatabaseEntity(Landfall.TABS.Workshop.WorkshopContentType.Any)
            {
                GUID = new Landfall.TABS.DatabaseID()
                {
                    m_ID = nodeScene.abilityID,
                    m_modID = -2,

                },
                Name = nodeScene.abilityName,
            });
            NodeRunner nodeRunner = abilityObject.AddComponent<NodeRunner>();
            abilityObject.AddComponent<NodeRunnerFixer>();
            abilityObject.AddComponent<DodgeMove>();
            abilityObject.AddComponent<ConditionalEvent>();

            GoToBodyPart goToBodyPart = abilityObject.AddComponent<GoToBodyPart>();
            goToBodyPart.targetPart = GoToBodyPart.TargetPart.Torso;
            abilityObject.AddComponent<OnlyRunWhenAddedToUnit>();
            specialAbility.Entity.SetSpriteIcon(GetSprite(nodeScene));
            nodeRunner.nodeScene = nodeScene;
            return abilityObject;
        }
        public static GameObject AddAbility(VirtualNodeScene nodeScene)
        {
            try
            {
                GameObject abilityObj = CreateAbility(nodeScene);

                SpecialAbility specialAbility = abilityObj.GetComponent<SpecialAbility>();
                nodeIDS.Add(specialAbility.Entity.GUID);

                LandfallContentDatabase database = ContentDatabase.Instance().LandfallContentDatabase;
                AssetLoader assetLoader = ContentDatabase.Instance().AssetLoader;

                Dictionary<DatabaseID, UnityEngine.Object> streamableAssets = assetLoader.GetField<Dictionary<DatabaseID, UnityEngine.Object>>("m_nonStreamableAssets");
                Dictionary<DatabaseID, UnityEngine.GameObject> combatMoves = database.GetField<Dictionary<DatabaseID, UnityEngine.GameObject>>("m_combatMoves");
                if (!streamableAssets.ContainsKey(specialAbility.Entity.GUID))
                    streamableAssets.Add(specialAbility.Entity.GUID, abilityObj);
                else
                    streamableAssets[specialAbility.Entity.GUID] = abilityObj;
                if (!combatMoves.ContainsKey(specialAbility.Entity.GUID))
                    combatMoves.Add(specialAbility.Entity.GUID, abilityObj);
                else
                    combatMoves[specialAbility.Entity.GUID] = abilityObj;
                database.SetField("m_combatMoves", combatMoves);
                assetLoader.SetField("m_nonStreamableAssets", streamableAssets);
                specialAbility.tags.Add(new CharacterItem.Tag(CharacterItem.TagType.Faction, "Custom Abilites"));
                if (abilites.ContainsKey(abilityObj.GetComponent<NodeRunner>().nodeScene.abilityName))
                    abilites.Remove(abilityObj.GetComponent<NodeRunner>().nodeScene.abilityName);
                abilites.Add(abilityObj.GetComponent<NodeRunner>().nodeScene.abilityName, abilityObj);
                return abilityObj;
            }
            catch { return null; }
        }
        void CreateNodeBlueprint(string key, List<NodeBlueprint.ConnectionClass> connectionsTypes, Dictionary<string, TMP_InputField.ContentType> fields, NodeBlueprint.Type type, Color color, string name, string Tab, bool longfield = false, Type node = null)
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
                    color = color,
                    key = key,
                    Name = name,
                    type = type,
                    fields = fields1,
                    LongField = longfield,
                    name = name,
                    connections = connectionsTypes,
                    nodeFunction = node,
                    tab = Tab,
                };
                nodeDatabase.Add(key, nodeBlueprint);
                DontDestroyOnLoad(nodeDatabase[key]);
                return;
            }
            else
            {
                var NodeBlueprint = new NodeBlueprint()
                {
                    color = color,
                    key = key,
                    Name = name,
                    type = type,
                    fields = fields1,
                    LongField = longfield,
                    name = name,
                    connections = connectionsTypes,
                    tab = Tab,
                };
                nodeDatabase.Add(key, NodeBlueprint);
            }
        }
        void CreateNewNodeBlueprint(string key, List<NodeBlueprint.ConnectionClass> connectionsTypes, List<NodeBlueprint.Field> fields, Type node, NodeBlueprint.Type type, Color color, string name, string tab, bool obselete = false)
        {
            if (nodeDatabase.ContainsKey(key))
                key += "+";
            var nodeBlueprint = new NodeBlueprint()
            {
                color = color,
                key = key,
                Name = name,
                type = type,
                fields = fields,
                LongField = false,
                obselete = obselete,
                name = name,
                connections = connectionsTypes,
                nodeFunction = node,
                tab = tab,
            };
            nodeDatabase.Add(key, nodeBlueprint);
            DontDestroyOnLoad(nodeDatabase[key]);
            return;
        }
        public static void Reload()
        {
            MReload();
        }
        public static void MReload()
        {
            
            
            ContentDatabase contentDatabase = ContentDatabase.Instance();
            // Landfall Content database
            LandfallContentDatabase landfallContentDatabase = contentDatabase.LandfallContentDatabase;
            var combatMoves = landfallContentDatabase.GetField<Dictionary<DatabaseID, GameObject>>("m_combatMoves");
            for (int i = 0; i < nodeIDS.Count; i++)
            {
                combatMoves.Remove(nodeIDS[i]);
            }
            landfallContentDatabase.SetField("m_combatMoves", combatMoves);

            // AssetLoader
            AssetLoader assetLoader = contentDatabase.AssetLoader;
            var nonStreamableAssets = assetLoader.GetField<Dictionary<DatabaseID, UnityEngine.Object>>("m_nonStreamableAssets");
            for (int i = 0; i < nodeIDS.Count; i++)
            {
                nonStreamableAssets.Remove(nodeIDS[i]);
            }
            assetLoader.SetField("m_nonStreamableAssets", nonStreamableAssets);

            // Other
            assetManager.assets[assetManager.assets.Keys.FirstOrDefault(n =>n.GetName() == "move")] = new Dictionary<string, object>();
            nodeScenes = new List<VirtualNodeScene>();
            LegacyNodeScene[] oldNodeScenes = FindObjectsOfType<LegacyNodeScene>();
            oldNodeScenes.Do(n => Destroy(n.gameObject));
            string[] directories = Directory.GetDirectories(abilitespath);
            for (int i = 0; i < directories.Length; i++)
            {
                string[] files = Directory.GetFiles(directories[i]);
                for (int i2 = 0; i2 < files.Length; i2++)
                {
                    string path = files[i2];
                    if (System.IO.Path.GetExtension(path) != ".ability")
                    {
                        continue;
                    }
                    VirtualNodeScene nodeScene = Serialize.LoadJson<VirtualNodeScene>(File.ReadAllText(path));
                    GameObject ability = AddAbility(nodeScene);
                    ability.GetComponent<NodeRunner>().nodeScene = nodeScene;
                }
                

            }

            UnitBlueprint[] unitBlueprints = contentDatabase.UserContentDatabase.GetUnitBlueprints().ToArray();
            for (int i = 0; i < unitBlueprints.Length; i++)
            {
                CleanUnitblueprint(unitBlueprints[i]);
            }
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
        public static string CleanNodeName(string name)
        {
            return name.Replace("<", "").Replace(">", "").Replace("/", "").Replace("\\", "").Replace(":", "").Replace("*", "").Replace("?", "").Replace("\"", "").Replace("|", "");
        }
        public static string GetPath(VirtualNodeScene nodeScene)
        {
            return abilitespath + "/" + CleanNodeName(nodeScene.abilityName) + "/" + nodeScene.abilityID + ".abilityx";
        }
        public static Sprite GetSprite(VirtualNodeScene nodeScene)
        {
            string image = "";
            string[] imgs = Directory.GetFiles(Directory.GetParent(GetPath(nodeScene)).FullName);
            for (int i = 0; i < imgs.Length; i++)
            {
                if (Path.GetExtension(imgs[i]) == ".png" | Path.GetExtension(imgs[i]) == ".bmp")
                {
                    image = imgs[i];
                }
            }
            if (image != "")
            {

                Texture2D texture = new Texture2D(0, 0);
                texture.LoadImage(File.ReadAllBytes(image));
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
                return sprite;
            }
            else if (sprites.ContainsKey(nodeScene.abilityIcon))
            {
                return sprites[nodeScene.abilityIcon];
            }
            return null;
        }
        public static bool IsAbilityWritten(LegacyNodeScene nodeScene)
        {
            return Directory.Exists(AbilityCreator.abilitespath + "/" + AbilityCreator.CleanNodeName(nodeScene.sceneName));
        }
        public static bool IsAbilityWritten(LegacySavedNodeScene nodeScene)
        {
            return Directory.Exists(AbilityCreator.abilitespath + "/" + AbilityCreator.CleanNodeName(nodeScene.sceneName));
        }
        public static StreamedSceneManager sceneManager;
        public static Dictionary<string, NodeBlueprint> nodeDatabase = new Dictionary<string, NodeBlueprint>();
        public static List<VirtualNodeScene> nodeScenes = new List<VirtualNodeScene>();
        public static string abilitespath = path + "/Abilites";
    }
}