using System.Collections;
using UnityEngine;

namespace AC
{
    public class SetSiblingIndex : MonoBehaviour
    {

        public int Ind = 0;
        // Update is called once per frame
        void Update()
        {
            if (gameObject.activeSelf == true)
            {
                transform.SetSiblingIndex(Ind);
            }
            
        }
    }
}