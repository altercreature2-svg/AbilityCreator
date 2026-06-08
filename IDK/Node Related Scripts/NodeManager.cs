using DM;
using HarmonyLib;
using IDK.Node_Related_Scripts;
using IDK.Node_Related_Scripts.connection_stuff;
using IDK.Node_Related_Scripts.ConnectionStuff;
using IDK.Node_Related_Scripts.Field_stuff;
using IDK.Node_Related_Scripts.SavingStuff;
using Landfall.TABC;
using Landfall.TABS;
using Landfall.TABS.GameState;
using Landfall.TABS.RuntimeCleanup;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking.Types;
using UnityEngine.UI;




namespace IDK
{
    public class NodeManager : MonoBehaviour
    {

        public NodeComponent[] SelectedNodes
        {
            get
            {
                NodeComponent[] array = FindObjectsOfType<UnityEngine.UI.Outline>().Where(n => n.transform.parent.GetComponent<NodeComponent>()).Select(n => n.transform.parent.GetComponent<NodeComponent>()).ToArray();
                return array;
            }
        }
        public Stack<EditorAction> editorActions = new Stack<EditorAction>();
        public bool CanMove = true;
        public int NodesceneIndex = 0;
        public int nodeid = 0;
        private GameObject RightClickMenu;
        private GameObject SaveMenu;
        private GameObject QuitMenu;
        private GameObject nodeScaler;
        public UnitBlueprint unitBlueprint1;
        public UnitBlueprint unitBlueprint2;
        public GameObject baseButton;
        public Vector3 scale = Vector3.one;
        private bool buttonState;
        public bool canScroll = true;
        public List<NodeComponent> nodes = new List<NodeComponent>();
        public Coroutine unitSearchCoroutine;
        private GameObject m_ability;
        private GameObject tutorial;
        private float zoom;
        public GameObject[] buttonPool;
        public int poolSize = 50;
        public ItemList itemList;
        public GameObject Ability
        {
            get
            {
                if (!m_ability)
                    RefreshAbility();
                return m_ability;
            }
        }
        public void PushEditorAction(EditorAction editorAction)
        {
            editorActions.Push(editorAction);
        }
        public void UndoLastEditorAction()
        {
            editorActions.Peek().Undo();
            editorActions.Pop();
        }
        private void Awake()
        {
            if (FindObjectsOfType<NodeManager>().Where(n => n != this).Count() != 0)
                Destroy(this);
            StartCoroutine(Awakeinternal());
        }
        public void RefreshAbility()
        {
            m_ability = AbilityCreator.CreateAbility(BuildNodeScene());
        }
        public void DuplicateSelectedNode(NodeComponent node)
        {
            NodeComponent duplicate = AbilityCreator.nodeDatabase[node.nodeBlueprint.key].Spawn();
            NodeField[] fields = node.GetComponentsInChildren<NodeField>();
            duplicate.SetFields(fields.Select(n => n.Value).ToArray());
            duplicate.transform.position = node.transform.position + new Vector3(200, -200);
        }
        private void Update()
        {

            if (Input.GetKeyDown(KeyCode.Escape))
                itemList.itemsList.SetActive(false);

            if (Input.GetKey(KeyCode.LeftControl))
            {
                if (Input.GetKeyDown(KeyCode.D))
                    for (int i = 0; i < SelectedNodes?.Length; i++)
                        DuplicateSelectedNode(SelectedNodes[i]);
                if (Input.GetKeyDown(KeyCode.W))
                    UndoLastEditorAction();
            }
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                tutorial.SetActive(false);
                if (RightClickMenu.activeSelf == false)
                {
                    RightClickMenu.SetActive(true);
                }
                else
                {
                    RightClickMenu.SetActive(false);
                }
            }

            if (Input.GetKeyDown(KeyCode.Delete))
                for (int i = 0; i < SelectedNodes?.Length; i++)
                    SelectedNodes[i].Remove();


