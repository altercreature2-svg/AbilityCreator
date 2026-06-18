using AC;
using AC.Node_Related_Scripts.SavingStuff;
using IDK.AbilityHandling.Saving;
using IDK.GlobalReferencing;
using IDK.Node_Related_Scripts;
using Landfall.TABS.UnitEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.Experimental.GlobalIllumination;
using static Mono.Security.X509.X520;

namespace IDK.AbilityHandling
{
    public class AbilityManager
    {
        public struct AbilityData
        {
            public GameObject abilityGo;
            public NodeRunner nodeRunner;
            public VirtualNodeScene nodeScene;
            public string abilityName;
        }
        public struct AbilitySpawnerData
        {
            public AbilityData ability;
            public GameObject spawnerGo;
        }

        private static AbilityManager _instance;
        public static AbilityManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new AbilityManager();
                return _instance;
            }
        }

        public List<AbilityData> Abilities { get; private set; }
        public Dictionary<VirtualNodeScene, AbilityData> LookUp1 { get; private set; }
        public Dictionary<string, AbilityData> LookUp2 { get; private set; }

        public List<AbilitySpawnerData> AbilitySpawners { get; private set; }
        public Dictionary<VirtualNodeScene, AbilitySpawnerData> LookUp3 { get; private set; }
        public Dictionary<GameObject, AbilitySpawnerData> LookUp4 { get; private set; }
        public void Init()
        {
            Abilities = new List<AbilityData>();
            LookUp1 = new Dictionary<VirtualNodeScene, AbilityData>();
            LookUp2 = new Dictionary<string, AbilityData>();
            LookUp3 = new Dictionary<VirtualNodeScene, AbilitySpawnerData>();
            LookUp4 = new Dictionary<GameObject, AbilitySpawnerData>();
            string[] names = FilePaths.AbilityNames;
            foreach (var name in names)
            {
                string content = FileManager.ReadAbility(name);
                VirtualNodeScene virtualNodeScene = SaveableHelper.Load<VirtualNodeScene>(content);
                RegisterAbility(virtualNodeScene);
            }
        }
        public void RegisterAbility(VirtualNodeScene virtualNodeScene, NodeRunner nodeRunner, GameObject abilityObject)
        {
            AbilityData abilityData = new AbilityData()
            {
                abilityGo = abilityObject,
                abilityName = virtualNodeScene.abilityName,
                nodeScene = virtualNodeScene,
                nodeRunner = nodeRunner,
            };
            GameObject abilitySpawner = CreateSpawner(abilityData);
            AbilitySpawnerData abilitySpawnerData = new AbilitySpawnerData()
            {
                ability = abilityData,
                spawnerGo = abilitySpawner
            };
            LookUp1.Add(virtualNodeScene, abilityData);
            LookUp2.Add(virtualNodeScene.abilityName, abilityData);
            LookUp3.Add(virtualNodeScene, abilitySpawnerData);
            LookUp4.Add(abilityObject, abilitySpawnerData);
            Abilities.Add(abilityData);
            AbilitySpawners.Add(abilitySpawnerData);
        }
        public void RegisterAbility(VirtualNodeScene virtualNodeScene)
        {
            GameObject abilityObject = CreateAbility(virtualNodeScene);
            NodeRunner nodeRunner = abilityObject.GetComponent<NodeRunner>();
            RegisterAbility(virtualNodeScene, nodeRunner, abilityObject);
        }
        public void UpdateAbility(VirtualNodeScene virtualNodeScene)
        {
            int oldDataIndex = Abilities.FindIndex(n => n.abilityName == virtualNodeScene.abilityName);
            if (oldDataIndex == -1) return;
            AbilityData? oldData = Abilities[oldDataIndex];
            if (oldData == null)
                return;
            GameObject abilityObject = CreateAbility(virtualNodeScene, false);
            AbilityData abilityData = new AbilityData()
            {
                abilityGo = abilityObject,
                abilityName = virtualNodeScene.abilityName,
                nodeScene = virtualNodeScene,
            };
            var variable = LookUp3[oldData.Value.nodeScene];
            variable.ability = abilityData;
            LookUp3[oldData.Value.nodeScene] = variable;

        }
        public void DeregisterAbility(VirtualNodeScene virtualNodeScene)
        {
            AbilityData abilityData = LookUp1[virtualNodeScene];
            LookUp1.Remove(abilityData.nodeScene);
            LookUp2.Remove(abilityData.nodeScene.abilityName);
            LookUp3.Remove(abilityData.nodeScene);
            LookUp4.Remove(abilityData.abilityGo);
            UnityEngine.Object.Destroy(abilityData.abilityGo);
        }
        public GameObject CreateAbility(VirtualNodeScene nodeScene, bool autoRegister = true)
        {
            GameObject abilityObject = new GameObject(nodeScene.abilityName);
            abilityObject.hideFlags = HideFlags.HideAndDontSave;
            SpecialAbility specialAbility = abilityObject.AddComponent<SpecialAbility>();
            specialAbility.SetField("m_entity", new Landfall.TABS.DatabaseEntity(Landfall.TABS.Workshop.WorkshopContentType.Any)
            {
                GUID = new Landfall.TABS.DatabaseID()
                {
                    m_ID = nodeScene.abilityID,
                    m_modID = -2,

                },
                Name = nodeScene.abilityName,
            });
            
            NodeRunner nodeRunner = abilityObject.AddComponent<NodeRunner>();
            nodeRunner.nodeSceneReference = GlobalReferenceManager.Instance.GetReference(nodeScene);
            GoToBodyPart goToBodyPart = abilityObject.AddComponent<GoToBodyPart>();
            goToBodyPart.targetPart = GoToBodyPart.TargetPart.Torso;

            if (FileManager.SearchForSpirte(nodeScene.abilityName, out Sprite sprite))
                specialAbility.Entity.SetSpriteIcon(sprite);
            if (autoRegister)
                RegisterAbility(nodeScene, nodeRunner, abilityObject);
            abilityObject.SetActive(false);
            return abilityObject;
        }
        public GameObject CreateSpawner(AbilityData abilityData)
        {
            GameObject spawnerObject = new GameObject(abilityData.nodeScene.abilityName + " - Spawner");
            SpawnAbility spawnAbility = spawnerObject.AddComponent<SpawnAbility>();
            spawnAbility.abilityToSpawn = abilityData.nodeScene.abilityName;
            spawnAbility.fallbackObject = abilityData.abilityGo;
            return spawnerObject;
        }
        public GameObject CreateIllegalSpawner(VirtualNodeScene nodeScene)
        {
            GameObject ability = CreateAbility(nodeScene, false);
            AbilityData abilityData = new AbilityData()
            {
                abilityGo = ability,
                abilityName = nodeScene.abilityName,
                nodeRunner = ability.GetComponent<NodeRunner>(),
                nodeScene = nodeScene,
            };
            return CreateSpawner(abilityData);
        }
        public GameObject CreateIllegalSpawner(GameObject ability)
        {
            VirtualNodeScene virtualNodeScene = ability.GetComponent<NodeRunner>().NodeScene;
            AbilityData abilityData = new AbilityData()
            {
                abilityGo = ability,
                abilityName = virtualNodeScene.abilityName,
                nodeRunner = ability.GetComponent<NodeRunner>(),
                nodeScene = virtualNodeScene,
            };
            return CreateSpawner(abilityData);
        }
    }
}
