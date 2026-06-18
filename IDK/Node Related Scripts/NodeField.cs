using AC.Node_Related_Scripts.Field_stuff;
using Newtonsoft.Json.Linq;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AC
{
    public class NodeField : MonoBehaviour
    {
        public VirtualNodeField field;
        private TMP_Dropdown dropdown;
        private TMP_InputField inputField;
        private Button button;
        private TMP_Text inputText;
        private TMP_Text buttonText;
        public void Start()
        {
            dropdown = GetComponentInChildren<TMP_Dropdown>();
            inputField = GetComponentInChildren<TMP_InputField>();
            button = GetComponentInChildren<Button>();
            inputText = inputField.textComponent;
            buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
        }
        public string Value
        {
            get
            {
                return field.value;
            }
            set
            {
                field.value = value;
                UpdateDisplay();
            }
        }
        private void UpdateDisplay()
        {
            if (dropdown)
                dropdown.value = dropdown.options.FindIndex(n => n.text == Value);
            else if (button)
                buttonText.text = Value;
            else
                inputField.text = Value;

        }
    }
}