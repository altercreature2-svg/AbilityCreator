using System;
using System.Collections.Generic;
using Landfall.TABS;
using UnityEngine;
using UnityEngine.Events;

namespace HiddenUnits
{
	// Token: 0x02000016 RID: 22
	public class HadesEgg : MonoBehaviour
	{
		// Token: 0x06000089 RID: 137 RVA: 0x00006DF0 File Offset: 0x00004FF0
		public void AddHealth(float amount)
		{
			bool flag = this.hasHatched;
			if (!flag)
			{
				this.currentHealth += amount;
				ParticleSystem.EmissionModule emission = this.souls.emission;
				bool flag2 = this.currentHealth >= this.requiredHealth;
				if (flag2)
				{
					emission.rateOverTime = this.requiredHealth / 10f;
					this.HatchEgg();
				}
				else
				{
					emission.rateOverTime = emission.rateOverTime.constant + amount / 10f;
				}
			}
		}

		// Token: 0x0600008A RID: 138 RVA: 0x00006E84 File Offset: 0x00005084
		public void HatchEgg()
		{
			this.hatchEvent.Invoke();
			this.hasHatched = true;
		}

		// Token: 0x040000EC RID: 236
		[HideInInspector]
		public bool hasHatched;

		// Token: 0x040000ED RID: 237
		public UnityEvent hatchEvent = new UnityEvent();

		// Token: 0x040000EE RID: 238
		public ParticleSystem souls;

		// Token: 0x040000EF RID: 239
		public float currentHealth;

		// Token: 0x040000F0 RID: 240
		public float requiredHealth = 777f;

		// Token: 0x040000F1 RID: 241
		[HideInInspector]
		public List<Unit> hitList = new List<Unit>();
	}
}
