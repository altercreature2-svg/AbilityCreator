using IDK.Node_Related_Scripts.Field_stuff;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IDK
{
    public class NodeField : MonoBehaviour
    {
        public VirtualNodeField field;
        public string Value
        {
            get
            {
                if (GetComponentInChildren<TMP_Dropdown>())
                {
                    Debug.Log("dropdown - get");
                    return GetComponentInChildren<TMP_Dropdown>().options[GetComponentInChildren<TMP_Dropdown>().value].text;
                }
                else if (GetComponentInChildren<Button>())
                {
                    return GetComponentInChildren<Button>().GetComponentInChildren<TextMeshProUGUI>().text;
                }
                Debug.Log("TMPUGUI - get");
                return GetComponentInChildren<TMP_InputField>().text;
            }
            set
            {
                if (GetComponentInChildren<TMP_Dropdown>())
                {
                    Debug.Log("dropdown - set");
                    GetComponentInChildren<TMP_Dropdown>().value = GetComponentInChildren<TMP_Dropdown>().options.FindIndex(n => n.text == value);
                }
                else if (GetComponentInChildren<Button>())
                {
                    GetComponentInChildren<Button>().GetComponentInChildren<TextMeshProUGUI>().text = value;
                }
                else
                {
                    Debug.Log("TMPUGUI - set");
                    GetComponentInChildren<TMP_InputField>().text = value;
                }
            }
        }
    }
}