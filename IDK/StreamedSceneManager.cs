using BitCode.Debug.Commands;
using AC.Node_Related_Scripts.connection_stuff;
using AC.NodeScripts;
using Landfall.TABS;

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TFBGames;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using IDK.AbilityHandling;
using IDK.Node_Related_Scripts;
using IDK.AbilityHandling.Saving;
using AC.Node_Related_Scripts.SavingStuff;

namespace AC
{
    public class StreamedSceneManager : MonoBehaviour
    {
        public static VirtualNodeScene currentScene;
        private int currentEditMenuState = 0;
        private GameObject butto;
        private void Awake()
        {
            SceneManager.sceneLoaded += IDKOnSceneLoaded;
        }
        public static void ShowAllObjects(Transform transform)
        {
            if (transform.GetComponent<NodeComponent>())
                return;
            if (transform.gameObject.scene != SceneManager.GetActiveScene())
                return;
            if (transform.IsChildOf(GameObject.Find("TestPage").transform))
                return;
            transform.gameObject.SetActive(true);
            if (transform.childCount == 0)
                return;

            foreach (Transform child in transform.transform)
            {

                ShowAllObjects(child);
            }
        }
        public void EnterNodeScene(VirtualNodeScene virtualNodeScene)
        {
            TABSSceneManager.LoadScene("Assets/Scenes/AbilityCreator.unity");
            StartCoroutine(EnterNodeSceneEnumerator());
        }
        public void EnterNewNodeScene()
        {
            TABSSceneManager.LoadScene("Assets/Scenes/AbilityCreator.unity");
        }
        public void EnterNodeChanger()
        {
            TABSSceneManager.LoadScene("Assets/Scenes/Ability Selector.unity");
            Debug.Log("bananana :)");
            Time.timeScale = 1;
            StartCoroutine(EnterNodeChangerEnumerator());
            Time.timeScale = 1;
        }
        private IEnumerator EnterNodeChangerEnumerator()
        {
            yield return new WaitUntil(() => GameObject.Find("Ability") != null);
            yield return new WaitUntil(() => GameObject.Find("New Ability") != null);
            GameObject.Find("New Ability").GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => EnterNewNodeScene());

