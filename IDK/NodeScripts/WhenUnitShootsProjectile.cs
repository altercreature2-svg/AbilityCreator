using AC.Node_Related_Scripts.NodeRunning;
using AC.Node_Related_Scripts.NodeRunning.Instructions.Courtines;
using BitCode.Extensions;
using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AC.NodeScripts
{
    public class WhenUnitShootsProjectile : ITriggerNode
    {
        public IEnumerator<CoroutineReturn> Execute(NodeEnv env)
        {
            return null;
        }
        public IEnumerator<CoroutineReturn> Cache(NodeEnv env)
        {
            yield return new CoroutineReturn(CoroutineReturn.CourtineType.WaitUntil, arg0: 0, arg1: () => env.unit.data?.weaponHandler);
            WeaponHandler weaponHandler = env.unit.data.weaponHandler;
            List<Weapon> weapons = (List<Weapon>)weaponHandler.GetField("allWeapons");
            foreach (var weapon in weapons)
            {
                if (!(weapon is RangeWeapon rangeWeapon))
                    continue;
                rangeWeapon.shootEvent.AddListener(() => AttackStarted(env, rangeWeapon.gameObject));
                rangeWeapon.shootEndEvent.AddListener(() => AttackEnded(env, rangeWeapon.gameObject));
            }
        }

        void AttackStarted(NodeEnv env, GameObject weapon)
        {
            weapon.GetOrAddComponent<RangeWeaponProjectileStorer>();
        }
        void AttackEnded(NodeEnv env, GameObject rangeWeapon)
        {
            RangeWeaponProjectileStorer rangeWeaponProjectileStorer = rangeWeapon.GetComponent<RangeWeaponProjectileStorer>();
            env.AddValue(NodeBlueprint.ConnectionClass.GiveGameObject, rangeWeaponProjectileStorer.lastProjectile);
        }
    }

}