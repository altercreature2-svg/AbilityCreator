using System;
using UnityEngine;
using UnityEngine.Events;

namespace HiddenUnits
{
	// Token: 0x02000021 RID: 33
	public class OdinShield : ProjectileSurfaceEffect
	{
		// Token: 0x060000BB RID: 187 RVA: 0x00009FF9 File Offset: 0x000081F9
		public void Start()
		{
			this.health = this.maxHealth;
			this.ReflectionRune();
		}

		// Token: 0x060000BC RID: 188 RVA: 0x0000A00F File Offset: 0x0000820F
		public void ReflectionRune()
		{
			this.shieldState = OdinShield.ShieldStates.ReflectionRune;
			this.activateEvent.Invoke();
		}

		// Token: 0x060000BD RID: 189 RVA: 0x0000A025 File Offset: 0x00008225
		public void Deactivate()
		{
			this.shieldState = OdinShield.ShieldStates.Deactivated;
			this.deactivateEvent.Invoke();
		}

		// Token: 0x060000BE RID: 190 RVA: 0x0000A03C File Offset: 0x0000823C
		public override bool DoEffect(HitData hit, GameObject projectile)
		{
			bool flag = this.shieldState > OdinShield.ShieldStates.ReflectionRune;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = projectile.GetComponent<ProjectileHit>();
				if (flag2)
				{
					this.health -= projectile.GetComponent<ProjectileHit>().damage;
				}
				bool flag3 = projectile.GetComponent<CollisionWeapon>();
				if (flag3)
				{
					this.health -= projectile.GetComponent<CollisionWeapon>().damage;
				}
				bool flag4 = this.health <= 0f;
				if (flag4)
				{
					this.counter = 0f;
					this.Deactivate();
					result = false;
				}
				else
				{
					bool flag5 = projectile.GetComponent<TeamHolder>();
					if (flag5)
					{
						projectile.GetComponent<TeamHolder>().SwitchTeam();
						projectile.GetComponent<TeamHolder>().spawner = null;
					}
					projectile.GetComponent<MoveTransform>().velocity *= -1f;
					projectile.GetComponent<RaycastTrail>().ignoredFrames = 2;
					this.reflectEvent.Invoke();
					result = true;
				}
			}
			return result;
		}

		// Token: 0x060000BF RID: 191 RVA: 0x0000A144 File Offset: 0x00008344
		public void Update()
		{
			this.counter += Time.deltaTime;
			bool flag = this.counter > this.rechargeTime && this.health <= 0f;
			if (flag)
			{
				this.health = this.maxHealth;
				this.ReflectionRune();
			}
		}

		// Token: 0x0400012E RID: 302
		private OdinShield.ShieldStates shieldState;

		// Token: 0x0400012F RID: 303
		private float counter;

		// Token: 0x04000130 RID: 304
		private float health;

		// Token: 0x04000131 RID: 305
		public float rechargeTime = 6f;

		// Token: 0x04000132 RID: 306
		public UnityEvent activateEvent = new UnityEvent();

		// Token: 0x04000133 RID: 307
		public UnityEvent deactivateEvent = new UnityEvent();

		// Token: 0x04000134 RID: 308
		public UnityEvent reflectEvent = new UnityEvent();

		// Token: 0x04000135 RID: 309
		public float maxHealth = 1500f;

		// Token: 0x02000050 RID: 80
		private enum ShieldStates
		{
			// Token: 0x04000259 RID: 601
			ReflectionRune,
			// Token: 0x0400025A RID: 602
			Deactivated
		}
	}
}
