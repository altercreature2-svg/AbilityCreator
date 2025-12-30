using System.Collections;
using UnityEngine;

namespace IDK
{
    public class NodeRunnerFixer : MonoBehaviour
    {
        
        private void Awake()
        {
            StartCoroutine(AwakeInternal());
        }
        private IEnumerator AwakeInternal()
        {
            yield return new WaitUntil(() => GetComponent<NodeRunner>());
            GetComponent<NodeRunner>().Begin();
        } 
    }
}