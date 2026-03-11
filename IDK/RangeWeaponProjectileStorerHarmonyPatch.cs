using HarmonyLib;
using System.Collections;
using UnityEngine;

namespace IDK
{
    [HarmonyPatch(typeof(RangeWeapon), nameof(RangeWeapon.SetProjectileStats))]
    public class RangeWeaponProjectileStorerHarmonyPatch
    {
        [HarmonyPostfix]
        public static void Postfix(RangeWeapon __instance, GameObject spawnedObject, Vector3 spawnDir, Vector3 directionToTarget, Rigidbody targetRig, Vector3 shootPositionForward, Vector3 targetRigPosition, Vector3 targetRigVelocity, byte? randomSeed = null)
        {
            RangeWeaponProjectileStorer rangeWeaponProjectileStorer = __instance.GetComponent<RangeWeaponProjectileStorer>();
            if (rangeWeaponProjectileStorer)
                rangeWeaponProjectileStorer.lastProjectile = spawnedObject;
        }  
    }
}