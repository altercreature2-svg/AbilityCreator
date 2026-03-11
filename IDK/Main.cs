using BepInEx;
using DM;
using HarmonyLib;
using IDK.ExampleAbilites;
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
    public class Main : BaseUnityPlugin
    {
        public static GameObject sliceEffect;
        public static Dictionary<string, GameObject> explosions = new Dictionary<string, GameObject>();

        public static Dictionary<string, GameObject> Effects = new Dictionary<string, GameObject>();
        public static Dictionary<string, GameObject> Projectiles = new Dictionary<string, GameObject>();
        public static Dictionary<string, Type> components = new Dictionary<string, Type>();
        public static Dictionary<string, UnitBlueprint> units = new Dictionary<string, UnitBlueprint>();
        public static Dictionary<string, Sprite> pepsis = new Dictionary<string, Sprite>();
        public static Dictionary<string, GameObject> particles = new Dictionary<string, GameObject>();
        public static Dictionary<string, GameObject> abilites = new Dictionary<string, GameObject>();
        public static Dictionary<string, GameObject> weapons = new Dictionary<string, GameObject>();
        public static List<string> sounds = new List<string>();
        public static List<DatabaseID> nodeIDS = new List<DatabaseID>();
        public static List<BundledAbilitesManager.BundledAbility> bundledAbilitiesQueue = new List<BundledAbilitesManager.BundledAbility>();
        public static string path = GamePaths.DataPath + "/Abilty Creator";
        public static string Guide = "";


        private void Awake()
        {

            if (Application.platform == RuntimePlatform.OSXPlayer)
            {
                path = GamePaths.PersistentDataPath + "/Abilty Creator";
                abilitespath = path + "/Abilites";
            }
            Harmony harmony = new Harmony("Alter.AbilityCreator");
            harmony.PatchAll();
            Code.commnet = "Dear Code Viewer (pervert), prepare your self for the most unoptomized, evil, racist code you will ever see";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            if (!Directory.Exists(abilitespath))
            {
                Directory.CreateDirectory(abilitespath);

            }
            StartCoroutine(Call());
            UpdateSaveManager.Handle();
            string[] files = Directory.GetFiles(abilitespath);
            for (int i = 0; i < files.Length; i++)
            {
                Debug.Log("File Extension! " + Path.GetExtension(files[i]));
                if (Path.GetExtension(files[i]) == ".newnodescene")
                {
                    SavedNodeScene nodeScene = DeserializeAbility(File.ReadAllText(files[i]));
                    if (!FixNodeSystem.IsValid(nodeScene))
                    {
                        continue;
                    }
                    string p = abilitespath + "/" + nodeScene.sceneName + "/";
                    if (!Directory.Exists(p))
                        Directory.CreateDirectory(p);
                    File.Copy(files[i], p + nodeScene.id + ".ability");
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
            var allexplo = Resources.FindObjectsOfTypeAll<Explosion>();
            string Filetext = "";
            string Filetext2 = "";
            string Filetext3 = "";
            string Filetext4 = "";
            string Filetext5 = "";
            string Filetext6 = "";
            string Filetext7 = "";
            string Filetext8 = "";
            string Filetext9 = "";
            string Filetext10 = "";
            var index = 0;



            //Explosions
            for (int i = 0; i < allexplo.Length; i++)
            {

                if (explosions.ContainsValue(allexplo[i].gameObject.transform.root.gameObject) == true)
                {

                    continue;
                }

                index++;

                string name = allexplo[i].gameObject.transform.root.gameObject.name;

                name = (name.Contains("_1 Prefabs_VB") ? name.Replace("_1 Prefabs_VB", "") : (name.Contains("_1 Weapons_VB") ? name.Replace("_1 Weapons_VB", "") : name));
                name = (name.Contains("_4 Moves_VB") ? name.Replace("_4 Moves_VB", "") : (name.Contains("_2 Projectiles_VB") ? name.Replace("_2 Projectiles_VB", "") : name));
                name = (name.Contains("_3 Effects_VB") ? name.Replace("_3 Effects_VB", "") : (name.Contains("_0 UnitBases_VB") ? name.Replace("_0 UnitBases_VB", "") : name));
                while (explosions.ContainsKey(name))
                {
                    name += "+";
                }

                explosions.Add(name, allexplo[i].gameObject.transform.root.gameObject);
                Filetext3 += $"\n {index} > {name}";
                ;
            }
            GameObject[] weps = ContentDatabase.Instance().LandfallContentDatabase.GetWeapons().ToArray();
            int index3 = 0;
            for (int i = 0; i < weps.Length; i++)
            {

                if (weapons.ContainsValue(weps[i]))
                {
                    continue;
                }
                index3++;

                string name = weps[i].name;

                name = (name.Contains("_1 Prefabs_VB") ? name.Replace("_1 Prefabs_VB", "") : (name.Contains("_1 Weapons_VB") ? name.Replace("_1 Weapons_VB", "") : name));
                name = (name.Contains("_4 Moves_VB") ? name.Replace("_4 Moves_VB", "") : (name.Contains("_2 Projectiles_VB") ? name.Replace("_2 Projectiles_VB", "") : name));
                name = (name.Contains("_3 Effects_VB") ? name.Replace("_3 Effects_VB", "") : (name.Contains("_0 UnitBases_VB") ? name.Replace("_0 UnitBases_VB", "") : name));
                while (weapons.ContainsKey(name))
                {
                    name += "+";
                }

                weapons.Add(name, weps[i]);
                Filetext10 += $"\n {index3} > {name}";
            }







            File.WriteAllText(path + "/Weapons.txt", Filetext10);
            File.WriteAllText(path + "/Explosions.txt", Filetext);




            // Components
            var allTypes = new List<Type>();
            Assembly[] assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < assemblies.Length; i++)
            {
                try
                {
                    allTypes.AddRange(assemblies[i].GetTypes());
                }
                catch (ReflectionTypeLoadException ex)
                {
                    allTypes.AddRange(ex.Types); // some may be null, filter below
                }
            }
            for (int i = 0; i < allTypes.Count; i++)
            {
                if (typeof(Component).IsAssignableFrom(allTypes[i]))
                {
                    var name2 = allTypes[i].GetCompilableNiceFullName();
                    while (components.ContainsKey(name2))
                    {
                        name2 += "+";
                    }
                    Filetext6 += ">" + name2 + "\n";
                    components.Add(name2, allTypes[i]);
                }


            }
            File.WriteAllText(path + "/Components.txt", Filetext6);
            //Sounds
            var catagories = ServiceLocator.GetService<SoundPlayer>().soundBank.Categories;
            for (int i = 0; i < ServiceLocator.GetService<SoundPlayer>().soundBank.Categories.Length; i++)
            {
                for (int o = 0; o < catagories[i].soundEffects.Length; o++)
                {

                    Filetext2 += ">" + catagories[i].categoryName + "&" + catagories[i].soundEffects[o].soundRef + "\n";
                    if (ServiceLocator.GetService<SoundPlayer>().soundBank.GetSoundEffect(catagories[i].categoryName + "/" + catagories[i].soundEffects[o].soundRef) != null)
                    {
                        Filetext5 += ">" + catagories[i].categoryName + "&" + catagories[i].soundEffects[o].soundRef + "\n";
                        sounds.Add(catagories[i].categoryName + "&" + catagories[i].soundEffects[o].soundRef);
                    }
                }
            }



            File.WriteAllText(path + "/Sounds.txt", Filetext5);
            //Effects
            UnitEffectBase[] effectBases = Resources.FindObjectsOfTypeAll<UnitEffectBase>();
            index = 0;
            for (int i = 0; i < effectBases.Length; i++)
            {
                if (Effects.ContainsValue(effectBases[i].gameObject.transform.root.gameObject) == true)
                {

                    continue;
                }

                index++;

                string name = effectBases[i].gameObject.transform.root.gameObject.name;

                name = (name.Contains("_1 Prefabs_VB") ? name.Replace("_1 Prefabs_VB", "") : (name.Contains("_1 Weapons_VB") ? name.Replace("_1 Weapons_VB", "") : name));
                name = (name.Contains("_4 Moves_VB") ? name.Replace("_4 Moves_VB", "") : (name.Contains("_2 Projectiles_VB") ? name.Replace("_2 Projectiles_VB", "") : name));
                name = (name.Contains("_3 Effects_VB") ? name.Replace("_3 Effects_VB", "") : (name.Contains("_0 UnitBases_VB") ? name.Replace("_0 UnitBases_VB", "") : name));
                while (Filetext3.Contains((name)))
                {
                    name += "+";
                }
                Effects.Add(name, effectBases[i].gameObject.transform.root.gameObject);
                Filetext3 += $"\n > {name}";
            }

            File.WriteAllText(path + "/Effects.txt", Filetext3);

            // allParticles
            ParticleSystem[] allParticles = Resources.FindObjectsOfTypeAll<ParticleSystem>();
            for (int i = 0; i < allParticles.Length; i++)
            {
                string text = allParticles[i].transform.root.name + "+" + allParticles[i].name;
                while (particles.ContainsKey(text))
                {
                    text += "+";
                }
                particles.Add(text, allParticles[i].gameObject);
                Filetext9 += "> " + text + "\n";
            }
            File.WriteAllText(path + "/Particles.txt", Filetext9);
            var allunits = ContentDatabase.Instance().GetAllUnitBlueprints().ToArray();
            var langauge = Localizer.GetLanguage(Localizer.Language.LANG_EN_US);

            for (int i = 0; i < allunits.Length; i++)
            {
                string name;
                if (langauge.ContainsKey(allunits[i].Entity.Name))
                    name = langauge[allunits[i].Entity.Name];
                else
                    name = allunits[i].Entity.Name;
                while (units.ContainsKey(name))
                {
                    name += "+";
                }
                Filetext7 += i + ">" + name + "\n";
                units.Add(name, allunits[i]);

            }


            File.WriteAllText(path + "/Units.txt", Filetext7);


            //sprites
            var allsprites = new List<Sprite>();

            allsprites.AddRange(ContentDatabase.Instance().GetFactionIcons().Select(n => n.Entity.LargeSpriteIcon));
            allsprites.AddRange(ContentDatabase.Instance().GetAllCombatMoves().Select(n => n.GetComponent<CharacterItem>().Entity.LargeSpriteIcon));
            allsprites.AddRange(ContentDatabase.Instance().GetAllUnitBlueprints().Where(n => n.IsCustomUnit).Select(n => n.Entity.LargeSpriteIcon));

            int index2 = 0;
            for (int i = 0; i < allsprites.Count; i++)
            {
                try
                {
                    if (allsprites[i].name != string.Empty)
                    {
                        pepsis.Add(allsprites[i].name, allsprites[i]);
                        Filetext8 += index2 + ">" + allsprites[i].name + "\n";
                    }
                    index2++;
                }
                catch (Exception)
                {
                }

            }

            File.WriteAllText(path + "/Sprites.txt", Filetext8);

            //projectiles
            index = 0;
            Projectile[] projectiles = Resources.FindObjectsOfTypeAll<Projectile>();
            for (int i = 0; i < projectiles.Length; i++)
            {
                if (Projectiles.ContainsValue(projectiles[i].gameObject.transform.root.gameObject) == true)
                {

                    continue;
                }

                index++;

                string name = projectiles[i].gameObject.transform.root.gameObject.name;

                name = (name.Contains("_1 Prefabs_VB") ? name.Replace("_1 Prefabs_VB", "") : (name.Contains("_1 Weapons_VB") ? name.Replace("_1 Weapons_VB", "") : name));
                name = (name.Contains("_4 Moves_VB") ? name.Replace("_4 Moves_VB", "") : (name.Contains("_2 Projectiles_VB") ? name.Replace("_2 Projectiles_VB", "") : name));
                name = (name.Contains("_3 Effects_VB") ? name.Replace("_3 Effects_VB", "") : (name.Contains("_0 UnitBases_VB") ? name.Replace("_0 UnitBases_VB", "") : name));
                while (Filetext4.Contains((name)))
                {
                    name += "+";
                }
                Projectiles.Add(name, projectiles[i].gameObject.transform.root.gameObject);
                Filetext4 += $"\n {index} > {name}";
            }
            File.WriteAllText(path + "/Projectiles.txt", Filetext4);




            Code.commnet = "------------------INIT------------------------";
            CreateNodeBlueprint("BATTLE_BEGIN", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.Trigger,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {

            }, NodeBlueprint.Type.Function, Color.grey, "When Battle Begins", "Triggers", node: typeof(WhenBattleBegins));
            CreateNodeBlueprint("UNIT_SPAWN", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.Trigger,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {

            }, NodeBlueprint.Type.Function, Color.grey, "When unit spawns", "Triggers", node: typeof(WhenUnitSpawned));
            CreateNewNodeBlueprint("ENDBATTLE", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.Trigger,
                NodeBlueprint.ConnectionType.Triggered,
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
            CreateNewNodeBlueprint("GETMAXUNITHEALTH", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.ReciveUnit,
                NodeBlueprint.ConnectionType.GiveVariable,
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
            CreateNewNodeBlueprint("GETUNITHEALTH", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.ReciveUnit,
                NodeBlueprint.ConnectionType.GiveVariable,
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
            CreateNewNodeBlueprint("SETUNITHEALTH", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.Triggered,
                NodeBlueprint.ConnectionType.ReciveUnit,
                NodeBlueprint.ConnectionType.Trigger,
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
            CreateNewNodeBlueprint("SETUNITDAMAGE", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.Triggered,
                NodeBlueprint.ConnectionType.ReciveUnit,
                NodeBlueprint.ConnectionType.Trigger,
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
            CreateNewNodeBlueprint("SETUNITCOOLDOWN", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.Triggered,
                NodeBlueprint.ConnectionType.ReciveUnit,
                NodeBlueprint.ConnectionType.Trigger,
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

            CreateNewNodeBlueprint("QUICKBUFF", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.Triggered,
                NodeBlueprint.ConnectionType.ReciveUnit,
                NodeBlueprint.ConnectionType.Trigger,
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




            CreateNodeBlueprint("LOG", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.Triggered,
                NodeBlueprint.ConnectionType.Trigger,
                NodeBlueprint.ConnectionType.ReciveVariable,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
            }, NodeBlueprint.Type.Function, Color.white, "Log variable", "Variables", node: typeof(LogVariable));
            CreateNodeBlueprint("LOGANY", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.Triggered,
                NodeBlueprint.ConnectionType.Trigger,
                NodeBlueprint.ConnectionType.ReciveAnything,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
            }, NodeBlueprint.Type.Function, Color.white, "Log anything", "Gameobjects", node: typeof(LogAnything));


            CreateNewNodeBlueprint("ABILITYACTVIATE", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.Trigger
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


            CreateNewNodeBlueprint("UNITWASATTACKED", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.Trigger
            }, new List<NodeBlueprint.Field>()
            {
                 new NodeBlueprint.Field("Delay", TMP_InputField.ContentType.DecimalNumber),
                 new NodeBlueprint.Field("Range", TMP_InputField.ContentType.DecimalNumber),
            }, node: typeof(UnitWasAttacked), NodeBlueprint.Type.Input, Color.grey, "When unit gets attacked...", "Triggers");
            CreateNodeBlueprint("INTERVAL", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.Trigger
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
                { "Interval", TMP_InputField.ContentType.DecimalNumber },
            }, NodeBlueprint.Type.Input, Color.grey, "Do Every...", "Triggers", node: typeof(DoEveryNode));
            CreateNodeBlueprint("WHENUNITDIE", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.Trigger
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
            CreateNodeBlueprint("WHENUNITDAMAGED", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.Trigger,
                NodeBlueprint.ConnectionType.GiveUnit
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
            }, NodeBlueprint.Type.Input, Color.grey, "When unit damaged...", "Triggers", node: typeof(WhenUnitDamaged))
            ;
            CreateNodeBlueprint("WHENUNITATTACKS", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.Trigger,
                NodeBlueprint.ConnectionType.GiveUnit,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
            }, NodeBlueprint.Type.Input, Color.grey, "When unit attacks...", "Triggers", node: typeof(WhenUnitAttacks));
            CreateNewNodeBlueprint("PROJECTILERANGE", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.Trigger,
                NodeBlueprint.ConnectionType.GiveGameObject,
            }, new List<NodeBlueprint.Field>()
            {
                new NodeBlueprint.Field("Range",TMP_InputField.ContentType.DecimalNumber),
                new NodeBlueprint.Field("Block Power",TMP_InputField.ContentType.DecimalNumber),
            }, node: typeof(WhenProjectileEntersRange), NodeBlueprint.Type.Input, Color.grey, "When projectile in range...", "Triggers");
            CreateNodeBlueprint("IS_BATTLE", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.Trigger,
                NodeBlueprint.ConnectionType.Triggered,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {

            }, NodeBlueprint.Type.Function, Color.grey, "Run if in battle state", "Control", node: typeof(IsBattleState));
            CreateNodeBlueprint("IS_DEAD", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.Trigger,
                NodeBlueprint.ConnectionType.Triggered,
                NodeBlueprint.ConnectionType.ReciveUnit,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {

            }, NodeBlueprint.Type.Function, Color.grey, "Run if unit is dead", "Control", node: typeof(IsDead));
            CreateNodeBlueprint("IS_NOTDEAD", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.Trigger,
                NodeBlueprint.ConnectionType.Triggered,
                NodeBlueprint.ConnectionType.ReciveUnit,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {

            }, NodeBlueprint.Type.Function, Color.grey, "Run if unit is alive", "Control", node: typeof(IsAlive));
            CreateNodeBlueprint("PAUSE", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.Trigger,
                NodeBlueprint.ConnectionType.Triggered,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
                { "Seconds", TMP_InputField.ContentType.DecimalNumber },

            }, NodeBlueprint.Type.Function, Color.grey, "Do after pause", "Control", node: typeof(PauseNode));
            CreateNodeBlueprint("REAPET", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.Trigger,
                NodeBlueprint.ConnectionType.Triggered,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
                { "Times", TMP_InputField.ContentType.IntegerNumber },
                { "Delay", TMP_InputField.ContentType.DecimalNumber},
            }, NodeBlueprint.Type.Input, Color.grey, "Repeat", "Control", node: typeof(ReapetNode));
            CreateNodeBlueprint("REAPETVAR", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.Trigger,
                NodeBlueprint.ConnectionType.Triggered,
                NodeBlueprint.ConnectionType.ReciveVariable,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
                { "Delay", TMP_InputField.ContentType.DecimalNumber},
            }, NodeBlueprint.Type.Input, Color.grey, "Repeat for variable", "Control", node: typeof(ReapetVarNode));
            CreateNewNodeBlueprint("FOREACH", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.Trigger,
                NodeBlueprint.ConnectionType.Triggered,
                NodeBlueprint.ConnectionType.ReciveAnything,
                NodeBlueprint.ConnectionType.GiveAnything,

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
            CreateNewNodeBlueprint("FILTER", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.ReciveAnything,
                NodeBlueprint.ConnectionType.GiveAnything,

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



            CreateNodeBlueprint("NEWOBJ", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.GiveGameObject
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
                { "Object name", TMP_InputField.ContentType.Standard },
            }, NodeBlueprint.Type.Function, Color.green, "Create gameObject", "Gameobjects", node: typeof(CreateGameobject));



            CreateNodeBlueprint("ENEMY", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.GiveUnit
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
            }, NodeBlueprint.Type.Function, Color.green, "Enemy unit", "Units", node: typeof(EnemyNode));
            CreateNodeBlueprint("SELF", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.GiveUnit
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
            }, NodeBlueprint.Type.Function, Color.green, "Self unit", "Units", node: typeof(SelfNode));
            CreateNodeBlueprint("EXPSENSIVETEAMMATE", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.ReciveUnit,
                NodeBlueprint.ConnectionType.GiveUnit,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
            }, NodeBlueprint.Type.Function, Color.green, "Most expensive teammate of unit", "Units", node: typeof(MostExspensiveTeamMateUnit));
            CreateNodeBlueprint("EXPSENSIVEENEMY", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.ReciveUnit,
                NodeBlueprint.ConnectionType.GiveUnit,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
            }, NodeBlueprint.Type.Function, Color.green, "Most expensive enemy of unit", "Units", node: typeof(MostExspensiveEnemy));
            CreateNodeBlueprint("ALLTEAM", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.GiveUnit
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
            }, NodeBlueprint.Type.Function, Color.green, "All units of same team", "Units", node: typeof(AllTeam));
            CreateNodeBlueprint("ALLOTHERTEAM", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.GiveUnit
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
            }, NodeBlueprint.Type.Function, Color.green, "All units of other team", "Units", node: typeof(AllOtherTeam));


            CreateNodeBlueprint("GETUNIT", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.GiveUnit,
                NodeBlueprint.ConnectionType.ReciveGameObject
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
            }, NodeBlueprint.Type.Function, Color.green, "Get unit from gameobject", "Unit Tools", node: typeof(GetUnitFromGameobject));
            CreateNodeBlueprint("TEAMMATE", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.GiveUnit,
                NodeBlueprint.ConnectionType.ReciveUnit
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
            }, NodeBlueprint.Type.Function, Color.green, "Closest team mate of unit", "Unit Tools", node: typeof(ClosestTeamMateUnit));
            CreateNodeBlueprint("FREAZE", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.Trigger,
                NodeBlueprint.ConnectionType.Triggered,
                NodeBlueprint.ConnectionType.ReciveUnit
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
                { "Length", TMP_InputField.ContentType.DecimalNumber}
            }, NodeBlueprint.Type.Function, Color.magenta, "freeze unit for", "Unit Tools", node: typeof(FreezeNode));

            CreateNewNodeBlueprint("GAMEOBJ", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.ReciveUnit,
                NodeBlueprint.ConnectionType.GiveGameObject
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
            CreateNewNodeBlueprint("CLOTH", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.ReciveUnit,
                NodeBlueprint.ConnectionType.GiveGameObject
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
            CreateNewNodeBlueprint("ABILITIES", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.ReciveUnit,
                NodeBlueprint.ConnectionType.GiveGameObject
            }
            , new List<NodeBlueprint.Field>
            {
            }, typeof(GetAbilites), NodeBlueprint.Type.Function, Color.green, "Get abilites of unit", "Unit Tools");
            CreateNewNodeBlueprint("GETWEAPON", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.ReciveUnit,
                NodeBlueprint.ConnectionType.GiveGameObject
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
            CreateNewNodeBlueprint("SETFIST", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.Trigger,
                NodeBlueprint.ConnectionType.Triggered,
                NodeBlueprint.ConnectionType.ReciveUnit,
            }
            , new List<NodeBlueprint.Field>
            {
            }, typeof(SetFistNode), NodeBlueprint.Type.Function, Color.green, "Make unit use fists", "Weapons");
            CreateNewNodeBlueprint("GETEYE", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.ReciveUnit,
                NodeBlueprint.ConnectionType.GiveGameObject
            }
            , new List<NodeBlueprint.Field>
            {
            }, typeof(GetEye), NodeBlueprint.Type.Function, Color.green, "Get eyes of unit", "Unit Tools");

            CreateNewNodeBlueprint("GETWEAPONGLOBAL", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.GiveGameObject
            }, new List<NodeBlueprint.Field>
            {
                 new NodeBlueprint.Field("Weapon name", TMP_InputField.ContentType.Standard)
                 {
                     fieldType = NodeBlueprint.Field.FieldType.Weapon
                 }
             }, node: typeof(GetWeaponOfName), NodeBlueprint.Type.Function, Color.magenta, "Get weapon of name", "Weapons");
            CreateNewNodeBlueprint("DEAL_DAMAGE", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.Trigger,
                NodeBlueprint.ConnectionType.Triggered,
                NodeBlueprint.ConnectionType.ReciveUnit,
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
            CreateNodeBlueprint("DIE", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.Trigger,
                NodeBlueprint.ConnectionType.Triggered,
                NodeBlueprint.ConnectionType.ReciveUnit,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
            }, NodeBlueprint.Type.Function, Color.red, "Kill", "Unit Tools", node: typeof(DieNode));
            CreateNodeBlueprint("REVIVE", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.Trigger,
                NodeBlueprint.ConnectionType.Triggered,
                NodeBlueprint.ConnectionType.ReciveUnit,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
            }, NodeBlueprint.Type.Function, Color.red, "Revive unit", "Unit Tools", node: typeof(ReDie));
            CreateNewNodeBlueprint("REVIVEFIXER", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.Trigger,
                NodeBlueprint.ConnectionType.Triggered,
                NodeBlueprint.ConnectionType.ReciveUnit,
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
            CreateNodeBlueprint("SWAPTEAM", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.Trigger,
                NodeBlueprint.ConnectionType.Triggered,
                NodeBlueprint.ConnectionType.ReciveUnit,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
            }, NodeBlueprint.Type.Function, Color.red, "Swap unit's team", "Unit Tools", node: typeof(SwapTeam));
            CreateNewNodeBlueprint("SPAWNPROJ", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.Trigger,
                NodeBlueprint.ConnectionType.Triggered,
                NodeBlueprint.ConnectionType.ReciveGameObject,
                NodeBlueprint.ConnectionType.GiveGameObject,
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
            CreateNewNodeBlueprint("PARRYPROJECTILE", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.Trigger,
                NodeBlueprint.ConnectionType.Triggered,
                NodeBlueprint.ConnectionType.ReciveGameObject,
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
            CreateNewNodeBlueprint("PLAYSLICE", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.Trigger,
                NodeBlueprint.ConnectionType.Triggered,
                NodeBlueprint.ConnectionType.ReciveGameObject,
                NodeBlueprint.ConnectionType.GiveGameObject,
            }, new List<NodeBlueprint.Field>()
            {

            }, node: typeof(PlaySlice), NodeBlueprint.Type.Function, Color.red, "Play slice effect", "Projectiles");
            CreateNewNodeBlueprint("EXPLOSION", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.Trigger,
                NodeBlueprint.ConnectionType.Triggered,
                NodeBlueprint.ConnectionType.ReciveGameObject,
                NodeBlueprint.ConnectionType.GiveGameObject,
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
            CreateNewNodeBlueprint("PARTICLE", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.Trigger,
                NodeBlueprint.ConnectionType.Triggered,
                NodeBlueprint.ConnectionType.ReciveGameObject,
                NodeBlueprint.ConnectionType.GiveGameObject,
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

            CreateNewNodeBlueprint("ADDEFFECT", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.Trigger,
                NodeBlueprint.ConnectionType.Triggered,
                NodeBlueprint.ConnectionType.ReciveUnit,
            }
            , new List<NodeBlueprint.Field>
            {
                new NodeBlueprint.Field( "Effect", TMP_InputField.ContentType.Standard)
                {
                    fieldType = NodeBlueprint.Field.FieldType.Effect,
                },
            }, node: typeof(AddEffectNode), NodeBlueprint.Type.Function, Color.red, "Add Effect to unit", "Unit Tools");

            CreateNodeBlueprint("STUNWEAPONS", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.Trigger,
                NodeBlueprint.ConnectionType.Triggered,
                NodeBlueprint.ConnectionType.ReciveUnit,
            }
            , new Dictionary<string, TMP_InputField.ContentType>
            {
                { "Time", TMP_InputField.ContentType.DecimalNumber},


            }, NodeBlueprint.Type.Function, Color.red, "Stun weapons for unit", "Weapons", node: typeof(StunWeaponsNode));

            CreateNewNodeBlueprint("LETGO", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.Trigger,
                NodeBlueprint.ConnectionType.Triggered,
                NodeBlueprint.ConnectionType.ReciveUnit,
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
            CreateNewNodeBlueprint("SETWEAPON", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.Trigger,
                NodeBlueprint.ConnectionType.Triggered,
                NodeBlueprint.ConnectionType.ReciveUnit,
                NodeBlueprint.ConnectionType.ReciveGameObject,
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
            CreateNewNodeBlueprint("SPAWNUNIT", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.Trigger,
                NodeBlueprint.ConnectionType.Triggered,
                NodeBlueprint.ConnectionType.ReciveGameObject,
                NodeBlueprint.ConnectionType.GiveUnit,
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



            CreateNodeBlueprint("ALLABS", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.Trigger,
                NodeBlueprint.ConnectionType.Triggered,
                NodeBlueprint.ConnectionType.ReciveUnit,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
            }, NodeBlueprint.Type.Function, Color.magenta, "Play all abilites", "Unit Tools", node: typeof(PlayAllAbilites));
            CreateNodeBlueprint("IMMUNE", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.Trigger,
                NodeBlueprint.ConnectionType.Triggered,
                NodeBlueprint.ConnectionType.ReciveUnit,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
                { "Length", TMP_InputField.ContentType.DecimalNumber },
            }, NodeBlueprint.Type.Function, Color.magenta, "Make Immune for", "Unit Tools", node: typeof(MakeUnitImmune));

            CreateNodeBlueprint("TOGGLEPROPS", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.Trigger,
                NodeBlueprint.ConnectionType.Triggered,
                NodeBlueprint.ConnectionType.ReciveUnit,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
                 { "From", TMP_InputField.ContentType.IntegerNumber},
                { "To", TMP_InputField.ContentType.IntegerNumber},
            }, NodeBlueprint.Type.Function, Color.magenta, "Toggle Props at indexs", "Unit Tools", node: typeof(ToggleProps));
            CreateNewNodeBlueprint("ADDPROPS", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.Trigger,
                NodeBlueprint.ConnectionType.Triggered,
                NodeBlueprint.ConnectionType.ReciveUnit,
                NodeBlueprint.ConnectionType.ReciveGameObject,
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
            }, node: typeof(AddProps), NodeBlueprint.Type.Function, Color.magenta, "Add clothes/abilites to unit", "Unit Tools");
            CreateNodeBlueprint("ADDFORCE", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.Trigger,
                NodeBlueprint.ConnectionType.Triggered,
                NodeBlueprint.ConnectionType.ReciveGameObject,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
                { "Forward", TMP_InputField.ContentType.DecimalNumber},
                { "Upwareds", TMP_InputField.ContentType.DecimalNumber},
                { "Sideway", TMP_InputField.ContentType.DecimalNumber},
            }, NodeBlueprint.Type.Function, Color.magenta, "Add force to gameobject", "Gameobjects", node: typeof(AddForceNode));
            CreateNodeBlueprint("ADDFORCEGLOBAL", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.Trigger,
                NodeBlueprint.ConnectionType.Triggered,
                NodeBlueprint.ConnectionType.ReciveGameObject,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
                { "X", TMP_InputField.ContentType.DecimalNumber},
                { "Y", TMP_InputField.ContentType.DecimalNumber},
                 { "Z", TMP_InputField.ContentType.DecimalNumber},

            }, NodeBlueprint.Type.Function, Color.magenta, "Add global force to gameobject", "Gameobjects", node: typeof(AddGlobalForceNode));

            CreateNewNodeBlueprint("PLAYSOUND", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.Trigger,
                NodeBlueprint.ConnectionType.Triggered,
                NodeBlueprint.ConnectionType.ReciveGameObject,
            }, new List<NodeBlueprint.Field>
            {
                new NodeBlueprint.Field( "SoundRef", TMP_InputField.ContentType.Standard)
                {
                    fieldType = NodeBlueprint.Field.FieldType.Sound,
                },
                new NodeBlueprint.Field("Volume", TMP_InputField.ContentType.DecimalNumber),
                new NodeBlueprint.Field( "Pitch", TMP_InputField.ContentType.DecimalNumber),



            }, node: typeof(PlaySoundNode), NodeBlueprint.Type.Function, Color.magenta, "Play sound at gameobject position", "Gameobjects");

            CreateNewNodeBlueprint("GOTOUNIT", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.Trigger,
                NodeBlueprint.ConnectionType.Triggered,
                NodeBlueprint.ConnectionType.ReciveGameObject,
                NodeBlueprint.ConnectionType.ReciveUnit,
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
            CreateNewNodeBlueprint("ROTATETOUNIT", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.Trigger,
                NodeBlueprint.ConnectionType.Triggered,
                NodeBlueprint.ConnectionType.ReciveGameObject,
                NodeBlueprint.ConnectionType.ReciveUnit,
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
            CreateNewNodeBlueprint("GETDISTANCE", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.ReciveGameObject,
                NodeBlueprint.ConnectionType.ReciveUnit,
                NodeBlueprint.ConnectionType.GiveVariable,
            }, new List<NodeBlueprint.Field>()
            {
            }, node: typeof(GetDistanceFrom), NodeBlueprint.Type.Function, Color.magenta, "Get distance from unit to gameobject", "Gameobjects");
            CreateNewNodeBlueprint("TRANSFORMPOS", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.Trigger,
                NodeBlueprint.ConnectionType.Triggered,
                NodeBlueprint.ConnectionType.ReciveGameObject,
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

            CreateNodeBlueprint("TRANSFORMSCALE", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.Trigger,
                NodeBlueprint.ConnectionType.Triggered,
                NodeBlueprint.ConnectionType.ReciveGameObject,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
                { "x", TMP_InputField.ContentType.DecimalNumber},
                { "y", TMP_InputField.ContentType.DecimalNumber},
                { "z", TMP_InputField.ContentType.DecimalNumber}
            }, NodeBlueprint.Type.Function, Color.yellow, "Change scale of gameobj", "Animations", node: typeof(TransformScaleNode));
            CreateNewNodeBlueprint("TRANSFORMROT", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.Trigger,
                NodeBlueprint.ConnectionType.Triggered,
                NodeBlueprint.ConnectionType.ReciveGameObject,
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
            CreateNewNodeBlueprint("COLOR", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.Trigger,
                NodeBlueprint.ConnectionType.Triggered,
                NodeBlueprint.ConnectionType.ReciveGameObject,
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
            CreateNewNodeBlueprint("COLORHSV", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.Trigger,
                NodeBlueprint.ConnectionType.Triggered,
                NodeBlueprint.ConnectionType.ReciveGameObject,
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

            }, node: typeof(ColorObjectNodeHSV), NodeBlueprint.Type.Function, Color.yellow, "Color object (HSV)", "Gameobjects"); ;
            CreateNodeBlueprint("DESTROY", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.Trigger,
                NodeBlueprint.ConnectionType.Triggered,
                NodeBlueprint.ConnectionType.ReciveGameObject,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {

            }, NodeBlueprint.Type.Function, Color.yellow, "Destroy", "Gameobjects", node: typeof(DestroyNode));
            CreateNodeBlueprint("CREATEFUNC", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.Trigger,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
                {"Function name" , TMP_InputField.ContentType.Standard}
            }, NodeBlueprint.Type.Function, Color.yellow, "Create function", "Functions", node: typeof(CreateFunctionNode));
            CreateNodeBlueprint("RUNFUNC", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.Triggered,
                NodeBlueprint.ConnectionType.Trigger,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
                {"Function name" , TMP_InputField.ContentType.Standard}
            }, NodeBlueprint.Type.Function, Color.yellow, "Run function", "Functions", node: typeof(RunFunctionNode));
            CreateNodeBlueprint("ADDONHITACTION", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.Triggered,
                NodeBlueprint.ConnectionType.Trigger,
                NodeBlueprint.ConnectionType.ReciveGameObject,
                NodeBlueprint.ConnectionType.GiveGameObject,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
            }, NodeBlueprint.Type.Function, Color.yellow, "Add projectile hit action ", "Projectiles", node: typeof(AddProjectileOnHitAction));
            CreateNodeBlueprint("ADDONCOLLISIONACTION", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.Triggered,
                NodeBlueprint.ConnectionType.Trigger,
                NodeBlueprint.ConnectionType.ReciveGameObject,
                NodeBlueprint.ConnectionType.GiveGameObject,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
            }, NodeBlueprint.Type.Function, Color.yellow, "Add collision enter action ", "Gameobjects", node: typeof(AddOnCollisionAction));
            CreateNewNodeBlueprint("ADDCOMP", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.Trigger,
                NodeBlueprint.ConnectionType.Triggered,
                NodeBlueprint.ConnectionType.ReciveGameObject,
                NodeBlueprint.ConnectionType.GiveComponent,
            }, new List<NodeBlueprint.Field>
            {
                new NodeBlueprint.Field( "Component name", TMP_InputField.ContentType.Standard)
                {
                    fieldType = NodeBlueprint.Field.FieldType.Component
                },
            }, node: typeof(AddComponentNode), NodeBlueprint.Type.Function, Color.yellow, "Add Component to gameObj", "Gameobjects");

            CreateNewNodeBlueprint("GETCOMP", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.ReciveGameObject,
                NodeBlueprint.ConnectionType.GiveComponent,
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
            CreateNewNodeBlueprint("SetField", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.Trigger,
                NodeBlueprint.ConnectionType.Triggered,
                NodeBlueprint.ConnectionType.ReciveAnything,
            }, new List<NodeBlueprint.Field>
            {
                new NodeBlueprint.Field( "Field name", TMP_InputField.ContentType.Standard)
                {
                    fieldType = NodeBlueprint.Field.FieldType.Field
                },
                new NodeBlueprint.Field( "Value", TMP_InputField.ContentType.Standard),

                new NodeBlueprint.Field( "Type", TMP_InputField.ContentType.Standard)
                {
                    fieldType = NodeBlueprint.Field.FieldType.Component
                },
            }, node: typeof(SetFieldNode), NodeBlueprint.Type.Function, Color.yellow, "Set Field for gameObj", "Gameobjects");
            CreateNodeBlueprint("GETFIELD", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.ReciveAnything,
                NodeBlueprint.ConnectionType.GiveAnything,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
                { "Field name", TMP_InputField.ContentType.Standard},
            }, NodeBlueprint.Type.Function, Color.yellow, "Get Field for gameObj", "Gameobjects", node: typeof(GetFieldNode));
            CreateNodeBlueprint("INVOKE", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.Trigger,
                NodeBlueprint.ConnectionType.Triggered,
                NodeBlueprint.ConnectionType.ReciveAnything ,
                NodeBlueprint.ConnectionType.GiveAnything,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
                { "Method name", TMP_InputField.ContentType.Standard},

            }, NodeBlueprint.Type.Function, Color.yellow, "Invoke Method From Component", "Gameobjects", node: typeof(InvokeMethodNode));
            CreateNodeBlueprint("DUPE", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.Trigger,
                NodeBlueprint.ConnectionType.Triggered,
                NodeBlueprint.ConnectionType.ReciveAnything,
                NodeBlueprint.ConnectionType.GiveAnything,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {

            }, NodeBlueprint.Type.Function, Color.yellow, "Duplicate object", "Gameobjects", node: typeof(Duplicate));
            CreateNewNodeBlueprint("SETACTIVE", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.Trigger,
                NodeBlueprint.ConnectionType.Triggered,
                NodeBlueprint.ConnectionType.ReciveGameObject,
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


            CreateNodeBlueprint("VARCREATE", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.GiveVariable,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
                { "variable name", TMP_InputField.ContentType.Standard},

            }, NodeBlueprint.Type.Function, Color.white, "Get variable", "Variables", node: typeof(CreateVariableNode));

            CreateNodeBlueprint("VARADD", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.Trigger,
                NodeBlueprint.ConnectionType.Triggered,
                NodeBlueprint.ConnectionType.ReciveVariable,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {

                { "value", TMP_InputField.ContentType.DecimalNumber},

            }, NodeBlueprint.Type.Function, Color.white, "Change variable by", "Variables", node: typeof(ChangeVariableBy));
            CreateNodeBlueprint("VARSET", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.Trigger,
                NodeBlueprint.ConnectionType.Triggered,
                NodeBlueprint.ConnectionType.ReciveVariable,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
                { "value", TMP_InputField.ContentType.DecimalNumber},

            }, NodeBlueprint.Type.Function, Color.white, "Set variable to", "Variables", node: typeof(SetVariableTo));
            CreateNewNodeBlueprint("VARIF", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.Triggered,
                NodeBlueprint.ConnectionType.ReciveVariable,
                NodeBlueprint.ConnectionType.Trigger,
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
            CreateNewNodeBlueprint("RANDOM", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.GiveVariable,
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
            CreateNodeBlueprint("OBJECTVARCREATE", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.GiveObjectVariable,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
                { "variable name", TMP_InputField.ContentType.Standard},

            }, NodeBlueprint.Type.Function, new Color(.2f, .2f, .8f), "Get object variable", "Object Variables", node: typeof(CreateObjectVariableNode));
            CreateNodeBlueprint("OBJECTVARSET", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.Trigger,
                NodeBlueprint.ConnectionType.Triggered,
                NodeBlueprint.ConnectionType.ReciveObjectVariable,
                NodeBlueprint.ConnectionType.ReciveAnything,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
            }, NodeBlueprint.Type.Function, new Color(.2f, .2f, .8f), "Store object in object variable", "Object Variables", node: typeof(SetObjectVariableTo));
            CreateNodeBlueprint("OBJECTVARCLEAR", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.Trigger,
                NodeBlueprint.ConnectionType.Triggered,
                NodeBlueprint.ConnectionType.ReciveObjectVariable,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
            }, NodeBlueprint.Type.Function, new Color(.2f, .2f, .8f), "Clear object variable", "Object Variables", node: typeof(ClearObjectVariable));
            CreateNodeBlueprint("OBJECTVARIABLEVALUE", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.ReciveObjectVariable,
                NodeBlueprint.ConnectionType.GiveAnything,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {
            }, NodeBlueprint.Type.Function, new Color(.2f, .2f, .8f), "Get value of object variable", "Object Variables", node: typeof(GetValueOfObjectVariableNode));




            //Convert
            CreateNodeBlueprint("CONVERTOBJ", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.ReciveGameObject,
                NodeBlueprint.ConnectionType.GiveAnything,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {

            }, NodeBlueprint.Type.Function, Color.black, "Convert gameobject to anything", "Convert", node: typeof(ConvertObj));
            CreateNodeBlueprint("CONVERTUNIT", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.ReciveUnit,
                NodeBlueprint.ConnectionType.GiveAnything,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {

            }, NodeBlueprint.Type.Function, Color.black, "Convert unit to anything", "Convert", node: typeof(ConvertUnit));
            CreateNodeBlueprint("CONVERTCOMP", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.ReciveComponent,
                NodeBlueprint.ConnectionType.GiveAnything,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {

            }, NodeBlueprint.Type.Function, Color.black, "Convert component to anything", "Convert", node: typeof(ConvertComp));
            CreateNodeBlueprint("CONVERTOBJ2", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.ReciveAnything,
                NodeBlueprint.ConnectionType.GiveGameObject,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {

            }, NodeBlueprint.Type.Function, Color.black, "Convert anything to gameobject", "Convert", node: typeof(AnythingToObj));
            CreateNodeBlueprint("CONVERTUNIT2", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.ReciveAnything,
                NodeBlueprint.ConnectionType.GiveUnit,
            }, new Dictionary<string, TMP_InputField.ContentType>
            {

            }, NodeBlueprint.Type.Function, Color.black, "Convert anything to unit", "Convert", node: typeof(AnythingToUnit));
            CreateNodeBlueprint("CONVERTCOMP2", new List<NodeBlueprint.ConnectionType>()
            {
                NodeBlueprint.ConnectionType.ReciveAnything,
                NodeBlueprint.ConnectionType.GiveComponent,
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
                    var n = DeserializeAbility(File.ReadAllText(path));
                    AddAbility(n.SavedNodeSceneToNodeScene());
                }

            }


            Code.commnet = "------------------Ability------------------------";

            yield return new WaitUntil(() => TABSSceneManager.IsInMainMenuScene());
            yield return new WaitForEndOfFrame(); ;

        }
        public static IEnumerator ForceShowModal(BundledAbilitesManager.BundledAbility bundledAbility)
        {
            if (Main.abilites.ContainsKey(bundledAbility.abilityName))
                yield break;

            yield return new WaitUntil(() => (int)ServiceLocator.GetService<LoadingScreenHandler>().GetField("currentLoadingScreenFadeState") == 0);
            yield return MyModalPanel.ShowModal(bundledAbility);
        }
        public static GameObject CreateAbility(NodeScene nodeScene, bool isFinal = false)
        {
            if (!nodeScenes.Contains(nodeScene))
                nodeScenes.Add(nodeScene);
            GameObject abilityObject = new GameObject(nodeScene.sceneName);
            if (isFinal)
                abilityObject.hideFlags = HideFlags.HideAndDontSave;
            SpecialAbility specialAbility = abilityObject.AddComponent<SpecialAbility>();
            specialAbility.SetField("m_entity", new Landfall.TABS.DatabaseEntity(Landfall.TABS.Workshop.WorkshopContentType.Any)
            {
                GUID = new Landfall.TABS.DatabaseID()
                {
                    m_ID = nodeScene.id,
                    m_modID = -2,

                },
                Name = nodeScene.sceneName,
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
            nodeScene.isFinal = isFinal;
            return abilityObject;
        }
        public static GameObject AddAbility(NodeScene nodeScene)
        {
            try
            {
                GameObject abilityObj = CreateAbility(nodeScene, true);

                SpecialAbility specialAbility = abilityObj.GetComponent<SpecialAbility>();
                SavedNodeSceneStorer.gameobjs.Add(abilityObj);
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
                if (abilites.ContainsKey(abilityObj.GetComponent<NodeRunner>().nodeScene.sceneName))
                    abilites.Remove(abilityObj.GetComponent<NodeRunner>().nodeScene.sceneName);
                abilites.Add(abilityObj.GetComponent<NodeRunner>().nodeScene.sceneName, abilityObj);
                return abilityObj;
            }
            catch { return null; }
        }
        void CreateNodeBlueprint(string key, List<NodeBlueprint.ConnectionType> connectionsTypes, Dictionary<string, TMP_InputField.ContentType> fields, NodeBlueprint.Type type, Color color, string name, string Tab, bool longfield = false, Type node = null)
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
        void CreateNewNodeBlueprint(string key, List<NodeBlueprint.ConnectionType> connectionsTypes, List<NodeBlueprint.Field> fields, Type node, NodeBlueprint.Type type, Color color, string name, string Tab, bool longfield = false)
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
            abilites = new Dictionary<string, GameObject>();
            nodeScenes = new List<NodeScene>();
            NodeScene[] oldNodeScenes = FindObjectsOfType<NodeScene>();
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
                    NodeScene nodeScene = DeserializeAbility(File.ReadAllText(path)).SavedNodeSceneToNodeScene();
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
        public static string GetPath(NodeScene nodeScene)
        {
            return abilitespath + "/" + CleanNodeName(nodeScene.sceneName) + "/" + nodeScene.id + ".ability";
        }
        public static string GetPath(SavedNodeScene nodeScene)
        {
            return abilitespath + "/" + CleanNodeName(nodeScene.sceneName) + "/" + nodeScene.id + ".ability";
        }
        public static SavedNodeScene DeserializeAbility(string json)
        {
            var settings = new Newtonsoft.Json.JsonSerializerSettings()
            {
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore,
            };
            var n = Newtonsoft.Json.JsonConvert.DeserializeObject<SavedNodeScene>(json, settings);
            SavedNodeSceneStorer.savedNodeScenes.Add(n);
            return n;
        }
        public static Sprite GetSprite(NodeScene nodeScene)
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
            else if (pepsis.ContainsKey(nodeScene.sceneImage))
            {
                return pepsis[nodeScene.sceneImage];
            }
            return null;
        }
        public static Sprite GetSprite(SavedNodeScene nodeScene)
        {
            try
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
                else if (pepsis.ContainsKey(nodeScene.sceneImage))
                {
                    return pepsis[nodeScene.sceneImage];
                }
            }
            catch { }
            return null;
        }
        public static bool IsAbilityWritten(NodeScene nodeScene)
        {
            return Directory.Exists(Main.abilitespath + "/" + Main.CleanNodeName(nodeScene.sceneName));
        }
        public static bool IsAbilityWritten(SavedNodeScene nodeScene)
        {
            return Directory.Exists(Main.abilitespath + "/" + Main.CleanNodeName(nodeScene.sceneName));
        }
        public static StreamedSceneManager sceneManager;
        public static Dictionary<string, NodeBlueprint> nodeDatabase = new Dictionary<string, NodeBlueprint>();
        public static List<NodeScene> nodeScenes = new List<NodeScene>();
        public static string abilitespath = path + "/Abilites";
    }
}