using HarmonyLib;
using Landfall.TABS;
using System.Collections;
using UnityEngine;

namespace AC
{
    [HarmonyPatch(typeof(ProjectileHit))]
    [HarmonyPatch(nameof(ProjectileHit.Hit))]
    public class ProjectileHitHarmonyPatch
    {
        static bool Prefix(ProjectileHit __instance, RaycastHit sentHit, float multiplier = 1f)
        {
            IgnoreProjectileForTeam ignoreProjectileForTeam = __instance.GetComponent<IgnoreProjectileForTeam>();
            if (!ignoreProjectileForTeam)
                return true;
            return ignoreProjectileForTeam.team != sentHit.collider.transform.root.GetComponent<Unit>()?.Team;
            
        }
    }
}