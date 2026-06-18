using AC.Node_Related_Scripts.connection_stuff;
using AC.Node_Related_Scripts.ConnectionStuff;
using AC.Node_Related_Scripts.Field_stuff;
using Cinemachine;
using IDK.Node_Related_Scripts;
using IDK.Node_Related_Scripts.Intercating;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static RootMotion.FinalIK.IKSolver;
namespace AC
{
    public class NodeBlueprint : ScriptableObject
    {
        public System.Type nodeFunction;

        public bool obselete;
        public string tab;
        [Flags]
        public enum ConnectionClass
        {
            None = 0,
            Triggered = 1 << 0, 
            Trigger = 1 << 1, 
            ReciveUnit = 1 << 2, 
            GiveUnit = 1 << 3, 
            ReciveGameObject = 1 << 4,
            GiveGameObject = 1 << 5,  
            ReciveVariable = 1 << 6,
            GiveVariable = 1 << 7,
            ReciveComponent = 1 << 8, 
            GiveComponent = 1 << 9,
            ReciveAnything = 1 << 10,
            GiveAnything = 1 << 11,
            ReciveObjectVariable = 1 << 12,
            GiveObjectVariable = 1 << 13, 
            ReciveLeftRight = 1 << 14, 
            GiveLeftRight = 1 << 15,

            AllRecivers = 
                          ReciveUnit
                          | ReciveGameObject
                          | ReciveVariable
                          | ReciveComponent
                          | ReciveAnything
                          | ReciveObjectVariable
                          | ReciveLeftRight,
            AllGivers =
                          GiveUnit
                          | GiveGameObject
                          | GiveVariable
                          | GiveComponent
                          | GiveAnything
                          | GiveObjectVariable
                          | GiveLeftRight
        }
        public struct Field
        {
            public string name;
            public TMP_InputField.ContentType contentType;
            public bool isDropdown;
            public string[] dropDowns;
            public FieldType fieldType;
            public Field(string name, TMP_InputField.ContentType contentType)
            {
                this.name = name;
                this.contentType = contentType;
                this.isDropdown = false;
                this.dropDowns = new string[] { };
                this.fieldType = FieldType.None;
            }

