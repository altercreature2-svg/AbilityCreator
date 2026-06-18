using System;
using HarmonyLib;
using TGCore.Library;
using UnityEngine;

namespace HiddenUnits.HarmonyPatches
{
	// Token: 0x0200002A RID: 42
	[HarmonyPatch(typeof(ProjectileHit), "Hit")]
	internal class ArmorProjectilePatch
	{
		// Token: 0x060000F8 RID: 248 RVA: 0x0000B278 File Offset: 0x00009478
		[HarmonyPrefix]
		public static bool Prefix(ProjectileHit __instance, RaycastHit sentHit, float multiplier, ref MoveTransform ___move, ref RaycastTrail ___trail, ref TeamHolder ___teamHolder)
		{
			AchillesArmor.UnitIsArmored component = sentHit.transform.root.GetComponent<AchillesArmor.UnitIsArmored>();
			bool flag = !__instance.GetComponent<ProjectileHoming>() && sentHit.transform && component && component.armorActive && sentHit.rigidbody && component.blockPower > __instance.blockPoweredNeeded;
			bool result;
			if (flag)
			{
				bool flag2 = ___move;
				if (flag2)
				{
					___move.velocity = Vector3.Reflect(___move.velocity, sentHit.normal) * Random.Range(0.2f, 0.4f);
				}
				bool flag3 = ___trail;
				if (flag3)
				{
					___trail.ignoredFrames = 3;
				}
				Object.Instantiate<GameObject>(component.projectileHitEffect, sentHit.point, Quaternion.identity);
				result = false;
			}
			else
			{
				result = true;
			}
			return result;
		}
	}
}
