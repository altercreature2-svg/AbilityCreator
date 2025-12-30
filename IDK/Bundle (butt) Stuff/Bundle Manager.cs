

using BitCode.Platform.Steamworks;
using IDK.Node_Related_Scripts.connection_stuff;
using System.Collections;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace IDK
{
    public static class Bundle_Manager
        
    {
        public static GameObject Node;
        public static GameObject Empty;
        public static GameObject ConnecterTrig;
        public static GameObject ConnecterUnit;
        public static GameObject ConnecterGameObject;
        public static GameObject ConnectorComponent;
        public static GameObject ConnecterVar;
        public static GameObject ConnecterAnything;
        public static GameObject ConnecterObjectVariable;
        public static GameObject Popup;
        public static AssetBundle scenes;
        public static AssetBundle AbilityPrefabs;
        public static void Setup()
        {
            //scenes = AssetBundle.LoadFromFile("D:/games/Tabs Moded/TABS Moded/BepInEx/plugins/Assetbundles/scenes"); ;
            scenes = LoadBundle("IDK.Bundle__butt__Stuff.scenes");
            //AbilityPrefabs = AssetBundle.LoadFromFile("D:/games/Tabs Moded/TABS Moded/BepInEx/plugins/Assetbundles/ability"); ;
            AbilityPrefabs = LoadBundle("IDK.Bundle__butt__Stuff.ability");
            Node = AbilityPrefabs.LoadAsset<GameObject>("Node");
            Empty = AbilityPrefabs.LoadAsset<GameObject>("Empty");
            ConnecterTrig = AbilityPrefabs.LoadAsset<GameObject>("Assets/Connecter_Trig.prefab");
            ConnecterUnit = AbilityPrefabs.LoadAsset<GameObject>("Assets/Connector_Unit.prefab");
            ConnecterGameObject = AbilityPrefabs.LoadAsset<GameObject>("Assets/Connector_GameObject.prefab");
            ConnecterVar = AbilityPrefabs.LoadAsset<GameObject>("Assets/Connector_Variable.prefab");
            ConnectorComponent = AbilityPrefabs.LoadAsset<GameObject>("Assets/Connector_Component.prefab");
            ConnecterAnything = AbilityPrefabs.LoadAsset<GameObject>("Assets/Connector_Anything.prefab");
            ConnecterObjectVariable = AbilityPrefabs.LoadAsset<GameObject>("Assets/Connecter_ObjectVariable.prefab");
            Popup = AbilityPrefabs.LoadAsset<GameObject>("Assets/CANSAVE.prefab");
            ConnecterTrig.AddComponent<NodeConnector>();
            ConnecterUnit.AddComponent<NodeConnector>();
            ConnecterGameObject.AddComponent<NodeConnector>();
            ConnecterVar.AddComponent<NodeConnector>();
            ConnectorComponent.AddComponent<NodeConnector>();
            ConnecterAnything.AddComponent<NodeConnector>();
            ConnecterObjectVariable.AddComponent<NodeConnector>();
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