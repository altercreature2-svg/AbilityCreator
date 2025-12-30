using System.Collections;
using UnityEngine;

namespace IDK
{
    public class KillYourself : MonoBehaviour
    {
        public GameObject other;
        private void Update()
        {
            if (other == null)
                Destroy(gameObject);
        }
    }
}