using InControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.UI;

namespace IDK.Node_Related_Scripts.Intercating
{
    public class UIManager
    {
        public class UIElementWrapper
        {
            public string name;
            public GameObject go;
        }
        public class UIButtonWrapper : UIElementWrapper
        {
            public Button button;
            public UnityAction onClick;
        }
        private List<UIElementWrapper> uiWrappers = new List<UIElementWrapper>();
        public UIElementWrapper FindElement(string name)
        {
            UIElementWrapper result  = uiWrappers.Find(n => n.name == name);
            if (result == null)
                throw new KeyNotFoundException("Couldn't find element:" + name + " out of" + uiWrappers.Count + " elements");
            return result;
        }
        public void RegisterGenericElement(string name, GameObject gameObject)
        {
            UIElementWrapper elementWrapper = new UIElementWrapper()
            {
                name = name,
                go = gameObject,
            };
            uiWrappers.Add(elementWrapper);
        }
        public void RegisterButton(string name, GameObject gameObject)
        {
            UIButtonWrapper buttonWrapper = new UIButtonWrapper()
            {
                button = gameObject.GetComponent<Button>(),
                name = name,
                go = gameObject,
            };
            buttonWrapper.button.onClick.AddListener(buttonWrapper.onClick);
            uiWrappers.Add(buttonWrapper);
        }
        public void RegisterOnClickAction(string buttonName, UnityAction action)
        {
            UIElementWrapper elementWrapper = FindElement(buttonName);
            if (!(elementWrapper is UIButtonWrapper buttonWrapper))
            {
                Debug.LogError(buttonName + " is not a button!");
                return;
            }
            buttonWrapper.onClick += action;
        }
    }
}
