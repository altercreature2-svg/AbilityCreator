

using BitCode.Platform.Steamworks;
using IDK.Node_Related_Scripts.connection_stuff;
using System.Collections;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace IDK
{
    public static class BundleManager
    {
        public enum LeftRight
        {
            Left,
            Right,
        }
        public static GameObject node;
        public static GameObject empty;
        public static GameObject connecterTrig;
        public static GameObject connecterUnit;
        public static GameObject connecterGameObject;
        public static GameObject connectorComponent;
        public static GameObject connecterVar;
        public static GameObject connecterAnything;
        public static GameObject connecterObjectVariable;
        public static GameObject connecterLeftRight;
        public static GameObject Popup;
        public static AssetBundle scenes;
        public static AssetBundle AbilityPrefabs;
        public static void Setup()
        {
            //scenes = AssetBundle.LoadFromFile("D:/games/Tabs Moded/TABS Moded/BepInEx/plugins/Assetbundles/scenes"); ;
            scenes = LoadBundle("IDK.Bundle__butt__Stuff.scenes");
            //AbilityPrefabs = AssetBundle.LoadFromFile("D:/games/Tabs Moded/TABS Moded/BepInEx/plugins/Assetbundles/ability"); ;
            AbilityPrefabs = LoadBundle("IDK.Bundle__butt__Stuff.ability");
            node = AbilityPrefabs.LoadAsset<GameObject>("Node");
            empty = AbilityPrefabs.LoadAsset<GameObject>("Empty");
            connecterTrig = AbilityPrefabs.LoadAsset<GameObject>("Assets/Connecter_Trig.prefab");
            connecterUnit = AbilityPrefabs.LoadAsset<GameObject>("Assets/Connector_Unit.prefab");
            connecterGameObject = AbilityPrefabs.LoadAsset<GameObject>("Assets/Connector_GameObject.prefab");
            connecterVar = AbilityPrefabs.LoadAsset<GameObject>("Assets/Connector_Variable.prefab");
            connectorComponent = AbilityPrefabs.LoadAsset<GameObject>("Assets/Connector_Component.prefab");
            connecterAnything = AbilityPrefabs.LoadAsset<GameObject>("Assets/Connector_Anything.prefab");
            connecterObjectVariable = AbilityPrefabs.LoadAsset<GameObject>("Assets/Connecter_ObjectVariable.prefab");
            connecterLeftRight = AbilityPrefabs.LoadAsset<GameObject>("Assets/Connecter_LeftRight.prefab");
            Popup = AbilityPrefabs.LoadAsset<GameObject>("Assets/CANSAVE.prefab");
            connecterTrig.AddComponent<NodeConnector>();
            connecterUnit.AddComponent<NodeConnector>();
            connecterGameObject.AddComponent<NodeConnector>();
            connecterVar.AddComponent<NodeConnector>();
            connectorComponent.AddComponent<NodeConnector>();
            connecterAnything.AddComponent<NodeConnector>();
            connecterObjectVariable.AddComponent<NodeConnector>();
            connecterLeftRight.AddComponent<NodeConnector>();
        }
        public static AssetBundle LoadBundle(string assetBundleName)
        {
            using (Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(assetBundleName))
            {
                var a = AssetBundle.LoadFromStream(manifestResourceStream);
                
                return a;
            }
        }
    }
}