using EMPTY;
using IDK.Node_Related_Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AC
{
    public class BundledAbilitesManager : MonoBehaviour
    {
        public class BundledAbility : MonoBehaviour
        {
            public string unitName;
            public string abilityName;
            public Sprite sprite;
            public string abilityData;
            public override string ToString()
            {
                return unitName + " + " + abilityName;
            }
            public void Awake()
            {
                DontDestroyOnLoad(gameObject);
            }
            void Start()
            {
                gameObject.name = $"{unitName} - {abilityName}";
            }
        }
        public static List<BundledAbility> bundledAbilities = new List<BundledAbility>();
        public static BundledAbility BundleAbility(ExtraSerializedUnit unit, VirtualNodeScene savedNodeScene, string data)
        {
            BundledAbility bundledAbility = new GameObject("Bundledability").AddComponent<BundledAbility>();
            bundledAbility.abilityData = data;
            bundledAbility.abilityName = savedNodeScene.abilityName;
            bundledAbility.unitName = unit.m_name;
            if (FileManager.SearchForSpirte(savedNodeScene.abilityName, out Sprite sprite))
                bundledAbility.sprite = sprite;
            return bundledAbility;
            
        }
    }
}