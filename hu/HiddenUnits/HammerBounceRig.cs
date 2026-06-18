using System;
using System.Collections.Generic;
using System.Linq;
using Landfall.TABS;
using UnityEngine;
using UnityEngine.Events;

namespace HiddenUnits
{
	// Token: 0x02000019 RID: 25
	public class HammerBounceRig : MonoBehaviour
	{
		// Token: 0x06000098 RID: 152 RVA: 0x0000710C File Offset: 0x0000530C
		public void Start()
		{
			this.ownRig = base.GetComponent<Rigidbody>();
			this.ownUnit = base.transform.root.GetComponent<Unit>();
			this.weapon = (base.transform.GetComponentInParent<Weapon>() ? base.transform.GetComponentInParent<Weapon>() : base.transform.root.GetComponent<Unit>().WeaponHandler.rightWeapon);
			this.returnObject = this.weapon.transform.FindChildRecursive(this.objectToReturnTo);
			this.SetTarget(100f);
		}

		// Token: 0x06000099 RID: 153 RVA: 0x000071A4 File Offset: 0x000053A4
		public void Update()
		{
			this.counter += Time.deltaTime;
			bool flag = !this.target && !this.returning;
			if (flag)
			{
				this.SetTarget(0f);
			}
			else
			{
				bool flag2 = this.target;
				if (flag2)
				{
					Vector3 vector = this.target.data.mainRig.position - base.transform.position;
					this.ownRig.AddForce(vector.normalized * (this.flightSpeed * this.ownRig.mass * Time.deltaTime));
					this.ownRig.MoveRotation(Quaternion.LookRotation(Vector3.RotateTowards(base.transform.forward, vector, Time.deltaTime * this.rotationSpeed, 0f)));
				}
			}
			bool flag3 = this.returning;
			if (flag3)
			{
				bool flag4 = this.returnCounter >= 1f;
				if (flag4)
				{
					this.weapon.GetComponent<DelayEvent>().Go();
					Object.Destroy(base.gameObject);
					this.returning = false;
				}
				else
				{
					base.transform.position = Vector3.Lerp(this.returnPosition, this.returnObject.position, this.returnCounter);
					base.transform.rotation = Quaternion.Lerp(this.returnRotation, this.returnObject.rotation, this.returnCounter);
					this.returnCounter += Time.deltaTime * this.returnSpeed;
				}
			}
		}

		// Token: 0x0600009A RID: 154 RVA: 0x00007344 File Offset: 0x00005544
		public void OnCollisionEnter(Collision col)
		{
			Unit component = col.transform.root.GetComponent<Unit>();
			bool flag = this.counter < this.cooldown || !component || !col.rigidbody || (component && this.hitList.Contains(component)) || (component && component.Team == base.GetComponent<TeamHolder>().team);
			if (!flag)
			{
				this.counter = 0f;
				bool flag2 = col.transform.IsChildOf(component.data.transform);
				component.data.healthHandler.TakeDamage(this.damage * (flag2 ? 1f : 0f), Vector3.zero, null, DamageType.Default);
				float num = Mathf.Clamp(col.impulse.magnitude / (this.ownRig.mass + 10f) * 0.3f * this.impactMultiplier, 0f, 2f);
				bool flag3 = ScreenShake.Instance;
				if (flag3)
				{
					ScreenShake.Instance.AddForce(base.transform.forward * Mathf.Sqrt(num * 0.5f) * 0.5f * this.impactScreenShake, col.contacts[0].point);
				}
				WilhelmPhysicsFunctions.AddForceWithMinWeight(component.data.mainRig, Mathf.Sqrt(num * 50f) * base.transform.forward * this.impactForce * (flag2 ? 1f : 0f), 1, this.massCap);
				WilhelmPhysicsFunctions.AddForceWithMinWeight(col.rigidbody, Mathf.Sqrt(num * 50f) * base.transform.forward * this.impactForce, 1, this.massCap);
				foreach (CollisionWeaponEffect collisionWeaponEffect in base.GetComponents<CollisionWeaponEffect>())
				{
					collisionWeaponEffect.DoEffect(col.transform, col);
				}
				bool flag4 = base.GetComponent<CollisionSound>();
				if (flag4)
				{
					base.GetComponent<CollisionSound>().DoEffect(col.transform, col, num);
				}
				this.hitCount += ((!col.transform.name.Contains("Mjolnir")) ? 1 : this.hitLimit);
				this.hitList.Add(component);
				this.SetTarget(0f);
				bool flag5 = this.hitCount >= this.hitLimit;
				if (flag5)
				{
					this.Finish();
				}
			}
		}

