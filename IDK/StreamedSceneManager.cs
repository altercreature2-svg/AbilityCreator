using BitCode.Debug.Commands;
using IDK.Node_Related_Scripts.connection_stuff;
using IDK.NodeScripts;
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

namespace IDK
{
    public class StreamedSceneManager : MonoBehaviour
    {
        public static LegacyNodeScene currentscene;
        private int CurrentEditMenuState = 0;
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
        public void EnterNodeScene(int nodesceneIndex)
        {
            TABSSceneManager.LoadScene("Assets/Scenes/AbilityCreator.unity");
            //ShowAllObjects(FindObjectOfType<Canvas>().transform);
            Debug.Log("NodeSceneIndex:" + nodesceneIndex);
            currentscene = AbilityCreator.nodeScenes[nodesceneIndex];
            StartCoroutine(EnterNodeSceneEnumerator());
        }
        public void EnterNewNodeScene()
        {
            TABSSceneManager.LoadScene("Assets/Scenes/AbilityCreator.unity");
            
        }
        public void EnterNodeChanger()
        {
            LegacyNodeScene[] nodeScenes = FindObjectsOfType<LegacyNodeScene>();

            for (int i = 0; i < nodeScenes.Length; i++)
            {
                if (!nodeScenes[i].isFinal)
                    Destroy(nodeScenes[i]);
            }
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

            foreach (LegacyNodeScene nodeScene in AbilityCreator.nodeScenes)
            {
                if (!nodeScene)
                {
                    continue;
                }

                try
                {
                    GameObject button = Instantiate(GameObject.Find("Ability"));
                    button.name = nodeScene.id.ToString();
                    button.transform.SetParent(GameObject.Find("Ability").transform.parent, false);
                    void Move()
                    {
                        ServiceLocator.GetService<SoundPlayer>().PlaySoundEffect("UI/Click", 0.9f, Camera.current.transform.position);
                        MoveMenu(nodeScene);
                    }
                    button.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => Move());

                    button.GetComponentInChildren<TextMeshProUGUI>().text = nodeScene.sceneName;

                    button.transform.Find("Icon").GetComponent<Image>().sprite = AbilityCreator.GetSprite(nodeScene);
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
                string buttonName = AbilityCreator.assetManager.assets.Keys.ElementAt(i).GetName();
                button.GetComponentInChildren<TextMeshProUGUI>().text = buttonName;
                button.GetComponent<Button>().onClick.AddListener(() => txt.text = File.ReadAllText(Path.Combine(AbilityCreator.path, buttonName + ".txt")));
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
            VirtualNodeScene savedNodeScene = AbilityCreator.DeserializeAbility(bundledAbility.abilityData);
            NodeManager.WriteAbility(savedNodeScene, bundledAbility.abilityData, true, true);
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

        private void MoveMenu(LegacyNodeScene scene)
        {
            Time.timeScale = 1;
            if (CurrentEditMenuState == 0)
            {
                CurrentEditMenuState = 1;
                GameObject.Find("EditMenu").GetComponent<CodeAnimation>().setFirstFrame = true;
                GameObject.Find("EditMenu").GetComponent<CodeAnimation>().PlayIn();
            }
            else if (CurrentEditMenuState == 1)
            {
                CurrentEditMenuState = 0;
                GameObject.Find("EditMenu").GetComponent<CodeAnimation>().setFirstFrame = false;
                GameObject.Find("EditMenu").GetComponent<CodeAnimation>().PlayOut();

            }
            GameObject.Find("EditMenuText").GetComponent<TextMeshProUGUI>().text = scene.sceneName;
            GameObject.Find("EditButton").GetComponent<Button>().onClick.RemoveAllListeners();
            GameObject.Find("EditButton").GetComponent<Button>().onClick.AddListener(() => EnterNodeScene(scene.NodeSceneIndex));
            GameObject.Find("EditButton").GetComponent<Button>().onClick.AddListener(() => ServiceLocator.GetService<SoundPlayer>().PlaySoundEffect("UI/Click", 0.9f, Camera.current.transform.position));

            butto.GetComponent<Button>().onClick.RemoveAllListeners();
            butto.GetComponent<Button>().onClick.AddListener(() => DeleteScene(scene));
            butto.GetComponent<Button>().onClick.AddListener(() => ServiceLocator.GetService<SoundPlayer>().PlaySoundEffect("UI/Click", 0.9f, Camera.current.transform.position));




        }
        public void DeleteScene(LegacyNodeScene scene)
        {
            string path = Directory.GetParent(AbilityCreator.GetPath(scene)).FullName;
            Directory.Delete(path, true);
            GameObject buttonToDestroy = GameObject.Find(scene.id.ToString());
            if (AbilityCreator.nodeScenes.Contains(scene))
                AbilityCreator.nodeScenes.Remove(scene);
            if (AbilityCreator.abilites.ContainsKey(scene.sceneName))
                AbilityCreator.abilites.Remove(scene.sceneName);
            Destroy(buttonToDestroy);
            MoveMenu(scene);
            Destroy(scene);
            AbilityCreator.Reload();

        }

        public IEnumerator EnterNodeSceneEnumerator()
        {
            Time.timeScale = 1;
            //THIS IS NEEDED!!! ok (thx me btw) np man we should really make a better system for this ngl
            CampaignPlayerDataHolder.SetToNone();

            yield return new WaitUntil(() => GameObject.Find("nodesScaler") != null);
            GameObject.Find("AbilitySaveButton").AddComponent<SetSiblingIndex>().Ind = 99;
            GameObject.Find("AbilitySaveButton").transform.position += new Vector3(0, 0, -10);
            GameObject.Find("Quitbutton").AddComponent<SetSiblingIndex>().Ind = 99;
            GameObject.Find("Quitbutton").transform.position += new Vector3(0, 0, -10);
            if (currentscene == null)
            {
                Debug.Log("null nodescene!");
                yield break;
            }
            // Objectefize nodescenes
            // what???
            
            Dictionary<LegacySavedNode, NodeComponent> nodes = new Dictionary<LegacySavedNode, NodeComponent>();
            for (int i = 0; i < currentscene.everyNode.Length; i++)
            {
                try
                {
                    NodeBlueprint nodeBlueprint = currentscene.everyNode[i].Blueprint;
                    List<string> fields = currentscene.everyNode[i].fields;
                    NodeComponent node = nodeBlueprint.Spawn();
                    node.corispondingNode = currentscene.everyNode[i];
                    currentscene.everyNode[i].corispondingNode = node;
                    node.transform.position = currentscene.everyNode[i].position;
                    for (int i2 = 0; i2 < nodeBlueprint.fields.Count; i2++)
                    {
                        Debug.Log($"Field {fields[i2]}");
                        try
                        {
                            if (fields[i2] == "")
                                continue;
                            if (fields[i2] != "_")
                                node.transform.Find("Fields").Find(nodeBlueprint.fields[i2].name).GetComponentInChildren<NodeField>().Value = fields[i2];
                            else
                                node.transform.Find("Fields").Find(nodeBlueprint.fields[i2].name);
                        }
                        catch { }
                    }
                    nodes.Add(currentscene.everyNode[i], node);
                }
                catch { }
            }
            for (int i = 0; i < currentscene.everyNode.Length; i++)
            {
                NodeComponent node = nodes[currentscene.everyNode[i]];
                foreach (NodeComponent.LegacyConnection connection in currentscene.everyNode[i].connections)
                {
                    try
                    {
                        int index = node.Connections.Keys.ToList().FindIndex(n => n == connection.connectionsType);
                        node.Connections.Values.ElementAt(index).connected = nodes[connection.savedNode].GetComponentsInChildren<NodeConnector>().ToList().Find(n => n.CanConnect(node.Connections.Values.ToArray()[index]));
                    }
                    catch 
                    {
                        node.Connections.Clear();
                    }
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