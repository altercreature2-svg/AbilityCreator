using IDK.Node_Related_Scripts.connection_stuff;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace IDK
{
    public class NodeBlueprint : ScriptableObject
    {
        public System.Type nodeFunction;


        public string tab;
        public enum ConnectionType
        {
            Triggered,
            Trigger,
            ReciveUnit,
            GiveUnit,
            ReciveGameObject,
            GiveGameObject,
            ReciveVariable,
            GiveVariable,
            ReciveComponent,
            GiveComponent,
            ReciveAnything,
            GiveAnything,
            ReciveObjectVariable,
            GiveObjectVariable,
            ReciveLeftRight,
            GiveLeftRight,
        }
        public enum Type
        {
            Input,
            Function,
            SoftInput,
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
                Field,
            }
        }
        public string Name;
        public bool LongField = false;
        public string key;
        public Color color;
        public List<Field> fields = new List<Field>();
        public List<ConnectionType> connections;
        public Type type;
        public Node Spawn()
        {

            var node = Instantiate(BundleManager.node);

            node.AddComponent<NodeWindow>().dragRectTransform = node.GetComponent<RectTransform>();



            node.transform.SetParent(GameObject.Find("AbilitySaveButton").transform.parent.transform.Find("nodesScaler").transform);

            node.transform.localPosition = Vector3.zero;
            if (color == Color.white)
            {
                node.GetComponentInChildren<TextMeshProUGUI>().color = Color.black;
            }
            node.GetComponentInChildren<TextMeshProUGUI>().text = Name;

            node.transform.Find("Top").GetComponent<Image>().color = color;

            for (int i = 0; i < fields.Count; i++)
            {
                GameObject f = Instantiate(node.transform.Find("Fields").Find("Field").gameObject);
                var field = f;
                field.transform.name = fields[i].name;
                //field.transform.position = node.transform.Find("Field").transform.position;
                //field.transform.position += new Vector3((i*-50), 0, 0);
                field.GetComponentInChildren<TMP_InputField>().contentType = fields[i].contentType;
                field.transform.SetParent(node.transform.Find("Fields"));
                field.transform.Find("Field").GetComponentInChildren<TextMeshProUGUI>().text = fields[i].name;

                field.SetActive(true);
                if (LongField)
                {
                    field.transform.localScale = new Vector3()
                    {
                        x = field.transform.localScale.x * 3f,
                        y = field.transform.localScale.y,
                        z = field.transform.localScale.z,
                    };
                    field.transform.position += new Vector3((i * -100), 0, 0);
                }
                if (field.GetComponentInChildren<TMP_InputField>().contentType == TMP_InputField.ContentType.IntegerNumber | field.GetComponentInChildren<TMP_InputField>().contentType == TMP_InputField.ContentType.DecimalNumber)
                {
                    if (field.transform.Find("Field").GetComponentInChildren<TextMeshProUGUI>().text == "" | field.transform.Find("Field").GetComponentInChildren<TextMeshProUGUI>().text == null)
                    {
                        field.transform.Find("Field").GetComponentInChildren<TextMeshProUGUI>().text = "0";
                    }
                }
                if (fields[i].isDropdown)
                {
                    field.GetComponentInChildren<TMP_InputField>().gameObject.SetActive(false);
                    field.GetComponentInChildren<Button>().gameObject.SetActive(false);
                    field.GetComponentInChildren<TMP_Dropdown>().options = fields[i].dropDowns.Select(n => new TMP_Dropdown.OptionData(n)).ToList();
                }
                else if (fields[i].fieldType != Field.FieldType.None)
                {

                    field.GetComponentInChildren<TMP_InputField>().gameObject.SetActive(false);
                    field.GetComponentInChildren<TMP_Dropdown>().gameObject.SetActive(false);
                    var button = field.GetComponentInChildren<Button>();
                    var txt = button.GetComponentInChildren<TextMeshProUGUI>();
                    txt.text = fields[i].name;

                    NodeManager nodeManager = UnityEngine.Object.FindObjectOfType<NodeManager>();
                    if (fields[i].fieldType is Field.FieldType.Explosion)
                        button.onClick.AddListener(() => nodeManager.ShowExplosions(n => txt.text = n));
                    else if (fields[i].fieldType is Field.FieldType.Projectile)
                        button.onClick.AddListener(() => nodeManager.ShowProjectiles(n => txt.text = n));
                    else if (fields[i].fieldType is Field.FieldType.Unit)
                        button.onClick.AddListener(() => nodeManager.ShowUnit(n => txt.text = n));
                    else if (fields[i].fieldType is Field.FieldType.Weapon)
                        button.onClick.AddListener(() => nodeManager.ShowWeapons(n => txt.text = n));
                    else if (fields[i].fieldType is Field.FieldType.Effect)
                        button.onClick.AddListener(() => nodeManager.ShowEffects(n => txt.text = n));
                    else if (fields[i].fieldType is Field.FieldType.Sound)
                        button.onClick.AddListener(() => nodeManager.ShowSounds(n => txt.text = n));
                    else if (fields[i].fieldType is Field.FieldType.Particle)
                        button.onClick.AddListener(() => nodeManager.ShowParticles(n => txt.text = n));
                    else if (fields[i].fieldType is Field.FieldType.Component)
                        button.onClick.AddListener(() => nodeManager.ShowComponents(n => txt.text = n));
                    else if (fields[i].fieldType is Field.FieldType.Field)
                        button.onClick.AddListener(() => nodeManager.ShowFields(n => txt.text = n, node.GetComponent<Node>()));
                }


                else
                {
                    field.GetComponentInChildren<TMP_Dropdown>().gameObject.SetActive(false);
                    field.GetComponentInChildren<Button>().gameObject.SetActive(false);
                }

                field.AddComponent<NodeField>().fieldType = fields[i].fieldType;
            }

            Node nodeComp = node.AddComponent<Node>();
            nodeComp.nodeBlueprint = this;
            node.transform.position = new Vector3(500, 195, 0);
            Destroy(node.transform.Find("Fields").Find("Field").gameObject);
            Transform reciver = node.transform.Find("Recives");
            Transform outer = node.transform.Find("Out");
            foreach (var connector in connections)
            {
                if (connector == ConnectionType.Triggered)
                {
                    Instantiate(BundleManager.connecterTrig, reciver).GetComponent<NodeConnector>().connectionType = connector;
                }
                if (connector == ConnectionType.ReciveUnit)
                {
                    Instantiate(BundleManager.connecterUnit, reciver).GetComponent<NodeConnector>().connectionType = connector;
                }
                if (connector == ConnectionType.ReciveGameObject)
                {
                    Instantiate(BundleManager.connecterGameObject, reciver).GetComponent<NodeConnector>().connectionType = connector;
                }
                if (connector == ConnectionType.ReciveVariable)
                {
                    Instantiate(BundleManager.connecterVar, reciver).GetComponent<NodeConnector>().connectionType = connector;
                }
                if (connector == ConnectionType.ReciveComponent)
                {
                    Instantiate(BundleManager.connectorComponent, reciver).GetComponent<NodeConnector>().connectionType = connector;
                }
                if (connector == ConnectionType.ReciveObjectVariable)
                {
                    Instantiate(BundleManager.connecterObjectVariable, reciver).GetComponent<NodeConnector>().connectionType = connector;
                }
                if (connector == ConnectionType.ReciveLeftRight)
                {
                    Instantiate(BundleManager.connecterLeftRight, reciver).GetComponent<NodeConnector>().connectionType = connector;
                }
                if (connector == ConnectionType.ReciveAnything)
                {
                    Instantiate(BundleManager.connecterAnything, reciver).GetComponent<NodeConnector>().connectionType = connector;
                }
                if (connector == ConnectionType.Trigger)
                {
                    Instantiate(BundleManager.connecterTrig, outer).GetComponent<NodeConnector>().connectionType = connector;
                }
                if (connector == ConnectionType.GiveGameObject)
                {
                    Instantiate(BundleManager.connecterGameObject, outer).GetComponent<NodeConnector>().connectionType = connector;
                }
                if (connector == ConnectionType.GiveUnit)
                {
                    Instantiate(BundleManager.connecterUnit, outer).GetComponent<NodeConnector>().connectionType = connector;
                }
                if (connector == ConnectionType.GiveVariable)
                {
                    Instantiate(BundleManager.connecterVar, outer).GetComponent<NodeConnector>().connectionType = connector;
                }
                if (connector == ConnectionType.GiveComponent)
                {
                    Instantiate(BundleManager.connectorComponent, outer).GetComponent<NodeConnector>().connectionType = connector;
                }
                if (connector == ConnectionType.GiveObjectVariable)
                {
                    Instantiate(BundleManager.connecterObjectVariable, outer).GetComponent<NodeConnector>().connectionType = connector;
                }
                if (connector == ConnectionType.GiveLeftRight)
                {
                    Instantiate(BundleManager.connecterLeftRight, outer).GetComponent<NodeConnector>().connectionType = connector;
                }
                if (connector == ConnectionType.GiveAnything)
                {
                    Instantiate(BundleManager.connecterAnything, outer).GetComponent<NodeConnector>().connectionType = connector;
                }
            }
            int reciverHeight = (int)(reciver.childCount * 1.3) * 12 + 8;
            if (reciver.childCount <= 1)
                reciverHeight = 0;
            int fieldsHeight = (int)(Math.Pow((fields.Count - 1), 1.4)*12)+5;
            int outHeight = (int)(outer.childCount* 1.3) * 12 + 8;
            if (outer.childCount <= 1)
                outHeight = 0;
            int finalHeight = Math.Max(Math.Max(Math.Max(reciverHeight, fieldsHeight), outHeight),0);
            Vector3 localScale = node.transform.Find("Background").transform.localScale;
            localScale.y += ((float)finalHeight) / 100f;
            node.transform.Find("Background").transform.localScale = localScale;

            return nodeComp;
        }

    }
}