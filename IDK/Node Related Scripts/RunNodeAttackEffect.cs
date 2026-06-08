using Landfall.TABS;
using System.Collections;
using UnityEngine;

namespace IDK.Node_Related_Scripts
{
    public class RunNodeAttackEffect : AttackEffect
    {
        public Unit unit;
        public LegacySavedNode savedNode;
        public NodeRunner nodeRunner;
        
        public override void DoEffect(Rigidbody target, Vector3 targetDir)
        {
            Debug.Log("RUNNING EFFECT!!!");
            Unit unit2 = target.transform.root.GetComponent<Unit>();
            savedNode.GetValuePool(unit).ClearValues();
            savedNode.GetValuePool(unit).AddValue(unit2);
            savedNode.GetValuePool(unit).AddValue(unit2.gameObject);
            savedNode.GetValuePool(unit).AddValue(unit2.data.mainRig);
            nodeRunner.StartCoroutine(nodeRunner.TriggerConnection(savedNode));
        }
    }
}