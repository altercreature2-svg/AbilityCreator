using System;
using UnityEngine;

namespace HiddenUnits
{
	// Token: 0x02000026 RID: 38
	public class SwordAfterimages : MonoBehaviour
	{
		// Token: 0x060000DD RID: 221 RVA: 0x0000ABF0 File Offset: 0x00008DF0
		private void Start()
		{
			this.rig = base.GetComponentInParent<Rigidbody>();
		}

		// Token: 0x060000DE RID: 222 RVA: 0x0000AC00 File Offset: 0x00008E00
		private void Update()
		{
			this.counter += Time.deltaTime;
			bool flag = !this.canSpawn || (this.requireThreshold && this.rig.velocity.magnitude < this.thresholdToSpawn) || (this.requireCooldown && this.cooldown > this.counter);
			if (!flag)
			{
				this.DoAfterimage();
			}
		}

		// Token: 0x060000DF RID: 223 RVA: 0x0000AC74 File Offset: 0x00008E74
		public void DoAfterimage()
		{
			this.counter = 0f;
			Object.Instantiate<GameObject>(this.objectToSpawn, base.transform.position, base.transform.rotation);
		}

		// Token: 0x060000E0 RID: 224 RVA: 0x0000ACA4 File Offset: 0x00008EA4
		public void CanSpawn()
		{
			this.canSpawn = true;
		}

		// Token: 0x060000E1 RID: 225 RVA: 0x0000ACAE File Offset: 0x00008EAE
		public void CannotSpawn()
		{
			this.canSpawn = false;
		}

		// Token: 0x0400015F RID: 351
		private float counter;

		// Token: 0x04000160 RID: 352
		private Rigidbody rig;

		// Token: 0x04000161 RID: 353
		public GameObject objectToSpawn;

		// Token: 0x04000162 RID: 354
		public bool requireCooldown;

		// Token: 0x04000163 RID: 355
		public bool requireThreshold;

		// Token: 0x04000164 RID: 356
		public float cooldown;

		// Token: 0x04000165 RID: 357
		public float thresholdToSpawn;

		// Token: 0x04000166 RID: 358
		public bool canSpawn;
	}
}
