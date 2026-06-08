using BitCode.Extensions;
using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IDK.NodeScripts
{
    public class WhenUnitShootsProjectile : ITriggerNode
    {
        public override ValuePool GetValuePool(LegacySavedNode savedNode, Unit unit, List<NodeComponent.LegacyConnection> connections, string[] fields)
        {
            return savedNode.GetValuePool(unit);
        }
        public override void EveryFrame(LegacySavedNode savedNode, Unit unit, List<NodeComponent.LegacyConnection> connections, string[] fields, NodeRunner nodeRunner)
        {
        }
        public override void StartFrame(LegacySavedNode savedNode, Unit unit, List<NodeComponent.LegacyConnection> connections, string[] fields, NodeRunner nodeRunner)
        {
            nodeRunner.StartCoroutine(DelayStart(unit, nodeRunner, savedNode));
        }
        public IEnumerator DelayStart(Unit unit, NodeRunner nodeRunner, LegacySavedNode savedNode)
        {
            yield return new WaitUntil(() => unit.data);
            yield return new WaitUntil(() => unit.data.weaponHandler);
            if (unit?.data?.weaponHandler?.rightWeapon && unit?.data?.weaponHandler?.rightWeapon is RangeWeapon rightRangeWeapon)
            {
                rightRangeWeapon.shootEvent.AddListener(() => AttackStarted(nodeRunner, savedNode, unit, 2));
                rightRangeWeapon.shootEndEvent.AddListener(() => AttackEnded(nodeRunner, savedNode, unit, 2));
            }
            if (unit?.data?.weaponHandler?.leftWeapon && unit?.data?.weaponHandler?.leftWeapon is RangeWeapon leftRangeWeapon)
            {
                leftRangeWeapon.shootEvent.AddListener(() => AttackStarted(nodeRunner, savedNode, unit, 1));
                leftRangeWeapon.shootEndEvent.AddListener(() => AttackEnded(nodeRunner, savedNode, unit, 1));
            }
        }
        void AttackStarted(NodeRunner nodeRunner, LegacySavedNode savedNode,Unit self, int forceWeapon)
        {
            switch (forceWeapon)
            {
                case 0:
                    break;
                case 1:
                    if (self.data.weaponHandler.leftWeapon is RangeWeapon rangeWeapon)
                    {
                        rangeWeapon.gameObject.GetOrAddComponent<RangeWeaponProjectileStorer>();
                    }
                    break;
                case 2:
                    if (self.data.weaponHandler.rightWeapon is RangeWeapon rangeWeapon1)
                    {
                        rangeWeapon1.gameObject.GetOrAddComponent<RangeWeaponProjectileStorer>();
                    }
                    break;
                default:
                    break;
            }
            
        }
        void AttackEnded(NodeRunner nodeRunner, LegacySavedNode savedNode, Unit self, int forceWeapon)
        {
            switch (forceWeapon)
            {
                case 0:
                    break;
                case 1:
                    if (self.data.weaponHandler.leftWeapon is RangeWeapon rangeWeapon)
                    {
                        RangeWeaponProjectileStorer rangeWeaponProjectileStorer = rangeWeapon.GetComponent<RangeWeaponProjectileStorer>();
                        if (rangeWeaponProjectileStorer)
                        {
                            savedNode.GetValuePool(self).AddValue(rangeWeaponProjectileStorer.lastProjectile);
                            savedNode.GetValuePool(self).AddValue(BundleManager.LeftRight.Left);
                            nodeRunner.StartCoroutine(nodeRunner.TriggerConnection(savedNode));
                        }
                    }
                    break;
                case 2:
                    if (self.data.weaponHandler.rightWeapon is RangeWeapon rangeWeapon1)
                    {
                        RangeWeaponProjectileStorer rangeWeaponProjectileStorer = rangeWeapon1.GetComponent<RangeWeaponProjectileStorer>();
                        if (rangeWeaponProjectileStorer)
                        {
                            savedNode.GetValuePool(self).AddValue(rangeWeaponProjectileStorer.lastProjectile);
                            savedNode.GetValuePool(self).AddValue(BundleManager.LeftRight.Right);
                            nodeRunner.StartCoroutine(nodeRunner.TriggerConnection(savedNode));
                        }
                    }
                    break;
                default:
                    break;
            }

        }
    }

}