using System;
using HarmonyLib;
using Landfall.TABS;
using UnityEngine;

namespace HiddenUnits.HarmonyPatches
{
	// Token: 0x0200002B RID: 43
	[HarmonyPatch(typeof(CollisionWeapon), "OnCollisionEnter")]
	internal class ArmorWeaponPatch
	{
		// Token: 0x060000FA RID: 250 RVA: 0x0000B364 File Offset: 0x00009564
		[HarmonyPrefix]
		public static bool Prefix(CollisionWeapon __instance, Collision collision, ref MeleeWeapon ___meleeWeapon, ref Rigidbody ___rig, ref DataHandler ___connectedData)
		{
			AchillesArmor.UnitIsArmored component = collision.transform.root.GetComponent<AchillesArmor.UnitIsArmored>();
			bool flag = collision.transform && component && component.armorActive && collision.rigidbody && ___rig && ___meleeWeapon && ___meleeWeapon.isSwinging && ___connectedData && component.GetComponent<Unit>().Team != ___connectedData.unit.Team && component.parryPower > ___meleeWeapon.requiredPowerToParry;
			if (flag)
			{
				___meleeWeapon.StopSwing();
				___rig.AddForce(collision.contacts[0].point.normalized * -component.parryForce, 2);
				Object.Instantiate<GameObject>(component.weaponHitEffect, collision.contacts[0].point, Quaternion.identity);
			}
			return true;
		}
	}
}
