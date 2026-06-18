using Landfall.TABS;
using Landfall.TABS.AI;
using Landfall.TABS.AI.Components;
using System.Collections;
using Unity.Entities;
using UnityEngine;

namespace AC.HelpComponets
{
    public class MakeUnitStealth : MonoBehaviour
    {
        private void Start()
        {
            call();
        }
        private void call()
        {
            transform.root.GetComponent<UnitAPI>().SetIsDead();
        }
        private void OnDestroy()
        {
            bool flag = !base.transform.root.GetComponent<Unit>().data.Dead;
            if (flag)
            {
                
                base.transform.root.GetComponent<GameObjectEntity>().EntityManager.RemoveComponent<IsDead>(base.transform.root.GetComponent<GameObjectEntity>().Entity);
            }
        }
    }
}