            if (Input.mouseScrollDelta[1] != 0)
            {
                if (canScroll)
                {
                    if (nodeScaler == null)
                    {
                        if (GameObject.Find("nodesScaler") == null)
                        {
                            return;
                        }
                        else
                        {
                            nodeScaler = GameObject.Find("nodesScaler");
                        }
                    }
                    GameObject creationPage = GameObject.Find("AbilityCreationPage");
                    GameObject scaler = new GameObject("Scaler");
                    scaler.transform.SetParent(creationPage.transform);
                    scaler.transform.localScale = Vector3.one;
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(FindObjectOfType<Canvas>().transform as RectTransform, Input.mousePosition, Camera.current, out Vector2 localPoint);
                    scaler.transform.position = localPoint;

                    nodeScaler.transform.SetParent(scaler.transform);

                    scaler.transform.localScale += new Vector3(Input.mouseScrollDelta[1] / 15, Input.mouseScrollDelta[1] / 15, Input.mouseScrollDelta[1] / 15);
                    scale += new Vector3(Input.mouseScrollDelta[1] / 15, Input.mouseScrollDelta[1] / 15, Input.mouseScrollDelta[1] / 15);
                    zoom *= (Input.mouseScrollDelta[1]);


                    nodeScaler.transform.SetParent(creationPage.transform);
                    Destroy(scaler);
                }
            }
        }
        void Zoom(float zoom)
        {
            foreach (NodeComponent item in FindObjectsOfType<NodeComponent>())
            {
                item.transform.localScale = Vector3.one * zoom;
            }
            this.zoom = zoom;
        }

