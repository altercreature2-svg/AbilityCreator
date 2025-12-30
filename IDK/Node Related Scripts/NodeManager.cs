using BitCode.Debug.Commands;
using DM;
using HarmonyLib;
using IDK.Node_Related_Scripts.connection_stuff;
using Landfall.TABC;
using Landfall.TABS;
using Landfall.TABS.GameState;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;




namespace IDK
{
    public class NodeManager : MonoBehaviour
    {

        public Node[] SelectedNodes
        {
            get
            {
                Node[] array = FindObjectsOfType<UnityEngine.UI.Outline>().Where(n => n.transform.parent.GetComponent<Node>()).Select(n => n.transform.parent.GetComponent<Node>()).ToArray();
                return array;
            }
        }
        public bool CanMove = true;
        public int NodesceneIndex = 0;
        public int nodeid = 0;
        private GameObject RightClickMenu;
        private GameObject SaveMenu;
        private GameObject QuitMenu;
        private GameObject nodeScaler;
        private GameObject itemsList;
        public UnitBlueprint unitBlueprint1;
        public UnitBlueprint unitBlueprint2;
        public GameObject baseButton;
        public Vector3 scale = Vector3.one;
        private bool buttonState;
        public bool canScroll = true;
        public List<Node> nodes = new List<Node>();
        public Coroutine showItemsCoroutine;
        public Coroutine searchCoroutine;
        public Coroutine unitSearchCoroutine;
        private GameObject m_ability;
        private GameObject tutorial;
        private float zoom;
        public GameObject Ability
        {
            get
            {
                if (!m_ability)
                    m_ability = Main.CreateAbility(BuildNodeScene(), false);
                return m_ability;
            }
        }
        public void RefreshAbility()
        {
            m_ability = Main.CreateAbility(BuildNodeScene(), false);
        }
        private string[] items;
        private UnityAction<string> lastAction;
        private void Awake()
        {
            if (FindObjectsOfType<NodeManager>().Where(n => n != this).Count() != 0)
                Destroy(this);
            StartCoroutine(Awakeinternal());
        }
        private void Update()
        {

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                itemsList.SetActive(false);
            }
            if (Input.GetKey(KeyCode.LeftControl))
            {
                if (Input.GetKeyDown(KeyCode.D))
                {
                    if (SelectedNodes != null)
                    {
                        for (int i = 0; i < SelectedNodes.Length; i++)
                        {
                            GameObject duplicate = Main.nodeDatabase[SelectedNodes[i].blueprint.key].Spawn().gameObject;
                            NodeField[] fields = SelectedNodes[i].GetComponentsInChildren<NodeField>();
                            NodeField[] fields1 = duplicate.GetComponentsInChildren<NodeField>();
                            for (int i2 = 0; i2 < fields.Length; i2++)
                            {
                                fields1[i2].Value = fields[i2].Value;
                            }
                            duplicate.transform.position = SelectedNodes[i].transform.position + new Vector3(200, -200);
                        }

                    }
                }
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
            {
                if (SelectedNodes != null)
                {
                    for (int i = 0; i < SelectedNodes.Length; i++)
                    {
                        NodeConnector[] nodeConnectors = SelectedNodes[i].GetComponentsInChildren<NodeConnector>();
                        for (int i2 = 0; i2 < nodeConnectors.Length; i2++)
                        {
                            nodeConnectors[i2].RemoveAllConnections();
                        }
                    }



                    for (int i = 0; i < SelectedNodes.Length; i++)
                    {
                        Destroy(SelectedNodes[i].gameObject);
                    }

                }
            }

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
            foreach (Node item in FindObjectsOfType<Node>())
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
            NodeBlueprint[] allBlueprints = Main.nodeDatabase.Values.ToArray();
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
                    if (i2 <= nodeBlueprints.Length - 1)
                    {
                        GameObject button = Instantiate(buttonPrefab, tab.transform);
                        button.GetComponentInChildren<TextMeshProUGUI>(true).text = nodeBlueprints[referenceDestroyer2].Name;
                        button.GetComponentInChildren<Button>(true).onClick.AddListener(() =>
                        {
                            nodeBlueprints[referenceDestroyer2].Spawn();
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
                    Debug.Log("Name:" + tabsButtons[referenceDestroyer].name);
                    GameObject gameObject = tabsObjects.Find(n => n.name == tabsButtons[referenceDestroyer].name);
                    Debug.Log(gameObject);
                    gameObject.SetActive(true);
                });
            }
            Debug.Log("done");
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
            return Main.nodeDatabase.Values.Where(n => n.tab == tab).ToArray();
        }
        private IEnumerator Awakeinternal()
        {
            yield return new WaitUntil(() => TABSSceneManager.IsLoading == false);
            yield return new WaitUntil(() => GameObject.Find("Drawing Board") != null);
            yield return new WaitUntil(() => GameObject.Find("Hitbox Movement") != null);
            for (int i = 0; i < 3; i++)
                yield return null;
            unitBlueprint1 = Instantiate(Main.units["Clubber"]);
            unitBlueprint1.m_props = new GameObject[0];
            unitBlueprint1.RightWeapon = null;
            unitBlueprint2 = Instantiate(Main.units["Clubber"]);
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
            QuitMenu.GetComponent<Button>().onClick.AddListener(() => Main.sceneManager.EnterNodeChanger());
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
            itemsList = GameObject.Find("ListItems");
            Transform child = itemsList.transform.Find("Scroll").Find("Viewport").Find("Content");
            baseButton = child.transform.Find("ContentButton").gameObject;
            baseButton.SetActive(false);
            itemsList.SetActive(false);
            itemsList.AddComponent<SetSiblingIndex>().Ind = 100;
            yield return new WaitUntil(() => FindObjectOfType<Node>());
            yield return null;
            /*var allText = FindObjectsOfType<TextMeshProUGUI>();
            for (int i2 = 0; i2 < allText.Length; i2++)
            {
                allText[i2].extraPadding = true;
            }*/
            /*if (GooglyEyes.instance == null)
            {
                GameObject gameObject = new GameObject("Temp googly eyes");
                GooglyEyes.instance = gameObject.AddComponent<GooglyEyes>();
                DontDestroyOnLoad(gameObject);
            }*/
        }
        public void ShowFields(UnityAction<string> unityAction, Node node)
        {
            ShowListItems(Main.components[node.GetComponentsInChildren<NodeField>().First(n => n.fieldType is NodeBlueprint.Field.FieldType.Component).Value].GetRuntimeFields().Select(n => n.Name).ToArray(), unityAction);
        }
        public void ShowExplosions(UnityAction<string> unityAction)
        {
            ShowListItems(Main.explosions.Keys.ToArray(), unityAction);
        }
        public void ShowProjectiles(UnityAction<string> unityAction)
        {
            ShowListItems(Main.Projectiles.Keys.ToArray(), unityAction);
        }
        public void ShowParticles(UnityAction<string> unityAction)
        {
            ShowListItems(Main.particles.Keys.ToArray(), unityAction);
        }
        public void ShowComponents(UnityAction<string> unityAction)
        {
            ShowListItems(Main.components.Keys.ToArray(), unityAction);
        }
        public void ShowWeapons(UnityAction<string> unityAction)
        {
            ShowListItems(Main.weapons.Keys.ToArray(), unityAction);
        }
        public void ShowEffects(UnityAction<string> unityAction)
        {
            ShowListItems(Main.Effects.Keys.ToArray(), unityAction);
        }
        public void ShowSounds(UnityAction<string> unityAction)
        {
            ShowListItems(Main.sounds.ToArray(), unityAction);
        }
        public void ShowUnit(UnityAction<string> unityAction)
        {
            ShowListItems(Main.units.Keys.ToArray(), unityAction);
        }
        public void ShowListItems(string[] content, UnityAction<string> unityAction, bool changeContent = true)
        {
            if (showItemsCoroutine != null)
            {
                StopCoroutine(showItemsCoroutine);
                showItemsCoroutine = null;
            }
            showItemsCoroutine = StartCoroutine(ShowItemsList(content, unityAction, changeContent));
        }
        public IEnumerator SearchEnum(string txt)
        {
            yield return null;
            yield return null;
            yield return null;
            yield return null;
            yield return null;
            string[] strings = items;
            strings = strings.Where(n => n.ToLower().Contains(txt)).ToArray();
            ShowListItems(strings, lastAction, false);

        }
        public void Search(string txt)
        {
            if (searchCoroutine != null)
            {
                StopCoroutine(searchCoroutine);
                searchCoroutine = null;
            }
            searchCoroutine = StartCoroutine(SearchEnum(txt));
        }
        public IEnumerator ShowItemsList(string[] content, UnityAction<string> unityAction, bool changeContent = true)
        {

            if (changeContent)
                items = content;
            itemsList.SetActive(true);

            Transform child = itemsList.transform.Find("Scroll").Find("Viewport").Find("Content");
            Transform search = itemsList.transform.Find("SearchBarListItems");
            search.GetComponent<TMP_InputField>().onValueChanged.AddListener(Search);
            for (int i = 0; i < child.childCount; i++)
            {
                Transform item = child.GetChild(i);
                if (i % 100 == 0)
                    yield return null;
                try
                {
                    if (item == baseButton.transform)
                        continue;
                    Image img = baseButton.GetComponentInChildren<Image>();
                    if (img)
                        Destroy(img);
                    Destroy(item.gameObject);
                }
                catch { }
            }
            for (int i = 0; i < content.Length; i++)
            {
                string current = content[i];
                if (i % 100 == 0)
                    yield return null;
                try
                {
                    GameObject button = Instantiate(baseButton, baseButton.transform.parent);
                    button.SetActive(true);
                    button.GetComponentInChildren<TextMeshProUGUI>().text = content[i];
                    if (unityAction != null)
                        button.GetComponent<Button>().onClick.AddListener(() => unityAction(current));
                    button.GetComponent<Button>().onClick.AddListener(() => itemsList.SetActive(false));
                    if (i > 500)
                        break;
                }
                catch { }
            }

            lastAction = unityAction;
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
                TestUnit(GameObject.Find("SpawnUnitPos").transform.localPosition, GameObject.Find("SpawnEnemyPos").transform.localPosition);
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
            List<UnitBlueprint> unitBlueprints = Main.units.Values.ToList();
            string name = me.options[optionIndex].text;
            unitBlueprint1 = unitBlueprints.Find(n => GetNiceName(n.Entity.Name) == name);
        }
        private void GetUnitBlueprint2(int optionIndex, TMP_Dropdown me)
        {
            List<UnitBlueprint> unitBlueprints = Main.units.Values.ToList();
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
            var unitBlueprints = Main.units.GetEnumerator();
            int index = 0;
            int i = 0;
            while (unitBlueprints.MoveNext())
            {
                var unitBlueprint = unitBlueprints.Current.Value;
                if (unitBlueprint == null)
                {
                    Main.units.Remove(unitBlueprints.Current.Key);
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
                    dropdown.options = new List<TMP_Dropdown.OptionData>() { new TMP_Dropdown.OptionData(unitName)};
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
            RemoveAllUnits();
        }
        private void TestUnit(Vector3 position, Vector3 position2)
        {
            Time.timeScale = 1;
            RemoveAllUnits();

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
            Debug.Log("Healthbar stuff : " + m_healthBarOption?.currentValue);
            ServiceLocator.GetService<GameStateManager>().EnterBattleState();
        }
        private void RemoveAllUnits()
        {
            Unit[] units = FindObjectsOfType<Unit>();
            for (int i = 0; i < units.Length; i++)
            {
                Destroy(units[i].gameObject);
            }


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
        public NodeScene BuildNodeScene()
        {
            Debug.Log("Building node scene...");

            GameObject nodeobj = new GameObject($"Node Scene");
            var nodeScene = nodeobj.AddComponent<NodeScene>();

            Node[] nodes = FindObjectsOfType<Node>();
            Debug.Log($"{nodes.Length} nodes to build!");
            List<SavedNode> savedNodes = new List<SavedNode>();
            for (int savedNodeIndex = 0; savedNodeIndex < nodes.Length; savedNodeIndex++)
            {
                Node currentNode = nodes[savedNodeIndex];
                GameObject obj = new GameObject($"SavedNode ({currentNode.blueprint.Name}) ({currentNode.GetNodeInstanceID()})");
                DontDestroyOnLoad(obj);
                SavedNode savedNode = obj.AddComponent<SavedNode>();
                savedNode.corispondingNode = currentNode;
                currentNode.corispondingNode = savedNode;
                savedNode.blueprint = currentNode.blueprint;
                savedNodes.Add(savedNode);
                Debug.Log($"Created saved node ({savedNode.blueprint.Name})...");
            }
            Debug.Log("Created all saved nodes!");
            for (int nodeIndex = 0; nodeIndex < nodes.Length; nodeIndex++)
            {
                Debug.Log("Node index:" + nodeIndex);
                Node currentNode = nodes[nodeIndex];
                SavedNode savedNode = savedNodes[nodeIndex];
                //field stuff
                if (currentNode.blueprint.fields.Count == 0)
                {
                    Debug.Log($"Current node ({currentNode.blueprint.Name}) has no fields! skipping");
                    savedNode.fields.Add("");
                }
                else
                {
                    for (int l = 0; l < currentNode.blueprint.fields.Count; l++)
                    {
                        string txt = currentNode.transform.Find("Fields").Find(nodes[nodeIndex].blueprint.fields[l].name).GetComponentInChildren<NodeField>().Value;
                        if (txt == "")
                            txt = "_";
                        savedNode.fields.Add(txt);
                        Debug.Log($"Added field for node ({currentNode.blueprint.Name})!");
                    }
                }
                // position stuff
                Vector3 position = currentNode.transform.position;
                savedNode.position = position;
                Debug.Log($"Saved position for node ({currentNode.blueprint.Name})!");
                // connection stuff
                List<Node.Connection> connections = new List<Node.Connection>();
                KeyValuePair<NodeBlueprint.ConnectionType, NodeConnector>[] connectionsArray = currentNode.Connections.ToArray();
                for (int i = 0; i < currentNode.Connections.Count; i++)
                {
                    Debug.Log($"Adding connection for ({currentNode.blueprint.Name})! other: {connectionsArray[i].Value?.other}");
                    connections.Add(new Node.Connection()
                    {
                        connectionsType = connectionsArray[i].Key,
                        savedNode = connectionsArray[i].Value?.other?.transform?.parent?.parent?.GetComponent<Node>()?.corispondingNode
                    });
                    Debug.Log($"Added connection for ({currentNode.blueprint.Name})! other: {connectionsArray[i].Value?.other}");
                }
                Debug.Log("Finishing up connections...");
                savedNode.connections = connections;
                Debug.Log("Done with connections!");

            }
            Debug.Log("Adding extra data!");
            try
            {
                nodeScene.sceneName = GameObject.Find("AbName").GetComponent<TMP_InputField>().text;
                nodeScene.sceneDescription = GameObject.Find("Description").GetComponent<TMP_InputField>().text;
                nodeScene.sceneImage = GameObject.Find("Imagename").GetComponent<TMP_InputField>().text;
            }
            catch
            {
                Debug.Log("Faild to add extra data... its ok though!");
            }
            nodeScene.everyNode = savedNodes.ToArray();
            return nodeScene;
        }
        public void SaveNodeScene()
        {
            System.DateTime dateTime = System.DateTime.Now;
            NodeScene nodeScene = BuildNodeScene();
            if (Main.nodeScenes.Find(n => n.sceneName == nodeScene.sceneName) == null)
                NodesceneIndex = Main.nodeScenes.Count;
            else
            {
                NodesceneIndex = Main.nodeScenes.FindIndex(n => n.sceneName == nodeScene.sceneName);
            }

            if (nodeScene.id == 0)
            {
                int id = Random.Range(int.MinValue, int.MaxValue);
                while (ContentDatabase.Instance().AssetLoader.Exists(new DatabaseID(-2, id)))
                {
                    id = Random.Range(int.MinValue, int.MaxValue);
                }
                nodeScene.id = id;

                try
                {
                    Main.nodeScenes[NodesceneIndex] = nodeScene;
                }
                catch
                {
                    Main.nodeScenes.Add(nodeScene);
                }

            }
            nodeScene.zoom = zoom;
            Debug.Log("Zoom: " + zoom);
            SavedNodeScene savedNodeScene = SavedNodeScene.Instance(nodeScene);

            //Saves to files 
            var settings = new Newtonsoft.Json.JsonSerializerSettings()
            {
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore,
            };

            if (File.Exists(Main.GetPath(nodeScene)))
            {
                savedNodeScene.id = JsonConvert.DeserializeObject<SavedNodeScene>(File.ReadAllText(Main.GetPath(nodeScene)), settings).id;
                File.Delete(Main.GetPath(nodeScene));
                File.Create(Main.GetPath(nodeScene)).Close();
            }


            string json = Newtonsoft.Json.JsonConvert.SerializeObject(savedNodeScene, Newtonsoft.Json.Formatting.Indented, settings);
            WriteAbility(savedNodeScene, json, true);
            Main.Reload();


            TABSSceneManager.LoadMainMenu(true);
            Debug.Log($"Done building nodescene! that took {System.DateTime.Now - dateTime}");
        }
        public static void WriteAbility(SavedNodeScene nodeScene, string json, bool forceOverwrite = false, bool addToGame = false)
        {
            //Check if it exists
            if (Directory.Exists(Main.abilitespath + "/" + Main.CleanNodeName(nodeScene.sceneName)))
            {
                if (forceOverwrite)
                {
                    string[] files = Directory.GetFiles(Main.abilitespath + "/" + Main.CleanNodeName(nodeScene.sceneName));
                    for (int i = 0; i < files.Length; i++)
                    {
                        if (Path.GetExtension(files[i]) == ".ability")
                            File.Delete(files[i]);
                    }
                    File.WriteAllText(Main.abilitespath + "/" + Main.CleanNodeName(nodeScene.sceneName) + "/" + nodeScene.id + ".ability", json);
                }
            }
            else
            {
                Directory.CreateDirectory(Main.abilitespath + "/" + Main.CleanNodeName(nodeScene.sceneName));
                File.WriteAllText(Main.abilitespath + "/" + Main.CleanNodeName(nodeScene.sceneName) + "/" + nodeScene.id + ".ability", json);
            }
            if (addToGame)
            {
                Main.AddAbility(nodeScene.SavedNodeSceneToNodeScene());
            }

        }
    }
}