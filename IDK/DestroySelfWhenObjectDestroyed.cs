using System.Collections;
using UnityEngine;

namespace IDK
{
    public class DestroySelfWhenObjectDestroyed : MonoBehaviour
    {
        public GameObject obj;
        private void Update()
        {
            if (obj == null)
                Destroy(gameObject);
        }
    }
}