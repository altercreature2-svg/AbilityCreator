using AC.Help;
using CASA.ObjectManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFBGames;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Rendering;

namespace IDK.Help
{
    // this is kinda stupid because it doesn't reuse them :/
    // it just pre generates them
    // cant do that cause i cant fully reset projectiles Eg: changing color
    // Unless i cache that ?
    // eh if it needs more optimizations then why not
    // if i do that i gotta remember *To have an option to disable it*
    public class EasyPool
    {
        public const int POOL_SIZE = 16;
        private List<GameObject> pool;
        public Action<GameObject> onSpawn;
        public GameObject prefab;
        private GameObject parent;
        public EasyPool(GameObject prefab, Action<GameObject> onSpawn)
        {
            pool = new List<GameObject>(POOL_SIZE); // allocate sum sum
            this.onSpawn = onSpawn;
            this.prefab = prefab;
            parent = new GameObject("pah pah");
            parent.SetActive(false);
            Regenerate();
        }
        public void Regenerate()
        {
            for (int i = 0; i < POOL_SIZE; i++)
            {
                AddPrefab();
            }
        }
        private void AddPrefab()
        {
            GameObject go = GameObject.Instantiate(prefab, parent.transform);
            onSpawn(go);
            pool.Add(go);
        }
        public GameObject Spawn(Vector3 position, Quaternion rotation)
        {
            if (pool.Count == 0)
                Regenerate();
            GameObject result = pool[0];
            pool.RemoveAt(0);
            result.transform.position = position;
            result.transform.rotation = rotation;
            result.SetActive(true);
           
            return result;
        }
    }
}
