using Landfall.TABS;
using Landfall.TABS.GameState;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace IDK
{
    public class Code : GameStateListener
    {
        public static GameObject button;
        public void Begin()
        {

            SceneManager.activeSceneChanged += SpawnButtonFIXXXX;
        }
        public void SpawnButtonFIXXXX(Scene s,Scene s2)
        {
            if (s2.name == "MainMenu")
            {
                StartCoroutine(SpawnButton_Internal());
            }
            

        }
        public IEnumerator SpawnButton_Internal()
        {
            yield return new WaitUntil(() => GameObject.Find("Quit"));
            //Create Button
            button = GameObject.Instantiate(GameObject.Find("Quit"), GameObject.Find("Quit").transform.parent);
            button.name = "Ability Creator";
            button.GetComponent<Button>().onClick.RemoveAllListeners();
            GameObject.Destroy(button.GetComponentInChildren<LocalizeText>());
            button.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = "Ability Creator";
            button.transform.SetSiblingIndex(4);
            button.GetComponent<Button>().onClick.AddListener(() => AbilityCreator.sceneManager.EnterNodeChanger());
            
        }


        public override void OnEnterBattleState()
        {
        }
        public override void OnEnterPlacementState()
        {
        }
        
        public static string commnet;
    }
}