        private void GenerateTabMenu()
        {
            GameObject buttonPrefab = RightClickMenu.transform.Find("TABSContent").Find("TAB1").Find("NodeButton").gameObject;
            GameObject tabButtonPrefab = RightClickMenu.transform.Find("TABS").Find("TAB1").gameObject;
            GameObject tabPrefab = RightClickMenu.transform.Find("TABSContent").Find("TAB1").gameObject;
            List<string> tabs = new List<string>();
            NodeBlueprint[] allBlueprints = AbilityCreator.nodeDatabase.Values.ToArray();
            List<GameObject> tabsButtons = new List<GameObject>();
            List<GameObject> tabsObjects = new List<GameObject>();
            for (int i = 0; i < allBlueprints.Length; i++)
            {
                if (!tabs.Contains(allBlueprints[i].tab))
                {
                    tabs.Add(allBlueprints[i].tab);
                }
            }
            for (int i = 0; i < tabs.Count; i++)
            {
                GameObject tabButton = Instantiate(tabButtonPrefab, tabButtonPrefab.transform.parent);
                tabButton.GetComponentInChildren<TextMeshProUGUI>().text = tabs[i];
                tabButton.name = $"TAB{i}";
                tabsButtons.Add(tabButton);
            }
            for (int i = 0; i < tabsButtons.Count; i++)
            {
                string TABName = tabsButtons[i].GetComponentInChildren<TextMeshProUGUI>(true).text;
                int referenceDestroyer;
                referenceDestroyer = 0 + i;
                GameObject tab = Instantiate(tabPrefab, tabPrefab.transform.parent);
                NodeBlueprint[] nodeBlueprints = GetAllNodeblueprintsWithTAB(TABName);
                Destroy(tab.transform.GetChild(0).gameObject);
                for (int i2 = 0; i2 < tabs.Count * 2; i2++)
                {
                    int referenceDestroyer2;
                    referenceDestroyer2 = 0 + i2;
                    if (i2 <= nodeBlueprints.Where(n => !n.obselete).Count() - 1)
                    {
                        if (nodeBlueprints[i2].obselete)
                            continue;
                        GameObject button = Instantiate(buttonPrefab, tab.transform);
                        button.GetComponentInChildren<TextMeshProUGUI>(true).text = nodeBlueprints[referenceDestroyer2].Name;
                        button.GetComponentInChildren<Button>(true).onClick.AddListener(() =>
                        {
                            NodeComponent node = nodeBlueprints[referenceDestroyer2].Spawn();
                            editorActions.Push(new CreateNodeAction(node));
                            RightClickMenu.SetActive(false);
                        });
                    }
                    else
                    {
                        GameObject button = Instantiate(buttonPrefab, tab.transform);
                        button.GetComponentInChildren<TextMeshProUGUI>(true).text = "";
                    }
                }
                tab.name = $"TAB{i}";


                tabsObjects.Add(tab);
                tabsButtons[i].GetComponentInChildren<Button>(true).onClick.AddListener(() =>
                {
                    for (int i2 = 0; i2 < tabsObjects.Count; i2++)
                    {
                        tabsObjects[i2].SetActive(false);
                    }
                    DeveloperLogger.Log("Name:" + tabsButtons[referenceDestroyer].name);
                    GameObject gameObject = tabsObjects.Find(n => n.name == tabsButtons[referenceDestroyer].name);
                    DeveloperLogger.Log(gameObject);
                    gameObject.SetActive(true);
                });
            }
            DeveloperLogger.Log("done");
            //Generate Tabs

            for (int i = 0; i < tabsObjects.Count; i++)
            {
                tabsObjects[i].SetActive(false);
            }
            Destroy(tabPrefab);
            Destroy(buttonPrefab);
            Destroy(tabButtonPrefab);
            RightClickMenu.SetActive(false);
        }
        NodeBlueprint[] GetAllNodeblueprintsWithTAB(string tab)
        {
            return AbilityCreator.nodeDatabase.Values.Where(n => n.tab == tab).ToArray();
        }
        private IEnumerator Awakeinternal()
        {
            yield return new WaitUntil(() => TABSSceneManager.IsLoading == false);
            yield return new WaitUntil(() => GameObject.Find("Drawing Board") != null);
            yield return new WaitUntil(() => GameObject.Find("Hitbox Movement") != null);
            for (int i = 0; i < 3; i++)
                yield return null;
            unitBlueprint1 = Instantiate(AbilityCreator.units["Clubber"]);
            unitBlueprint1.m_props = new GameObject[0];
            unitBlueprint1.RightWeapon = null;
            unitBlueprint2 = Instantiate(AbilityCreator.units["Clubber"]);
            unitBlueprint2.m_props = new GameObject[0];
            unitBlueprint2.RightWeapon = null;
            GameObject drawingBoard = GameObject.Find("Drawing Board");
            GameObject hitboxMovement = GameObject.Find("Hitbox Movement");
            StreamedSceneManager.ShowAllObjects(drawingBoard.transform);
            hitboxMovement.AddComponent<NodeSceneMovement>().dragRectTransform = hitboxMovement.GetComponent<RectTransform>();
            RightClickMenu = GameObject.Find("RightClickMenu");
            RightClickMenu.AddComponent<SetSiblingIndex>().Ind = 98;
            SaveMenu = GameObject.Find("FuckingSaveButton");
            SaveMenu.GetComponent<Button>().onClick.AddListener(() => SaveNodeScene());
            QuitMenu = GameObject.Find("FuckingQuitButton");
            QuitMenu.GetComponent<Button>().onClick.AddListener(() => AbilityCreator.sceneManager.EnterNodeChanger());
            RightClickMenu = GameObject.Find("RightClickMenu");
            GameObject Nodes = GameObject.Find("nodesScaler").gameObject;
            tutorial = GameObject.Find("TabInfo");
            if (StreamedSceneManager.currentscene != null)
            {
                nodeid = StreamedSceneManager.currentscene.id;
            }

            Nodes.transform.parent = GameObject.Find("AbilitySaveButton").transform.parent;
            GenerateTabMenu();
            if (StreamedSceneManager.currentscene != null)
            {
                GameObject.Find("AbName").GetComponent<TMP_InputField>().text = StreamedSceneManager.currentscene.sceneName;
                GameObject.Find("Description").GetComponent<TMP_InputField>().text = StreamedSceneManager.currentscene.sceneDescription;
                GameObject.Find("Imagename").GetComponent<TMP_InputField>().text = StreamedSceneManager.currentscene.sceneImage;
            }
            GameObject.Find("AbilitySaveButton").GetComponent<Button>().onClick.AddListener(() => ToggleCanMove(0));
            GameObject.Find("Quitbutton").GetComponent<Button>().onClick.AddListener(() => ToggleCanMove(0));
            GameObject.Find("Cancel").GetComponent<Button>().onClick.AddListener(() => ToggleCanMove(1));
            GameObject.Find("Cancel2").GetComponent<Button>().onClick.AddListener(() => ToggleCanMove(1));
            SaveMenu.transform.parent.parent.gameObject.AddComponent<SetSiblingIndex>().Ind = 99;
            SaveMenu.transform.parent.parent.gameObject.SetActive(false);
            QuitMenu.transform.parent.parent.gameObject.AddComponent<SetSiblingIndex>().Ind = 99;
            QuitMenu.transform.parent.parent.gameObject.SetActive(false);
            Nodes.transform.localScale = Vector3.one;
            GameObject.Find("TestUnit").GetComponent<Button>().onClick.AddListener(() => DoInAnimation());
            GameObject.Find("TestUnitButton").GetComponent<Button>().onClick.AddListener(() => OnTestButtonPressed(GameObject.Find("TestUnitButton")));
            GameObject.Find("StopTestUnit").GetComponent<Button>().onClick.AddListener(() => DoOutAnimation());
            TMP_InputField searchBar1 = GameObject.Find("SearchUnitBlueprint").GetComponent<TMP_InputField>();
            TMP_InputField searchBar2 = GameObject.Find("SearchUnitBlueprint2").GetComponent<TMP_InputField>();

            TMP_Dropdown dropdown = GameObject.Find("UnitBlueprintDropdown").GetComponent<TMP_Dropdown>();
            TMP_Dropdown dropdown2 = GameObject.Find("UnitBlueprintDropdown2").GetComponent<TMP_Dropdown>();
            dropdown.onValueChanged.AddListener(n => GetUnitBlueprint1(n, dropdown));
            dropdown2.onValueChanged.AddListener(n => GetUnitBlueprint2(n, dropdown2));
            searchBar1.onValueChanged.AddListener(n => SearchForUnitBlueprint(n, dropdown));
            searchBar2.onValueChanged.AddListener(n => SearchForUnitBlueprint(n, dropdown2));

            itemList = gameObject.AddComponent<ItemList>();


            yield return new WaitUntil(() => FindObjectOfType<NodeComponent>());
            yield return null;
        }
        public void ShowAbilities(UnityAction<string> callBack)
        {
            ShowItemList(AbilityCreator.abilites.Keys.ToArray(), callBack);
        }
        public void ShowClothing(UnityAction<string> callBack)
        {
            ShowItemList(AbilityCreator.props.Keys.ToArray(), callBack);
        }
        public void ShowExplosions(UnityAction<string> callBack)
        {
            ShowItemList(AbilityCreator.explosions.Keys.ToArray(), callBack);
        }
        public void ShowProjectiles(UnityAction<string> callBack)
        {
            ShowItemList(AbilityCreator.projectiles.Keys.ToArray(), callBack);
        }
        public void ShowParticles(UnityAction<string> callBack)
        {
            ShowItemList(AbilityCreator.particles.Keys.ToArray(), callBack);
        }
        public void ShowComponents(UnityAction<string> callBack)
        {
            ShowItemList(AbilityCreator.components.Keys.ToArray(), callBack);
        }
        public void ShowWeapons(UnityAction<string> callBack)
        {
            ShowItemList(AbilityCreator.weapons.Keys.ToArray(), callBack);
        }
        public void ShowEffects(UnityAction<string> callBack)
        {
            ShowItemList(AbilityCreator.effects.Keys.ToArray(), callBack);
        }
        public void ShowSounds(UnityAction<string> callBack)
        {
            ShowItemList(AbilityCreator.sounds.ToArray(), callBack);
        }
        public void ShowUnit(UnityAction<string> callBack)
        {
            ShowItemList(AbilityCreator.units.Keys.ToArray(), callBack);
        }
        public void ShowItemList(string[] content, UnityAction<string> callBack, bool changeContent = true)
        {
            itemList.ShowItemsList(content, callBack, changeContent);
        }