            foreach (AbilityManager.AbilityData abilityData in AbilityManager.Instance.Abilities)
            {
                if (!abilityData.abilityGo) continue;
                try
                {
                    GameObject button = Instantiate(GameObject.Find("Ability"));
                    button.name = abilityData.nodeScene.abilityID.ToString();
                    button.transform.SetParent(GameObject.Find("Ability").transform.parent, false);
                    void Move()
                    {
                        ServiceLocator.GetService<SoundPlayer>().PlaySoundEffect("UI/Click", 0.9f, Camera.current.transform.position);
                        MoveMenu(abilityData.nodeScene);
                    }
                    button.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => Move());

                    button.GetComponentInChildren<TextMeshProUGUI>().text = abilityData.nodeScene.abilityName;

                    if (FileManager.SearchForSpirte(abilityData.abilityName, out Sprite sprite))
                        button.transform.Find("Icon").GetComponent<Image>().sprite = sprite;
                }
                catch { }


            }
            GameObject.Find("Ability").SetActive(false);
            GameObject DeleteMenu = GameObject.Find("DeleteMenu");
            butto = GameObject.Find("FuckingDeleteButton");
            butto.transform.parent.gameObject.SetActive(false);
            GameObject.Find("GoMainMenu").GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => TABSSceneManager.LoadMainMenu());
            GameObject.Find("Guides").GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => EnterGuides());
            GameObject.Find("Unit's abilites").GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => EnterUnitAbilites());
            GameObject.Find("ScrollUP").GetComponent<Button>().onClick.AddListener(() => ((RectTransform)GameObject.Find("Abilites").transform).anchoredPosition -= Vector2.up * 75);
            GameObject.Find("ScrollDOWN").GetComponent<Button>().onClick.AddListener(() => ((RectTransform)GameObject.Find("Abilites").transform).anchoredPosition += Vector2.up * 75);
            Time.timeScale = 1;

        }
        private void EnterGuides()
        {
            TABSSceneManager.LoadScene("Assets/Scenes/Guides.unity");
            StartCoroutine(EnterGuidesInt());

        }
        public IEnumerator EnterGuidesInt()
        {
            yield return new WaitUntil(() => GameObject.Find("TABS2"));
            GameObject tabs2 = GameObject.Find("TABS2");
            TMP_InputField txt = GameObject.Find("txt").GetComponent<TMP_InputField>();
            Button BaseButton = GameObject.Find("Base").GetComponent<Button>();
            for (int i = 0; i < AbilityCreator.assetManager.assets.Count; i++)
            {
                GameObject button = Instantiate(BaseButton.gameObject, BaseButton.transform.parent);
                string buttonName = AbilityCreator.assetManager.assets.Keys.ToArray()[i];
                button.GetComponentInChildren<TextMeshProUGUI>().text = buttonName;
                button.GetComponent<Button>().onClick.AddListener(() => txt.text = File.ReadAllText(Path.Combine(FilePaths.AbilityCreatorPath, buttonName + ".txt")));
            }
            Destroy(BaseButton.gameObject);
            GameObject.Find("GoMainMenu").GetComponent<Button>().onClick.AddListener(() => EnterNodeChanger());
            tabs2.SetActive(false);
            Button spawnButton = GameObject.Find("SpawnButton").GetComponent<Button>();
            TMP_Dropdown spawnType = GameObject.Find("TypeDropdown").GetComponent<TMP_Dropdown>();
            TMP_InputField spawnID = GameObject.Find("TextField").GetComponent<TMP_InputField>();
            Transform spawnPos = GameObject.Find("SpawnPos").transform;
            spawnPos.transform.forward = -spawnPos.transform.up;
            Object lastSpawnedObject = null;
            spawnButton.onClick.RemoveAllListeners();
            spawnButton.onClick.AddListener(() => Spawn());
            void Spawn()
            {
                if (lastSpawnedObject != null)
                    Destroy(lastSpawnedObject);
                Unit[] units = FindObjectsOfType<Unit>();
                for (int i = 0; i < units.Length; i++)
                {
                    Destroy(units[i].gameObject);
                }
                string dropdownValue = spawnType.options[spawnType.value].text;
                try
                {
                    if (dropdownValue == "Unit")
                    {
                        UnitBlueprint unitBlueprint = AbilityCreator.units[spawnID.text];
                        GameObject unit = unitBlueprint.Spawn(spawnPos.position, Quaternion.Euler(0, 180, 0), Team.Red)[0].transform.root.gameObject;
                        unit.transform.position = spawnPos.position;
                        lastSpawnedObject = unit;
                    }
                    if (dropdownValue == "Explosion")
                    {
                        GameObject gameObject = Instantiate(AbilityCreator.explosions[spawnID.text]);
                        gameObject.transform.position = spawnPos.position;
                        lastSpawnedObject = gameObject;
                    }
                    if (dropdownValue == "Effect")
                    {
                        UnitBlueprint unitBlueprint = AbilityCreator.units["Squire"];
                        GameObject unit = unitBlueprint.Spawn(spawnPos.position, Quaternion.Euler(0, 180, 0), Team.Red)[0].transform.root.gameObject;
                        unit.transform.position = spawnPos.position;
                        lastSpawnedObject = unit;

                        GameObject effect = Object.Instantiate(AbilityCreator.effects[spawnID.text], unit.transform);
                        if (effect.GetComponent<UnitEffectBase>())
                        {
                            effect.GetComponent<UnitEffectBase>().DoEffect();
                        }
                    }
                    if (dropdownValue == "Sound")
                    {
                        var sound = ServiceLocator.GetService<SoundPlayer>().soundBank.GetSoundEffect(spawnID.text.Replace("&", "/"));
                        int random = Random.Range(0, sound.clipTypes.Length);
                        int random2 = Random.Range(0, sound.clipTypes[random].clips.Length);
                        spawnPos.gameObject.AddComponent<AudioSource>().clip = sound.clipTypes[random].clips[random2];
                        spawnPos.gameObject.GetComponent<AudioSource>().Play();
                        lastSpawnedObject = spawnPos.gameObject.GetComponent<AudioSource>();
                        Debug.Log($"playing sound:{sound.clipTypes[random].clips[random2]}");
                    }
                    if (dropdownValue == "Projectile")
                    {
                           
                        GameObject projectilePrefab = AbilityCreator.projectiles[spawnID.text];
                        lastSpawnedObject = SmartProjectileSpawner.SpawnProjectile(projectilePrefab, spawnPos.gameObject, null, spawnPos, spawnPos, 0);
                    }
                    if (dropdownValue == "Sprite")
                    {
                        GameObject image = new GameObject("PreviewSprite");
                        
                        if (!image.GetComponent<RectTransform>())
                            image.AddComponent<RectTransform>();
                        image.transform.SetParent(spawnButton.transform.parent);
                        image.GetComponent<RectTransform>().SetWidthAndHeight(.7f, .7f);
                        image.AddComponent<Image>().sprite = AbilityCreator.sprites[spawnID.text];
                        lastSpawnedObject = image;
                    }
                    if (dropdownValue == "Particle")
                    {
                        GameObject prefab = AbilityCreator.particles[spawnID.text];
                        GameObject particle = Instantiate(prefab);
                        particle.GetComponent<ParticleSystem>().Play();
                        lastSpawnedObject = particle;
                    }
                    if (dropdownValue == "Weapon")
                    {
                        GameObject prefab = AbilityCreator.weapons[spawnID.text];
                        UnitBlueprint unitBlueprint = Instantiate(AbilityCreator.units["Squire"]);
                        unitBlueprint.m_props = new GameObject[0];
                        unitBlueprint.RightWeapon = prefab;
                        GameObject unit = unitBlueprint.Spawn(spawnPos.position, Quaternion.Euler(0, 180, 0), Team.Red)[0].transform.root.gameObject;
                        unit.transform.position = spawnPos.position;
                        lastSpawnedObject = unit;
                        
                    }
                    if (dropdownValue == "Prop")
                    {
                        GameObject prefab = AbilityCreator.assetManager.GetAsset<GameObject>("prop", spawnID.text);
                        UnitBlueprint unitBlueprint = Instantiate(AbilityCreator.units["Squire"]);
                        unitBlueprint.m_props = new GameObject[] { prefab};
                        unitBlueprint.RightWeapon = null;
                        GameObject unit = unitBlueprint.Spawn(spawnPos.position, Quaternion.Euler(0, 180, 0), Team.Red)[0].transform.root.gameObject;
                        unit.transform.position = spawnPos.position;
                        lastSpawnedObject = unit;
                    }
                }
                catch
                {

                }
            }
        }

        private void EnterUnitAbilites()
        {
            TABSSceneManager.LoadScene("Assets/Scenes/Bundled units.unity");
            StartCoroutine(EnterUnitAbilitesInt());

        }
        public static void AddBundledAbility(BundledAbilitesManager.BundledAbility bundledAbility)
        {
            VirtualNodeScene nodeScene = SaveableHelper.Load<VirtualNodeScene>(bundledAbility.abilityData);
            FileManager.WriteAbility(nodeScene.abilityName, bundledAbility.abilityData);
            BundledAbilitesManager.bundledAbilities.Remove(bundledAbility);
            MyModalPanel.queue.Remove(bundledAbility);
            
        }
        public IEnumerator EnterUnitAbilitesInt()
        {
            yield return new WaitUntil(() => GameObject.Find("ContentButton"));
            GameObject contentButton = GameObject.Find("ContentButton");
            for (int i = 0; i < BundledAbilitesManager.bundledAbilities.Count; i++)
            {

                BundledAbilitesManager.BundledAbility bundledAbility = BundledAbilitesManager.bundledAbilities[i];
                GameObject abilityButton = Instantiate(contentButton, contentButton.transform.parent);
                abilityButton.transform.Find("UnitIcon").GetComponent<Image>().sprite = bundledAbility.sprite;
                abilityButton.transform.Find("AbilityName").GetComponent<TextMeshProUGUI>().text = bundledAbility.abilityName;
                abilityButton.transform.Find("UnitName").GetComponent<TextMeshProUGUI>().text = bundledAbility.unitName;
                abilityButton.transform.Find("Load").GetComponent<Button>().onClick.AddListener(() =>
                {
                    Destroy(abilityButton);
                    AddBundledAbility(bundledAbility);
                });
            }
            GameObject.Find("GoMainMenu").GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => TABSSceneManager.LoadMainMenu());
            contentButton.SetActive(false);
        }

        private void MoveMenu(VirtualNodeScene scene)
        {
            Time.timeScale = 1;
            if (currentEditMenuState == 0)
            {
                currentEditMenuState = 1;
                GameObject.Find("EditMenu").GetComponent<CodeAnimation>().setFirstFrame = true;
                GameObject.Find("EditMenu").GetComponent<CodeAnimation>().PlayIn();
            }
            else if (currentEditMenuState == 1)
            {
                currentEditMenuState = 0;
                GameObject.Find("EditMenu").GetComponent<CodeAnimation>().setFirstFrame = false;
                GameObject.Find("EditMenu").GetComponent<CodeAnimation>().PlayOut();

            }
            GameObject.Find("EditMenuText").GetComponent<TextMeshProUGUI>().text = scene.abilityName;
            GameObject.Find("EditButton").GetComponent<Button>().onClick.RemoveAllListeners();
            GameObject.Find("EditButton").GetComponent<Button>().onClick.AddListener(() => EnterNodeScene(scene));
            GameObject.Find("EditButton").GetComponent<Button>().onClick.AddListener(() => ServiceLocator.GetService<SoundPlayer>().PlaySoundEffect("UI/Click", 0.9f, Camera.current.transform.position));

            butto.GetComponent<Button>().onClick.RemoveAllListeners();
            butto.GetComponent<Button>().onClick.AddListener(() => DeleteScene(scene));
            butto.GetComponent<Button>().onClick.AddListener(() => ServiceLocator.GetService<SoundPlayer>().PlaySoundEffect("UI/Click", 0.9f, Camera.current.transform.position));




        }
        public void DeleteScene(VirtualNodeScene scene)
        {
            AbilityManager.Instance.DeregisterAbility(scene);
        }

        public IEnumerator EnterNodeSceneEnumerator()
        {
            Time.timeScale = 1;
            //THIS IS NEEDED!!! ok (thx me btw) np man we should really make a better system for this ngl (no fuck you)
            CampaignPlayerDataHolder.SetToNone();

            yield return new WaitUntil(() => GameObject.Find("nodesScaler") != null);
            GameObject.Find("AbilitySaveButton").AddComponent<SetSiblingIndex>().Ind = 99;
            GameObject.Find("AbilitySaveButton").transform.position += new Vector3(0, 0, -10);
            GameObject.Find("Quitbutton").AddComponent<SetSiblingIndex>().Ind = 99;
            GameObject.Find("Quitbutton").transform.position += new Vector3(0, 0, -10);
            if (currentScene == null)
            {
                Debug.Log("null nodescene!");
                yield break;
            }
            // Objectefize nodescenes
            // what???
            
            Dictionary<VirtualNodeScene, NodeComponent> nodes = new Dictionary<VirtualNodeScene, NodeComponent>(currentScene.savedObjects.Length);
            for (int i = 0; i < currentScene.savedObjects.Length; i++)
            {
                ISaveable saveable = currentScene.savedObjects[i];
                if (!(saveable is VirtualNode virtualNode))
                    continue;
                NodeBlueprint nodeBlueprint = AbilityCreator.nodeDatabase[virtualNode.nodeBlueprint];
                NodeComponent nodeComponent = nodeBlueprint.Spawn();
                NodeField[] nodeFields = nodeComponent.GetComponentsInChildren<NodeField>();
                for (int j = 0; j < Mathf.Min(nodeFields.Length, virtualNode.fields.Count); j++)
                {
                    nodeFields[i].Value = virtualNode.fields[i].value;
                }
            }



        }
        public IEnumerator IDKONSCENELOADEDFUCK(Scene scene, LoadSceneMode loadSceneMode)
        {
            NodeManager[] nodeManagers = FindObjectsOfType<NodeManager>();
            for (int i = 0; i < nodeManagers.Length; i++)
            {
                Destroy(nodeManagers[i].gameObject);
            }
            yield return new WaitUntil(() => TABSSceneManager.IsLoading == false);
            Debug.Log("Loaded scene:" + scene.name);


            bool found = false;

            for (int i = 0; i < BundleManager.scenes.GetAllScenePaths().Length; i++)
            {
                if (scene.path == BundleManager.scenes.GetAllScenePaths()[i])
                {
                    found = true;
                    Time.timeScale = 1;
                    ServiceLocator.GetService<LoadingScreenHandler>().HideLoadingScreen(null);
                    Button[] buttons = FindObjectsOfType<Button>();
                    for (int f = 0; f < buttons.Length; f++)
                    {
                        buttons[f].onClick.AddListener(() => ServiceLocator.GetService<SoundPlayer>().PlaySoundEffect("UI/Click", 0.9f, Camera.current.transform.position));
                    }
                }
            }
            if (scene.path == "Assets/Scenes/AbilityCreator.unity")
            {
                
                NodeManager nodemanager = new GameObject("Node Manager").AddComponent<NodeManager>();
                nodemanager.gameObject.name = "NodeManager";
                DontDestroyOnLoad(nodemanager.gameObject);
            }

            if (found == false)
            {
                FindObjectOfType<NodeManager>()?.gameObject?.Destroy(null);
            }

        }

        public void IDKOnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            StartCoroutine(IDKONSCENELOADEDFUCK(scene, loadSceneMode));
        }
        public void ValueChanged(bool value)
        {
            GlobalSettings.SaveAbilites = value;
        }
    }
}