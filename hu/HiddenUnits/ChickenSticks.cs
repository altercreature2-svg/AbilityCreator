using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Landfall.TABS;
using UnityEngine;
using UnityEngine.Events;

namespace HiddenUnits
{
	// Token: 0x02000014 RID: 20
	public class ChickenSticks : MonoBehaviour
	{
		// Token: 0x06000075 RID: 117 RVA: 0x0000643C File Offset: 0x0000463C
		public void Start()
		{
			this.rig = base.GetComponent<Rigidbody>();
			this.offset = (base.GetComponentInParent<Holdable>().hl ? new Vector3(-this.offset.x, this.offset.y, this.offset.z) : this.offset);
		}

		// Token: 0x06000076 RID: 118 RVA: 0x000064A0 File Offset: 0x000046A0
		public void Update()
		{
			this.damageCounter += Time.deltaTime;
			bool flag = this.currentState == ChickenSticks.ClubState.Idle && this.hoverTarget;
			if (flag)
			{
				this.rig.AddForce((this.hoverTarget.TransformPoint(this.offset) - base.transform.position) * this.idleForce, 5);
				this.rig.velocity *= this.idleDrag;
				this.rig.angularVelocity *= this.idleDrag;
				this.rig.AddTorque(Vector3.Cross(base.transform.forward, this.hoverTarget.forward).normalized * Vector3.Angle(base.transform.forward, this.hoverTarget.forward) * this.idleAngularForce, 5);
				this.rig.AddTorque(Vector3.Cross(base.transform.up, Vector3.up).normalized * Vector3.Angle(base.transform.up, Vector3.up) * this.idleAngularForce * 0.2f, 5);
			}
			else
			{
				bool flag2 = this.currentState == ChickenSticks.ClubState.Roaming;
				if (flag2)
				{
					bool flag3 = this.roamingTarget;
					if (flag3)
					{
						this.rig.AddForce((this.roamingTarget.data.mainRig.position - base.transform.position).normalized * this.roamingForce * this.rig.mass * Time.deltaTime);
						this.rig.AddTorque((this.roamingTarget.data.mainRig.position - base.transform.position).normalized * this.roamingTorque * this.rig.mass * Time.deltaTime);
					}
					else
					{
						this.SetTarget(this.targetingRadius);
					}
				}
			}
		}

		// Token: 0x06000077 RID: 119 RVA: 0x000066FC File Offset: 0x000048FC
		public void SetState(ChickenSticks.ClubState state)
		{
			this.currentState = state;
			this.hitList.Clear();
			bool flag = state == ChickenSticks.ClubState.Idle;
			if (flag)
			{
				this.rig.drag = this.idleDragAmount;
			}
			bool flag2 = state == ChickenSticks.ClubState.Swinging;
			if (flag2)
			{
				this.rig.drag = this.swingingDragAmount;
			}
			bool flag3 = state == ChickenSticks.ClubState.Roaming;
			if (flag3)
			{
				this.rig.drag = this.roamingDragAmount;
			}
			bool flag4 = state == ChickenSticks.ClubState.Disabled;
			if (flag4)
			{
				this.rig.drag = 0f;
			}
		}

		// Token: 0x06000078 RID: 120 RVA: 0x00006784 File Offset: 0x00004984
		public void SetStateDisabled()
		{
			this.SetState(ChickenSticks.ClubState.Disabled);
		}

		// Token: 0x06000079 RID: 121 RVA: 0x0000678F File Offset: 0x0000498F
		public void Swing()
		{
			base.StartCoroutine(this.DoSwing());
		}

		// Token: 0x0600007A RID: 122 RVA: 0x0000679F File Offset: 0x0000499F
		public IEnumerator DoSwing()
		{
			yield return new WaitUntil(() => this.currentState == ChickenSticks.ClubState.Idle);
			this.SetState(ChickenSticks.ClubState.Swinging);
			this.canDealDamage = true;
			yield return new WaitForSeconds(this.returnDelay);
			this.SetState(ChickenSticks.ClubState.Idle);
			bool flag = !this.dealDamageOutsideOfSwing;
			if (flag)
			{
				yield return new WaitForSeconds(this.disableDamageDelay);
				this.canDealDamage = false;
			}
			yield break;
		}

		// Token: 0x0600007B RID: 123 RVA: 0x000067AE File Offset: 0x000049AE
		public void Roam()
		{
			base.StartCoroutine(this.StartRoaming());
		}

		// Token: 0x0600007C RID: 124 RVA: 0x000067BE File Offset: 0x000049BE
		public IEnumerator StartRoaming()
		{
			yield return new WaitUntil(() => this.currentState == ChickenSticks.ClubState.Idle);
			this.SetState(ChickenSticks.ClubState.Roaming);
			this.SetTarget(50f);
			this.roamingBeginEvent.Invoke();
			yield return new WaitForSeconds(this.roamingTimer);
			this.SetState(ChickenSticks.ClubState.Idle);
			this.roamingEndEvent.Invoke();
			yield break;
		}