        public string GetNiceName(string s)
        {
            try
            {
                return Localizer.GetLanguage(Localizer.GetSystemLanguage())[s];
            }
            catch
            {
                return s;
            }

        }
        private void DoOutAnimation()
        {
            try
            {
                Transform transform1 = GameObject.Find("AbilitySaveButton").transform.parent.transform.Find("nodesScaler").transform;
                for (int i = 0; i < transform1.childCount; i++)
                {
                    transform1.GetChild(i).gameObject.SetActive(true);
                }
            }
            catch
            {
            }
            GameObject.Find("AbilityCreationPage").GetComponent<CodeAnimation>().PlayOut();
            GameObject.Find("UnitTestingPage").GetComponent<CodeAnimation>().PlayOut();
            GameObject.Find("TestPage").GetComponent<CodeAnimation>().PlayOut();
            UnTestUnit();
        }
        private void DoInAnimation()
        {
            RefreshAbility();
            GameObject.Find("AbilityCreationPage").GetComponent<CodeAnimation>().PlayIn();
            GameObject.Find("UnitTestingPage").GetComponent<CodeAnimation>().PlayIn();
            GameObject.Find("TestPage").GetComponent<CodeAnimation>().PlayIn();
        }
        private void OnTestButtonPressed(GameObject button)
        {
            GameObject trigImage = button.GetComponentsInChildren<Image>(true)[0].gameObject;
            GameObject squareImage = button.GetComponentsInChildren<Image>(true)[1].gameObject;
            if (buttonState == false)
            {
                squareImage.SetActive(false);
                trigImage.SetActive(true);
                buttonState = true;
                StartCoroutine(TestUnit(GameObject.Find("SpawnUnitPos").transform.localPosition, GameObject.Find("SpawnEnemyPos").transform.localPosition));
            }
            else
            {
                squareImage.SetActive(true);
                trigImage.SetActive(false);
                buttonState = false;
                UnTestUnit();
            }

        }
        private void GetUnitBlueprint1(int optionIndex, TMP_Dropdown me)
        {
            List<UnitBlueprint> unitBlueprints = AbilityCreator.units.Values.ToList();
            string name = me.options[optionIndex].text;
            unitBlueprint1 = unitBlueprints.Find(n => GetNiceName(n.Entity.Name) == name);
        }
        private void GetUnitBlueprint2(int optionIndex, TMP_Dropdown me)
        {
            List<UnitBlueprint> unitBlueprints = AbilityCreator.units.Values.ToList();
            string name = me.options[optionIndex].text;
            unitBlueprint2 = unitBlueprints.Find(n => GetNiceName(n.Entity.Name) == name);
        }
        public void SearchForUnitBlueprint(string str, TMP_Dropdown dropdown)
        {

            if (unitSearchCoroutine != null)
            {
                StopCoroutine(unitSearchCoroutine);
                unitSearchCoroutine = null;
            }
            unitSearchCoroutine = StartCoroutine(SearchForUnitBlueprintEnum(str, dropdown));
        }
        private IEnumerator SearchForUnitBlueprintEnum(string s, TMP_Dropdown dropdown)
        {

            yield return null;
            yield return null;
            yield return null;
            yield return null;
            yield return null;
            dropdown.options = new List<TMP_Dropdown.OptionData>();
            var unitBlueprints = AbilityCreator.units.GetEnumerator();
            int index = 0;
            int i = 0;
            while (unitBlueprints.MoveNext())
            {
                var unitBlueprint = unitBlueprints.Current.Value;
                if (unitBlueprint == null)
                {
                    AbilityCreator.units.Remove(unitBlueprints.Current.Key);
                    unitBlueprint = unitBlueprints.Current.Value;
                }
                string unitName = GetNiceName(unitBlueprint.Entity.Name);
                if (i % 10 == 0)
                {
                    yield return null;
                }
                if (unitName.ToLower().Contains(s.ToLower()))
                {
                    dropdown.options.Add(new TMP_Dropdown.OptionData(unitName));
                    index++;
                }
                if (unitName.ToLower() == s.ToLower())
                {
                    dropdown.options = new List<TMP_Dropdown.OptionData>() { new TMP_Dropdown.OptionData(unitName) };
                    dropdown.value = 0;
                    dropdown.onValueChanged.Invoke(0);
                    dropdown.itemText.text = unitName;
                    break;
                }
                i++;
                if (index > 5)
                    break;
            }



        }
        private void UnTestUnit()
        {
            ServiceLocator.GetService<RuntimeGarbageCollector>().ForceFlushGC();

        }
        private IEnumerator TestUnit(Vector3 position, Vector3 position2)
        {
            ServiceLocator.GetService<RuntimeGarbageCollector>().ForceFlushGC();
            yield return null;
            // Unit 1
            UnitBlueprint unitBlueprint = Instantiate(unitBlueprint1);
            unitBlueprint.objectsToSpawnAsChildren = unitBlueprint.objectsToSpawnAsChildren.AddItem(Ability).ToArray();
            GameObject[] gameObjects = unitBlueprint.Spawn(position, Quaternion.identity, Team.Red);
            gameObjects[0].transform.root.position = position;

            // Unit 2
            UnitBlueprint enemyUnitBlueprint = Instantiate(unitBlueprint2);
            GameObject[] gameObjects2 = enemyUnitBlueprint.Spawn(position2, Quaternion.identity, Team.Blue);
            gameObjects2[0].transform.root.position = position2;
            SettingsInstance m_healthBarOption = (SettingsInstance)ServiceLocator.GetService<UnitHealthbars>().GetField("m_healthBarOption");
            GameObject healthBar = (GameObject)ServiceLocator.GetService<UnitHealthbars>().GetField("healthBar");
            if (m_healthBarOption == null)
            {
                ServiceLocator.GetService<UnitHealthbars>().CallMethod("GetHealthBarSettings");
            }

            if (m_healthBarOption != null && m_healthBarOption.currentValue == 1)
            {
                Instantiate(healthBar).GetComponent<TABCUnitUI>().Init(gameObjects[0].GetComponent<Unit>());
                Instantiate(healthBar).GetComponent<TABCUnitUI>().Init(gameObjects2[0].GetComponent<Unit>());
            }
            DeveloperLogger.Log("Healthbar stuff : " + m_healthBarOption?.currentValue);
            ServiceLocator.GetService<GameStateManager>().EnterBattleState();
        }

