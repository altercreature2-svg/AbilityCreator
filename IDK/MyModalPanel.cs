using IDK.NodeScripts;
using Landfall.TABS;
using Landfall.TABS_Input;
using System.Collections;
using System.Collections.Generic;
using TFBGames;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IDK
{
    public class MyModalPanel : MonoBehaviour
    {
        public static List<BundledAbilitesManager.BundledAbility> queue = new List<BundledAbilitesManager.BundledAbility>();
        public static IEnumerator ShowModal(BundledAbilitesManager.BundledAbility bundledAbility)
        {
            Debug.Log("Modal:" + bundledAbility);
            queue.Add(bundledAbility);
            yield return new WaitUntil(() => queue.FindIndex(n => n == bundledAbility) == 0);
            Debug.Log("Done waiting!");
            ModalPanel modalPanel = ServiceLocator.GetService<ModalPanel>();
            Button m_YesButton = modalPanel.GetField<Button>("m_YesButton");
            Button m_CancelButton = modalPanel.GetField<Button>("m_CancelButton");
            m_YesButton.GetComponentInChildren<TextMeshProUGUI>().text = "Add";
            m_CancelButton.GetComponentInChildren<TextMeshProUGUI>().text = "Don't";

            
            AudioPathData path = null;
            AudioPathData.ValidateAndAssignPathData("UI/Unit Unlocked", ref path);
            AudioPlayer audioPlayer = ServiceLocator.GetService<SoundPlayer>().PlaySoundEffectNonAlloc(path, 1, Vector3.zero, SoundEffectVariations.MaterialType.Default);
            
            modalPanel.CallMethod("ResetButtonTexts");
            modalPanel.CallMethod("ResetElementConfiguration");
            
            
            NavigableTMPTextInput m_InputField = modalPanel.GetField<NavigableTMPTextInput>("m_InputField");
            GameObject inputFieldParent = m_InputField.transform.parent.gameObject;
            
            int result = (int)modalPanel.CallMethod("OpenPanel", new object[] { true });
            m_CancelButton.onClick.RemoveAllListeners();
            m_CancelButton.onClick.AddListener(delegate
            {
                modalPanel.CallMethod("ClosePanel", new object[] {true});
            });
            m_YesButton.onClick.RemoveAllListeners();
            m_YesButton.onClick.AddListener(delegate
            {
                modalPanel.CallMethod("ClosePanel", new object[] { true });
                StreamedSceneManager.AddBundledAbility(bundledAbility);
            });
            inputFieldParent.SetActive(value: false);
            if (PlayerActions.Instance.InputType == InputType.Controller)
            {
                m_InputField.Select();
            }

            modalPanel.CallMethod("SetHeaderText",new object[] { "New ability found!", new string[0] });
            modalPanel.CallMethod("SetQuestionText", new object[] { $"The unit \"{bundledAbility.unitName}\" has a custom ability called: \"{bundledAbility.abilityName}\", Do you wanna add it?", new string[0] });
            m_YesButton.gameObject.SetActive(value: true);
            m_CancelButton.gameObject.SetActive(value: true);
            modalPanel.CallMethod("SetupExplicitNavigation");
            yield return null;
            m_YesButton.GetComponentInChildren<TextMeshProUGUI>().text = "Add";
            m_CancelButton.GetComponentInChildren<TextMeshProUGUI>().text = "Don't";
        }
    }
}