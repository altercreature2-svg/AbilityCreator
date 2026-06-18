using System;
using Landfall.TABS;
using UnityEngine;
using UnityEngine.Events;

namespace HiddenUnits
{
	// Token: 0x0200000C RID: 12
	public class AchillesArmor : MonoBehaviour
	{
		// Token: 0x0600004D RID: 77 RVA: 0x00005094 File Offset: 0x00003294
		public void Start()
		{
			this.unit = base.transform.root.GetComponent<Unit>();
			Unit unit = this.unit;
			unit.WasDealtDamageAction = (Action<float>)Delegate.Combine(unit.WasDealtDamageAction, new Action<float>(this.Armor));
			this.armoredUnit = this.unit.gameObject.AddComponent<AchillesArmor.UnitIsArmored>();
			this.armoredUnit.projectileHitEffect = this.projectileHitEffect;
			this.armoredUnit.weaponHitEffect = this.weaponHitEffect;
			this.armoredUnit.parryForce = this.parryForce;
			this.armoredUnit.parryPower = this.parryPower;
			this.armoredUnit.blockPower = this.blockPower;
			this.maxArmorHealth = this.armorHealth;
			this.armorListeners = this.unit.GetComponentsInChildren<AchillesArmorEvent>();
			foreach (AchillesArmorEvent achillesArmorEvent in this.armorListeners)
			{
				achillesArmorEvent.OnArmorActivated();
			}
		}

		// Token: 0x0600004E RID: 78 RVA: 0x00005188 File Offset: 0x00003388
		public void Armor(float damage)
		{
			bool flag = this.armorDisabled;
			if (!flag)
			{
				this.unit.data.health += damage;
				this.armorHealth -= damage;
				this.armorHealth = Mathf.Clamp(this.armorHealth, 0f, this.maxArmorHealth);
				bool flag2 = this.armorHealth <= 0f;
				if (flag2)
				{
					this.armorDisabled = true;
					this.armorDisableEvent.Invoke();
					foreach (AchillesArmorEvent achillesArmorEvent in this.armorListeners)
					{
						achillesArmorEvent.OnArmorDeactivated();
					}
					this.armoredUnit.armorActive = false;
				}
			}
		}

		// Token: 0x0600004F RID: 79 RVA: 0x00005240 File Offset: 0x00003440
		public void Update()
		{
			bool flag = this.armorDisabled;
			if (flag)
			{
				this.armorDisabledCounter += Time.deltaTime;
				bool flag2 = this.armorDisabledCounter >= this.armorDisabledTime;
				if (flag2)
				{
					this.armorDisabledCounter = 0f;
					this.armorDisabled = false;
					this.armorHealth = this.maxArmorHealth;
					this.armorEnableEvent.Invoke();
					foreach (AchillesArmorEvent achillesArmorEvent in this.armorListeners)
					{
						achillesArmorEvent.OnArmorActivated();
					}
					this.armoredUnit.armorActive = true;
				}
			}
			else
			{
				bool flag3 = this.armorRegenerate;
				if (flag3)
				{
					this.armorHealth += Time.deltaTime * this.armorRegenerationRate;
					this.armorHealth = Mathf.Clamp(this.armorHealth, 0f, this.maxArmorHealth);
				}
				bool flag4 = this.healthRegenerate;
				if (flag4)
				{
					this.unit.data.health += Time.deltaTime * this.healthRegenerationRate;
					this.unit.data.health = Mathf.Clamp(this.unit.data.health, 0f, this.unit.data.maxHealth);
				}
			}
		}

		// Token: 0x0400007E RID: 126
		private Unit unit;

		// Token: 0x0400007F RID: 127
		private AchillesArmor.UnitIsArmored armoredUnit;

		// Token: 0x04000080 RID: 128
		private AchillesArmorEvent[] armorListeners;

		// Token: 0x04000081 RID: 129
		private bool armorDisabled;

		// Token: 0x04000082 RID: 130
		private float armorDisabledCounter;

		// Token: 0x04000083 RID: 131
		private float maxArmorHealth;

		// Token: 0x04000084 RID: 132
		[Header("Armor Settings")]
		public UnityEvent armorDisableEvent = new UnityEvent();

		// Token: 0x04000085 RID: 133
		public UnityEvent armorEnableEvent = new UnityEvent();

		// Token: 0x04000086 RID: 134
		public float armorDisabledTime = 3f;

		// Token: 0x04000087 RID: 135
		[Header("Hit Settings")]
		public GameObject projectileHitEffect;

		// Token: 0x04000088 RID: 136
		public GameObject weaponHitEffect;

		// Token: 0x04000089 RID: 137
		public float parryForce;

		// Token: 0x0400008A RID: 138
		public float parryPower;

		// Token: 0x0400008B RID: 139
		public float blockPower;

		// Token: 0x0400008C RID: 140
		[Header("Health Settings")]
		public float armorHealth = 500f;

		// Token: 0x0400008D RID: 141
		public bool armorRegenerate;

		// Token: 0x0400008E RID: 142
		public float armorRegenerationRate = 5f;

		// Token: 0x0400008F RID: 143
		public bool healthRegenerate;

		// Token: 0x04000090 RID: 144
		public float healthRegenerationRate = 50f;

		// Token: 0x02000035 RID: 53
		public class UnitIsArmored : MonoBehaviour
		{
			// Token: 0x040001C5 RID: 453
			public bool armorActive = true;

			// Token: 0x040001C6 RID: 454
			public GameObject projectileHitEffect;

			// Token: 0x040001C7 RID: 455
			public GameObject weaponHitEffect;

			// Token: 0x040001C8 RID: 456
			public float parryForce;

			// Token: 0x040001C9 RID: 457
			public float parryPower;

			// Token: 0x040001CA RID: 458
			public float blockPower;
		}
	}
}
