using HarmonyLib;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static Sony.PS4.SaveData.Dialogs;
using static System.Net.Mime.MediaTypeNames;

namespace AC.Node_Related_Scripts
{
    public class ItemList : MonoBehaviour
    {
        public struct ButtonInfo
        {
            public GameObject obj;
            public TextMeshProUGUI text;
            public Button component;
        }
        public GameObject itemsList;
        public GameObject baseButton;
        public ButtonInfo[] buttonPool;
        public int poolSize = 50;
        public string[] items;
        public UnityAction<string> lastCallback;
        public Transform buttonHome;

        public void Awake()
        {
            itemsList = GameObject.Find("ListItems");
            buttonHome = itemsList.transform.Find("Scroll").Find("Viewport").Find("Content");
            baseButton = buttonHome.transform.Find("ContentButton").gameObject;
            baseButton.SetActive(false);
            // make pool
            buttonPool = new ButtonInfo[poolSize];
            for (int i = 0; i < poolSize; i++)
            {
                GameObject buttonObj = Instantiate(baseButton, buttonHome);
                buttonPool[i] = new ButtonInfo
                {
                    obj = buttonObj,
                    text = buttonObj.GetComponentInChildren<TextMeshProUGUI>(),
                    component = buttonObj.GetComponent<Button>()
                };
            }
            itemsList.SetActive(false);
            itemsList.AddComponent<SetSiblingIndex>().Ind = 100;
            Transform search = itemsList.transform.Find("SearchBarListItems");
            search.GetComponent<TMP_InputField>().onValueChanged.AddListener((string txt) => Search(txt));
        }

        public void Search(string txt)
        {
            string[] strings = items;
            List<string> result = new List<string>();

            string lower = txt.ToLower();

            foreach (var s in items)
            {
                if (s.ToLower().Contains(lower))
                    result.Add(s);
            }

            ShowItemsList(result.ToArray(), lastCallback, false);
        }
        public void ShowItemsList(string[] content, UnityAction<string> callBack, bool changeContent = true)
        {
            Debug.Log("Showing items list with " + content.Length + " items");
            if (changeContent)
                items = content;
            itemsList.SetActive(true);
            for (int i = 0; i < buttonPool.Length; i++)
            {
                if (i >= content.Length)
                {
                    buttonPool[i].obj.SetActive(false);
                    continue;
                }

                buttonPool[i].obj.SetActive(true);
                buttonPool[i].text.text = content[i];
                buttonPool[i].component.onClick.RemoveAllListeners();
                string capturedItem = content[i];
                buttonPool[i].component.onClick.AddListener(() =>
                {
                    Debug.Log(capturedItem + ": stop clicking me senpai :(");
                    callBack(capturedItem);
                    itemsList.SetActive(false);
                });



            }

            lastCallback = callBack;
        }
    }
}