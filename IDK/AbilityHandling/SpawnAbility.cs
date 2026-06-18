using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

namespace IDK.AbilityHandling
{
    
    public class SpawnAbility : MonoBehaviour
    {
        public string abilityToSpawn;
        public GameObject fallbackObject;
        public GameObject ObjectToSpawn
        {
            get
            {
                if (AbilityManager.Instance.LookUp2.TryGetValue(abilityToSpawn, out AbilityManager.AbilityData abilityData))
                    return abilityData.abilityGo;
                return null;
            }
        }
        public void Start()
        { 
            if (ObjectToSpawn)
                Spawn(ObjectToSpawn);
            else
                Spawn(fallbackObject);
        }
        public void Spawn(GameObject go)
        {
            Instantiate(go, transform).SetActive(true);
        }
    }
}
