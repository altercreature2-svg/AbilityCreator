using System;
using Landfall.TABS;
using UnityEngine;
using UnityEngine.Events;

namespace HiddenUnits
{
	// Token: 0x02000028 RID: 40
	public class TriggerCollisionWeapon : MonoBehaviour
	{
		// Token: 0x060000E8 RID: 232 RVA: 0x0000ADB0 File Offset: 0x00008FB0
		public void Start()
		{
			this.collisionEffects = base.GetComponentsInChildren<CollisionWeaponEffect>();
			bool flag = this.meleeWeapon == null && base.GetComponent<MeleeWeapon>();
			if (flag)
			{
				this.meleeWeapon = base.GetComponent<MeleeWeapon>();
			}
		}

		// Token: 0x060000E9 RID: 233 RVA: 0x0000ADF8 File Offset: 0x00008FF8
		public void Update()
		{
			bool flag = this.startedCounting;
			if (flag)
			{
				this.counter += Time.deltaTime;
			}
			bool flag2 = this.counter >= this.cooldown;
			if (flag2)
			{
				this.counter = 0f;
				this.startedCounting = false;
				this.canDealDamage = true;
			}
		}

		// Token: 0x060000EA RID: 234 RVA: 0x0000AE54 File Offset: 0x00009054
		public void OnTriggerStay(Collider col)
		{
			this.DoDamage(col);
		}

		// Token: 0x060000EB RID: 235 RVA: 0x0000AE5F File Offset: 0x0000905F
		public void OnTriggerEnter(Collider col)
		{
			this.DoDamage(col);
		}

		// Token: 0x060000EC RID: 236 RVA: 0x0000AE6C File Offset: 0x0000906C
		public void DoDamage(Collider col)
		{
			bool flag = !col.attachedRigidbody || !col.attachedRigidbody.transform.root.GetComponent<Unit>() || !this.canDealDamage || (col.attachedRigidbody.transform.root.GetComponent<Unit>().Team == base.transform.root.GetComponent<Unit>().Team && !this.canDealDamageToTeammates) || col.attachedRigidbody.transform.root == base.transform.root;
			if (!flag)
			{
				bool flag2 = col.attachedRigidbody.transform.parent.name != "Rigidbodies";
				if (!flag2)
				{
					bool flag3 = this.meleeWeapon && !this.meleeWeapon.canDealDamage;
					if (!flag3)
					{
						foreach (CollisionWeaponEffect collisionWeaponEffect in this.collisionEffects)
						{
							collisionWeaponEffect.DoEffect(col.transform, new Collision());
						}
						this.collisionEvent.Invoke();
						col.attachedRigidbody.transform.root.GetComponent<Unit>().data.healthHandler.TakeDamage(this.damage, Vector3.zero, base.transform.root.GetComponent<Unit>(), DamageType.Piercing);
						this.AddForceToTarget(col.attachedRigidbody, 1f);
						bool flag4 = base.GetComponent<CollisionSound>();
						if (flag4)
						{
							this.DoEffect(col.transform, col, 100f);
						}
						this.startedCounting = true;
						this.canDealDamage = false;
					}
				}
			}
		}

		// Token: 0x060000ED RID: 237 RVA: 0x0000B02C File Offset: 0x0000922C
		public void AddForceToTarget(Rigidbody rig, float m = 1f)
		{
			WilhelmPhysicsFunctions.AddForceWithMinWeight(rig, base.transform.forward * this.knockback * m, 1, this.minMassCap);
			rig.velocity *= 0.7f;
		}

		// Token: 0x060000EE RID: 238 RVA: 0x0000B07C File Offset: 0x0000927C
		public void DoEffect(Transform hitTransform, Collider col, float impact)
		{
			bool flag = impact * 0.5f < 0.1f;
			if (!flag)
			{
				bool flag2 = base.GetComponent<CollisionSound>().onlySoundOnRig && !col.attachedRigidbody;
				if (!flag2)
				{
					bool flag3 = this.meleeWeapon;
					if (flag3)
					{
						ServiceLocator.GetService<SoundPlayer>().PlaySoundEffect(base.GetComponent<CollisionSound>().SoundEffectRef, impact * 0.5f, base.transform.position, SoundEffectVariations.GetMaterialType(col.gameObject, col.attachedRigidbody), null, 1f);
					}
				}
			}
		}

		// Token: 0x04000169 RID: 361
		public float damage = 100f;

		// Token: 0x0400016A RID: 362
		public float knockback = 100f;

		// Token: 0x0400016B RID: 363
		public float minMassCap = 10f;

		// Token: 0x0400016C RID: 364
		public float cooldown;

		// Token: 0x0400016D RID: 365
		private float counter;

		// Token: 0x0400016E RID: 366
		private bool canDealDamage = true;

		// Token: 0x0400016F RID: 367
		public bool canDealDamageToTeammates;

		// Token: 0x04000170 RID: 368
		private bool startedCounting;

		// Token: 0x04000171 RID: 369
		private CollisionWeaponEffect[] collisionEffects;

		// Token: 0x04000172 RID: 370
		public UnityEvent collisionEvent;

		// Token: 0x04000173 RID: 371
		public MeleeWeapon meleeWeapon;
	}
}
