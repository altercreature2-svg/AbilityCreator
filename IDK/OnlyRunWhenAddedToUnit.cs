using Landfall.TABS;
using System.Collections;
using UnityEngine;

namespace IDK
{
    public class OnlyRunWhenAddedToUnit : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {
            Debug.Log("Awake");
            //yield return new WaitUntil(() => GetComponent<GoToBodyPart>());
            if (!transform.root.GetComponent<Unit>())
            {
                GetComponent<NodeRunner>().enabled = false;
                GetComponent<NodeRunnerFixer>().enabled = false;
                GetComponent<DodgeMove>().enabled = false;
                GetComponent<GoToBodyPart>().enabled = false;
                GetComponent<ConditionalEvent>().enabled = false;
                Debug.Log("False!!");
            }
            else
            {
                GetComponent<NodeRunner>().enabled = true;
                GetComponent<NodeRunnerFixer>().enabled = true;
                GetComponent<DodgeMove>().enabled = true;
                GetComponent<GoToBodyPart>().enabled = true;
                GetComponent<ConditionalEvent>().enabled = true;
                GetComponent<GoToBodyPart>().GoToPart();
                Debug.Log("True!!");
            }
        }

       
    }
}