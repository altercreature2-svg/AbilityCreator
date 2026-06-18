using System;
using System.Collections.Generic;
using Landfall.TABS;
using UnityEngine;
using UnityEngine.Events;

namespace HiddenUnits
{
	// Token: 0x02000013 RID: 19
	public class BillyLaser : MonoBehaviour
	{
		// Token: 0x0600006F RID: 111 RVA: 0x0000615D File Offset: 0x0000435D
		public void Start()
		{
			this.collisionEffects = base.GetComponentsInChildren<CollisionWeaponEffect>();
		}

		// Token: 0x06000070 RID: 112 RVA: 0x0000616C File Offset: 0x0000436C
		public void OnTriggerStay(Collider col)
		{
			this.DoDamage(col);
		}

		// Token: 0x06000071 RID: 113 RVA: 0x00006177 File Offset: 0x00004377
		public void OnTriggerEnter(Collider col)
		{
			this.DoDamage(col);
		}

		// Token: 0x06000072 RID: 114 RVA: 0x00006184 File Offset: 0x00004384
		public void DoDamage(Collider col)
		{
			bool flag = !col.attachedRigidbody || !col.attachedRigidbody.transform.root.GetComponent<Unit>() || col.attachedRigidbody.transform.root.GetComponent<Unit>().Team == base.transform.root.GetComponent<Unit>().Team || col.attachedRigidbody.transform.root == base.transform.root || !col.attachedRigidbody.transform.IsChildOf(col.attachedRigidbody.transform.root.GetComponent<Unit>().data.transform) || (col.attachedRigidbody.transform.root.GetComponent<Unit>() && this.hitList.Contains(col.attachedRigidbody.transform.root.GetComponent<Unit>()));
			if (!flag)
			{
				Unit component = col.attachedRigidbody.transform.root.GetComponent<Unit>();
				foreach (CollisionWeaponEffect collisionWeaponEffect in this.collisionEffects)
				{
					collisionWeaponEffect.DoEffect(col.transform, new Collision());
				}
				this.collisionEvent.Invoke();
				component.data.healthHandler.TakeDamage(this.damage, Vector3.zero, base.transform.root.GetComponent<Unit>(), DamageType.Piercing);
				float num = Mathf.Clamp(col.attachedRigidbody.drag / 3f, 0.1f, 1f);
				WilhelmPhysicsFunctions.AddAxplosionForceWithMinWeight(col.attachedRigidbody, this.knockback * num, base.transform.position, 3f, 1, this.massCap);
				col.attachedRigidbody.velocity *= 0.9f;
				WilhelmPhysicsFunctions.AddAxplosionForceWithMinWeight(component.data.mainRig, this.knockback * num, base.transform.position, 3f, 1, this.massCap);
				component.data.mainRig.velocity *= 0.9f;
				bool flag2 = !this.canHitMultipleTimes;
				if (flag2)
				{
					this.hitList.Add(component);
				}
			}
		}

		// Token: 0x06000073 RID: 115 RVA: 0x000063EF File Offset: 0x000045EF
		public void ClearHits()
		{
			this.hitList.Clear();
		}

		// Token: 0x040000B6 RID: 182
		private List<Unit> hitList = new List<Unit>();

		// Token: 0x040000B7 RID: 183
		public float damage = 100f;

		// Token: 0x040000B8 RID: 184
		public float knockback = 100f;

		// Token: 0x040000B9 RID: 185
		public float massCap = 10f;

		// Token: 0x040000BA RID: 186
		public bool canHitMultipleTimes = true;

		// Token: 0x040000BB RID: 187
		private CollisionWeaponEffect[] collisionEffects;

		// Token: 0x040000BC RID: 188
		public UnityEvent collisionEvent;
	}
}
