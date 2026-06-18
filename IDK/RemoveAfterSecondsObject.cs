using System.Collections;
using UnityEngine;

namespace AC
{
    public class RemoveAfterSecondsObject : MonoBehaviour
    {
        public Object other;
        public float time;
        private IEnumerator Start()
        {
            yield return new WaitForSeconds(time);
            Destroy(other);
        }
    }
}