		// Token: 0x0600009B RID: 155 RVA: 0x000075FC File Offset: 0x000057FC
		public void SetTarget(float radius = 0f)
		{
			RaycastHit[] source = Physics.SphereCastAll(base.transform.position, (radius != 0f) ? radius : this.maxRange, Vector3.up, 0.1f, LayerMask.GetMask(new string[]
			{
				"MainRig"
			}));
			Unit[] array = (from hit in source
			select hit.transform.root.GetComponent<Unit>() into x
			where x && !x.data.Dead && x.Team != this.ownUnit.Team && !this.hitList.Contains(x)
			orderby (x.data.mainRig.transform.position - base.transform.position).magnitude
			select x).Distinct<Unit>().ToArray<Unit>();
			bool flag = array.Length != 0;
			if (flag)
			{
				this.target = array[0];
			}
			else
			{
				this.Finish();
			}
		}

		// Token: 0x0600009C RID: 156 RVA: 0x000076B6 File Offset: 0x000058B6
		public void Return()
		{
			this.returning = true;
			this.returnPosition = base.transform.position;
			this.returnRotation = base.transform.rotation;
		}

		// Token: 0x0600009D RID: 157 RVA: 0x000076E4 File Offset: 0x000058E4
		public void Finish()
		{
			bool flag = this.finished;
			if (!flag)
			{
				this.finishEvent.Invoke();
				this.finished = true;
			}
		}

		// Token: 0x04000100 RID: 256
		private float counter;

		// Token: 0x04000101 RID: 257
		private Rigidbody ownRig;

		// Token: 0x04000102 RID: 258
		private Unit target;

		// Token: 0x04000103 RID: 259
		private Unit ownUnit;

		// Token: 0x04000104 RID: 260
		private List<Unit> hitList = new List<Unit>();

		// Token: 0x04000105 RID: 261
		private int hitCount;

		// Token: 0x04000106 RID: 262
		private bool finished;

		// Token: 0x04000107 RID: 263
		[Header("Projectile Settings")]
		public int hitLimit = 10;

		// Token: 0x04000108 RID: 264
		public float maxRange = 50f;

		// Token: 0x04000109 RID: 265
		public float flightSpeed = 1f;

		// Token: 0x0400010A RID: 266
		public float rotationSpeed = 30f;

		// Token: 0x0400010B RID: 267
		[Header("Return Settings")]
		public float returnSpeed = 0.5f;

		// Token: 0x0400010C RID: 268
		public string objectToReturnTo;

		// Token: 0x0400010D RID: 269
		private bool returning;

		// Token: 0x0400010E RID: 270
		private float returnCounter;

		// Token: 0x0400010F RID: 271
		private Transform returnObject;

		// Token: 0x04000110 RID: 272
		private Weapon weapon;

		// Token: 0x04000111 RID: 273
		private Vector3 returnPosition;

		// Token: 0x04000112 RID: 274
		private Quaternion returnRotation;

		// Token: 0x04000113 RID: 275
		public UnityEvent finishEvent = new UnityEvent();

		// Token: 0x04000114 RID: 276
		[Header("Damage Settings")]
		public float cooldown = 0.01f;

		// Token: 0x04000115 RID: 277
		public float damage;

		// Token: 0x04000116 RID: 278
		public float impactMultiplier = 1f;

		// Token: 0x04000117 RID: 279
		public float impactForce;

		// Token: 0x04000118 RID: 280
		public float impactScreenShake = 1f;

		// Token: 0x04000119 RID: 281
		public float massCap;
	}
}