		// Token: 0x0600007D RID: 125 RVA: 0x000067D0 File Offset: 0x000049D0
		public void OnCollisionEnter(Collision col)
		{
			Unit component = col.transform.root.GetComponent<Unit>();
			bool flag = this.damageCounter < this.damageCooldown || !component || !col.rigidbody || (component && this.hitList.Contains(component)) || (component && component.Team == base.transform.root.GetComponent<Unit>().Team) || !this.canDealDamage;
			if (!flag)
			{
				this.damageCounter = 0f;
				bool flag2 = col.transform.IsChildOf(component.data.transform);
				component.data.healthHandler.TakeDamage(this.damage * (flag2 ? 1f : 0f), Vector3.zero, null, DamageType.Default);
				float num = Mathf.Clamp(col.impulse.magnitude / (base.GetComponent<Rigidbody>().mass + 10f) * 0.3f * this.impactMultiplier, 0f, 2f);
				bool flag3 = ScreenShake.Instance;
				if (flag3)
				{
					ScreenShake.Instance.AddForce(base.transform.forward * Mathf.Sqrt(num * 0.5f) * 0.5f * this.screenShakeAmount, col.contacts[0].point);
				}
				WilhelmPhysicsFunctions.AddForceWithMinWeight(component.data.mainRig, Mathf.Sqrt(num * 50f) * base.transform.forward * this.knockback * (flag2 ? 1f : 0f), 1, this.massCap);
				WilhelmPhysicsFunctions.AddForceWithMinWeight(col.rigidbody, Mathf.Sqrt(num * 50f) * base.transform.forward * this.knockback, 1, this.massCap);
				foreach (CollisionWeaponEffect collisionWeaponEffect in base.GetComponents<CollisionWeaponEffect>())
				{
					collisionWeaponEffect.DoEffect(col.transform, col);
				}
				bool flag4 = base.GetComponent<CollisionSound>();
				if (flag4)
				{
					base.GetComponent<CollisionSound>().DoEffect(col.transform, col, num);
				}
				this.hitList.Add(component);
				bool flag5 = this.currentState == ChickenSticks.ClubState.Roaming;
				if (flag5)
				{
					this.SetTarget(this.targetingRadius);
				}
			}
		}

		// Token: 0x0600007E RID: 126 RVA: 0x00006A5C File Offset: 0x00004C5C
		public void SetTarget(float range)
		{
			RaycastHit[] source = Physics.SphereCastAll(base.transform.position, range, Vector3.up, 0.1f, LayerMask.GetMask(new string[]
			{
				"MainRig"
			}));
			Unit[] array = (from hit in source
			select hit.transform.root.GetComponent<Unit>() into x
			where x && !x.data.Dead && x.Team != base.transform.root.GetComponent<Unit>().Team && !this.hitList.Contains(x)
			orderby (x.data.mainRig.transform.position - base.transform.position).magnitude
			select x).Distinct<Unit>().ToArray<Unit>();
			bool flag = array.Length != 0;
			if (flag)
			{
				this.roamingTarget = array[Random.Range(0, array.Length - 1)];
			}
		}

		// Token: 0x040000BD RID: 189
		private Rigidbody rig;

		// Token: 0x040000BE RID: 190
		public ChickenSticks.ClubState currentState;

		// Token: 0x040000BF RID: 191
		[Header("Idle")]
		public Transform hoverTarget;

		// Token: 0x040000C0 RID: 192
		public Vector3 offset;

		// Token: 0x040000C1 RID: 193
		public float idleDrag = 0.8f;

		// Token: 0x040000C2 RID: 194
		public float idleForce;

		// Token: 0x040000C3 RID: 195
		public float idleAngularForce;

		// Token: 0x040000C4 RID: 196
		public float idleDragAmount;

		// Token: 0x040000C5 RID: 197
		[Header("Swinging")]
		public float returnDelay = 1f;

		// Token: 0x040000C6 RID: 198
		public float swingingDragAmount;

		// Token: 0x040000C7 RID: 199
		[Header("Roaming")]
		public UnityEvent roamingBeginEvent = new UnityEvent();

		// Token: 0x040000C8 RID: 200
		public UnityEvent roamingEndEvent = new UnityEvent();

		// Token: 0x040000C9 RID: 201
		public float roamingTimer = 5f;

		// Token: 0x040000CA RID: 202
		public float targetingRadius = 10f;

		// Token: 0x040000CB RID: 203
		public float roamingForce;

		// Token: 0x040000CC RID: 204
		public float roamingTorque;

		// Token: 0x040000CD RID: 205
		public float roamingDragAmount;

		// Token: 0x040000CE RID: 206
		private Unit roamingTarget;

		// Token: 0x040000CF RID: 207
		private List<Unit> hitList = new List<Unit>();

		// Token: 0x040000D0 RID: 208
		[Header("Damage")]
		public float damage = 120f;

		// Token: 0x040000D1 RID: 209
		public float knockback = 100f;

		// Token: 0x040000D2 RID: 210
		public float massCap = 25f;

		// Token: 0x040000D3 RID: 211
		public float impactMultiplier = 1f;

		// Token: 0x040000D4 RID: 212
		public float screenShakeAmount = 1f;

		// Token: 0x040000D5 RID: 213
		public float damageCooldown = 0.01f;

		// Token: 0x040000D6 RID: 214
		public float disableDamageDelay = 0.3f;

		// Token: 0x040000D7 RID: 215
		public bool dealDamageOutsideOfSwing;

		// Token: 0x040000D8 RID: 216
		private float damageCounter;

		// Token: 0x040000D9 RID: 217
		private bool canDealDamage = true;

		// Token: 0x0200003C RID: 60
		public enum ClubState
		{
			// Token: 0x040001ED RID: 493
			Idle,
			// Token: 0x040001EE RID: 494
			Swinging,
			// Token: 0x040001EF RID: 495
			Roaming,
			// Token: 0x040001F0 RID: 496
			Disabled
		}
	}
}
