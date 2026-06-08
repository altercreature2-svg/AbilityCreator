using Landfall.TABS;
using System.Collections;
using UnityEngine;

namespace IDK
{
    public class NodeRunnerFixer : MonoBehaviour
    {
        
        private void Awake()
        {
            if (transform.root.GetComponent<Unit>())
                StartCoroutine(AwakeInternal());
        }
        private IEnumerator AwakeInternal()
        {
            yield return new WaitUntil(() => GetComponent<NodeRunner>());
            GetComponent<NodeRunner>().Begin();
        } 
    }
}