        private void ToggleCanMove(int i = 2)
        {
            if (i == 2)
            {
                if (CanMove == false)
                {
                    CanMove = true;
                }
                else if (CanMove == true)
                {
                    CanMove = false;
                }
            }
            else if (i == 1)
            {
                CanMove = true;
            }
            else if (i == 0)
            {
                CanMove = false;
            }

        }
        public VirtualNode VirtualizeNode(NodeComponent nodeComponent)
        {
            VirtualNode virtualNode = new VirtualNode(nodeComponent.GetNodeInstanceID());
            virtualNode.nodeBlueprint = AbilityCreator.nodeDatabase.First(n => n.Value == nodeComponent.nodeBlueprint).Key;
            virtualNode.editorPositon = nodeComponent.transform.position;
            List<VirtualNodeField> virtualNodeFields = new List<VirtualNodeField>();
            foreach (NodeField item in nodeComponent.GetComponentsInChildren<NodeField>())
            {
                virtualNodeFields.Add(item.field);
            }
            List<ConnectionType> connectionTypes = new List<ConnectionType>();
            foreach (var connection in nodeComponent.Connections)
            {
                connectionTypes.Add(new ConnectionType()
                {
                    connectedNode = connection.Value.connected.transform.root.GetComponent<NodeComponent>().GetNodeInstanceID(),
                    connectedNodePortName = connection.Value.connected.connectionType.portName,
                    portName = connection.Value.connectionType.portName,
                    connectionType = connection.Value.connectionType.connectionType, 
                });
            }
            return virtualNode;
        }
        public VirtualNodeScene BuildNodeScene()
        {
            DeveloperLogger.Log("Building node scene...");

            VirtualNodeScene virtualNodeScene = new VirtualNodeScene();
            NodeComponent[] nodes = FindObjectsOfType<NodeComponent>();

            DeveloperLogger.Log($"{nodes.Length} nodes to build!");

            List<LegacySavedNode> savedNodes = new List<LegacySavedNode>();
            for (int savedNodeIndex = 0; savedNodeIndex < nodes.Length; savedNodeIndex++)
            {
                NodeComponent currentNode = nodes[savedNodeIndex];
                GameObject obj = new GameObject($"SavedNode ({currentNode.nodeBlueprint.Name}) ({currentNode.GetNodeInstanceID()})");
                DontDestroyOnLoad(obj);
                LegacySavedNode savedNode = obj.AddComponent<LegacySavedNode>();
                savedNode.corispondingNode = currentNode;
                currentNode.corispondingNode = savedNode;
                savedNode.blueprintName = currentNode.nodeBlueprint.key;
                savedNodes.Add(savedNode);
                DeveloperLogger.Log($"Created saved node ({savedNode.Blueprint.Name})...");
            }
            DeveloperLogger.Log("Created all saved nodes!");
            for (int nodeIndex = 0; nodeIndex < nodes.Length; nodeIndex++)
            {
                DeveloperLogger.Log("Node index:" + nodeIndex);
                NodeComponent currentNode = nodes[nodeIndex];
                LegacySavedNode savedNode = savedNodes[nodeIndex];
                //field stuff
                if (currentNode.nodeBlueprint.fields.Count == 0)
                {
                    DeveloperLogger.Log($"Current node ({currentNode.nodeBlueprint.Name}) has no fields! skipping");
                    savedNode.fields.Add("");
                }
                else
                {
                    for (int l = 0; l < currentNode.nodeBlueprint.fields.Count; l++)
                    {
                        string txt = currentNode.transform.Find("Fields").Find(nodes[nodeIndex].nodeBlueprint.fields[l].name).GetComponentInChildren<NodeField>().Value;
                        if (txt == "")
                            txt = "_";
                        savedNode.fields.Add(txt);
                        DeveloperLogger.Log($"Added field for node ({currentNode.nodeBlueprint.Name})!");
                    }
                }
                // position stuff
                Vector3 position = currentNode.transform.position;
                savedNode.position = position;
                DeveloperLogger.Log($"Saved position for node ({currentNode.nodeBlueprint.Name})!");
                // connection stuff
                List<NodeComponent.LegacyConnection> connections = new List<NodeComponent.LegacyConnection>();
                KeyValuePair<NodeBlueprint.ConnectionClass, NodeConnector>[] connectionsArray = currentNode.Connections.ToArray();
                for (int i = 0; i < currentNode.Connections.Count; i++)
                {
                    DeveloperLogger.Log($"Adding connection for ({currentNode.nodeBlueprint.Name})! other: {connectionsArray[i].Value?.connected}");
                    connections.Add(new NodeComponent.LegacyConnection()
                    {
                        connectionsType = connectionsArray[i].Key,
                        savedNode = connectionsArray[i].Value?.connected?.transform?.parent?.parent?.GetComponent<NodeComponent>()?.corispondingNode
                    });
                    DeveloperLogger.Log($"Added connection for ({currentNode.nodeBlueprint.Name})! other: {connectionsArray[i].Value?.connected}");
                }
                DeveloperLogger.Log("Finishing up connections...");
                savedNode.connections = connections;
                DeveloperLogger.Log("Done with connections!");

            }
            DeveloperLogger.Log("Adding extra data!");
            try
            {
                virtualNodeScene.abilityName = GameObject.Find("AbName").GetComponent<TMP_InputField>().text;
                virtualNodeScene.abilityDescription = GameObject.Find("Description").GetComponent<TMP_InputField>().text;
                virtualNodeScene.abilityIcon = GameObject.Find("Imagename").GetComponent<TMP_InputField>().text;
            }
            catch
            {
                DeveloperLogger.Log("Faild to add extra data... its ok though!");
            }
            return virtualNodeScene;
        }
        public void SaveNodeScene()
        {
            System.DateTime dateTime = System.DateTime.Now;
            VirtualNodeScene nodeScene = BuildNodeScene();
            if (AbilityCreator.nodeScenes.Find(n => n.abilityName == nodeScene.abilityName) == null)
                NodesceneIndex = AbilityCreator.nodeScenes.Count;
            else
            {
                NodesceneIndex = AbilityCreator.nodeScenes.FindIndex(n => n.abilityName == nodeScene.abilityName);
            }

            if (nodeScene.abilityID == 0)
            {
                int id = Random.Range(int.MinValue, int.MaxValue);
                while (ContentDatabase.Instance().AssetLoader.Exists(new DatabaseID(-2, id)))
                {
                    id = Random.Range(int.MinValue, int.MaxValue);
                }
                nodeScene.abilityID = id;

                try
                {
                    AbilityCreator.nodeScenes[NodesceneIndex] = nodeScene;
                }
                catch
                {
                    AbilityCreator.nodeScenes.Add(nodeScene);
                }

            }
            DeveloperLogger.Log("Zoom: " + zoom);

            //Saves to files 
            if (File.Exists(AbilityCreator.GetPath(nodeScene)))
            {
                nodeScene.abilityID = Serialize.LoadJson<VirtualNodeScene>(File.ReadAllText(AbilityCreator.GetPath(nodeScene))).abilityID;
                File.Delete(AbilityCreator.GetPath(nodeScene));
                File.Create(AbilityCreator.GetPath(nodeScene)).Close();
            }


            WriteAbility(nodeScene, Serialize.SaveJson(nodeScene));
            AbilityCreator.Reload();


            TABSSceneManager.LoadMainMenu(true);
            DeveloperLogger.Log($"Done building nodescene! that took {System.DateTime.Now - dateTime}");
        }
        public static void WriteAbility(VirtualNodeScene nodeScene, string json)
        {
            string dirPath = AbilityCreator.abilitespath + "/" + AbilityCreator.CleanNodeName(nodeScene.abilityName);
            string abilityPath = Path.Combine(dirPath, nodeScene.abilityID + ".abilityx");
            //Check if it exists
            //say wallahi bro
            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);
            
            File.WriteAllText(abilityPath, json);


        }
    }
}