            public enum FieldType
            {
                None,
                Explosion,
                Projectile,
                Effect,
                Component,
                Sound,
                Unit,
                Weapon,
                Particle,
                Ability,
                Clothing,
            }
        }
        public string nodeName;
        public string nodeKey;
        public Color nodeBarColor;
        public List<Field> nodeFields = new List<Field>();
        public List<ConnectionClass> nodeConnections;
        public NodeComponent Spawn()
        {
            var node = Instantiate(BundleManager.node);

            // setting up shit
            TextMeshProUGUI textMesh = node.GetComponentInChildren<TextMeshProUGUI>();
            Image top = node.transform.Find("Top").GetComponent<Image>();
            Transform fieldDaddy = node.transform.Find("Fields");
            GameObject fieldPrefab = fieldDaddy.Find("Field").gameObject;
            Transform reciver = node.transform.Find("Recives");
            Transform outer = node.transform.Find("Out");

            NodeWindow nodeWindow = node.AddComponent<NodeWindow>();
            nodeWindow.dragRectTransform = node.GetComponent<RectTransform>();

            // doing shit
            node.transform.SetParent(GameObject.Find("AbilitySaveButton").transform.parent.transform.Find("nodesScaler").transform);
            if (nodeBarColor == Color.white | nodeBarColor == Color.yellow)
                textMesh.color = Color.black;
            textMesh.text = nodeName;
            top.color = nodeBarColor;

            for (int i = 0; i < nodeFields.Count; i++)
            {
                SpawnField(fieldDaddy, fieldPrefab, nodeFields[i], i);
            }

            NodeComponent nodeComp = node.AddComponent<NodeComponent>();
            nodeComp.nodeBlueprint = this;
            node.transform.position = new Vector3(500, 195, 0);
            Destroy(fieldPrefab);
            int j = 0;
            foreach (var connector in nodeConnections)
            {
                ConnectionType connectionType = new ConnectionType()
                {
                    first = new ConnectionType.VirtualPort()
                    {
                        connectedNode = nodeComp.GetNodeInstanceID(),
                        portIndex = j,
                    },
                    second = ConnectionType.VirtualPort.Null,
                    connectionClass = connector,
                };
                GameObject prefabToInstantiate = null;
                Transform parentTransform = null;

                switch (connector)
                {
                    case ConnectionClass.Triggered:
                        prefabToInstantiate = BundleManager.connecterTrig;
                        parentTransform = reciver;
                        break;
                    case ConnectionClass.ReciveUnit:
                        prefabToInstantiate = BundleManager.connecterUnit;
                        parentTransform = reciver;
                        break;
                    case ConnectionClass.ReciveGameObject:
                        prefabToInstantiate = BundleManager.connecterGameObject;
                        parentTransform = reciver;
                        break;
                    case ConnectionClass.ReciveVariable:
                        prefabToInstantiate = BundleManager.connecterVar;
                        parentTransform = reciver;
                        break;
                    case ConnectionClass.ReciveComponent:
                        prefabToInstantiate = BundleManager.connectorComponent;
                        parentTransform = reciver;
                        break;
                    case ConnectionClass.ReciveObjectVariable:
                        prefabToInstantiate = BundleManager.connecterObjectVariable;
                        parentTransform = reciver;
                        break;
                    case ConnectionClass.ReciveLeftRight:
                        prefabToInstantiate = BundleManager.connecterLeftRight;
                        parentTransform = reciver;
                        break;
                    case ConnectionClass.ReciveAnything:
                        prefabToInstantiate = BundleManager.connecterAnything;
                        parentTransform = reciver;
                        break;

                    case ConnectionClass.Trigger:
                        prefabToInstantiate = BundleManager.connecterTrig;
                        parentTransform = outer;
                        break;
                    case ConnectionClass.GiveGameObject:
                        prefabToInstantiate = BundleManager.connecterGameObject;
                        parentTransform = outer;
                        break;
                    case ConnectionClass.GiveUnit:
                        prefabToInstantiate = BundleManager.connecterUnit;
                        parentTransform = outer;
                        break;
                    case ConnectionClass.GiveVariable:
                        prefabToInstantiate = BundleManager.connecterVar;
                        parentTransform = outer;
                        break;
                    case ConnectionClass.GiveComponent:
                        prefabToInstantiate = BundleManager.connectorComponent;
                        parentTransform = outer;
                        break;
                    case ConnectionClass.GiveObjectVariable:
                        prefabToInstantiate = BundleManager.connecterObjectVariable;
                        parentTransform = outer;
                        break;
                    case ConnectionClass.GiveLeftRight:
                        prefabToInstantiate = BundleManager.connecterLeftRight;
                        parentTransform = outer;
                        break;
                    case ConnectionClass.GiveAnything:
                        prefabToInstantiate = BundleManager.connecterAnything;
                        parentTransform = outer;
                        break;

                    default:
                        // handle fallback if connector doesnt match any enum state
                        break;
                }

                if (prefabToInstantiate != null && parentTransform != null)
                {
                    GameObject spawnedConnector = Instantiate(prefabToInstantiate, parentTransform);
                    spawnedConnector.GetComponent<NodeConnector>().connectionType = connectionType;
                }
                j++;
            }

            // beware of the evil curse
            int reciverHeight = (int)(reciver.childCount * 1.3) * 12 + 8;
            if (reciver.childCount <= 1)
                reciverHeight = 0;
            int fieldsHeight = (int)(Math.Pow((nodeFields.Count - 1), 1.4)*12)+5;
            int outHeight = (int)(outer.childCount* 1.3) * 12 + 8;
            if (outer.childCount <= 1)
                outHeight = 0;
            int finalHeight = Mathf.Max(reciverHeight, fieldsHeight, outHeight,0);
            // wow you survived the evil curse you must be some kind of alpha

            Vector3 localScale = node.transform.Find("Background").transform.localScale;
            localScale.y += (finalHeight) / 100f;

            node.transform.Find("Background").transform.localScale = localScale;

            return nodeComp;
        }
        public static GameObject SpawnField(Transform parent, GameObject prefab, Field nodeField, int index)
        {
            GameObject field = Instantiate(prefab, parent);
            Button button = field.GetComponentInChildren<Button>();
            TMP_Dropdown dropdown = field.GetComponentInChildren<TMP_Dropdown>();
            TMP_InputField inputField = field.GetComponentInChildren<TMP_InputField>();
            TMP_Text text = inputField.textComponent;
            field.transform.name = nodeField.name;
            inputField.contentType = nodeField.contentType;
            text.text = field.name;
            field.SetActive(true);

            if (inputField.contentType == TMP_InputField.ContentType.IntegerNumber | inputField.contentType == TMP_InputField.ContentType.DecimalNumber)
            {
                if (string.IsNullOrEmpty(text.text))
                {
                    text.text = "0";
                }
            }

            if (nodeField.isDropdown)
            {
                inputField.gameObject.SetActive(false);
                button.gameObject.SetActive(false);
                dropdown.options = nodeField.dropDowns.Select(n => new TMP_Dropdown.OptionData(n)).ToList();
            }
            else if (nodeField.fieldType != Field.FieldType.None)
            {
                inputField.gameObject.SetActive(false);
                dropdown.gameObject.SetActive(false);
                TextMeshProUGUI showText = button.GetComponentInChildren<TextMeshProUGUI>();
                showText.text = nodeField.name;
                NodeManager nodeManager = UnityEngine.Object.FindObjectOfType<NodeManager>();
                if (nodeField.fieldType is Field.FieldType.Explosion)
                    button.onClick.AddListener(() => nodeManager.ShowExplosions(n => showText.text = n));
                else if (nodeField.fieldType is Field.FieldType.Projectile)
                    button.onClick.AddListener(() => nodeManager.ShowProjectiles(n => showText.text = n));
                else if (nodeField.fieldType is Field.FieldType.Unit)
                    button.onClick.AddListener(() => nodeManager.ShowUnit(n => showText.text = n));
                else if (nodeField.fieldType is Field.FieldType.Weapon)
                    button.onClick.AddListener(() => nodeManager.ShowWeapons(n => showText.text = n));
                else if (nodeField.fieldType is Field.FieldType.Effect)
                    button.onClick.AddListener(() => nodeManager.ShowEffects(n => showText.text = n));
                else if (nodeField.fieldType is Field.FieldType.Sound)
                    button.onClick.AddListener(() => nodeManager.ShowSounds(n => showText.text = n));
                else if (nodeField.fieldType is Field.FieldType.Particle)
                    button.onClick.AddListener(() => nodeManager.ShowParticles(n => showText.text = n));
                else if (nodeField.fieldType is Field.FieldType.Component)
                    button.onClick.AddListener(() => nodeManager.ShowComponents(n => showText.text = n));
                else if (nodeField.fieldType is Field.FieldType.Ability)
                    button.onClick.AddListener(() => nodeManager.ShowAbilities(n => showText.text = n));
                else if (nodeField.fieldType is Field.FieldType.Clothing)
                    button.onClick.AddListener(() => nodeManager.ShowClothing(n => showText.text = n));
            }
            else
            {
                dropdown.gameObject.SetActive(false);
                button.gameObject.SetActive(false);
            }
            VirtualNodeField virtualNodeField = new VirtualNodeField(index, text.text);
            NodeField nodeFieldComponent = field.AddComponent<NodeField>();
            nodeFieldComponent.field = virtualNodeField;

            return field;
        }

    }
}