using EMPTY;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IDK
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
        public static BundledAbility BundleAbility(ExtraSerializedUnit unit, SavedNodeScene savedNodeScene, string data)
        {
            BundledAbility bundledAbility = new GameObject("Bundledability").AddComponent<BundledAbility>();
            bundledAbility.abilityData = data;
            bundledAbility.abilityName = savedNodeScene.sceneName;
            bundledAbility.unitName = unit.m_name;
            bundledAbility.sprite = Main.GetSprite(savedNodeScene);
            return bundledAbility;
            
        }
    }
}