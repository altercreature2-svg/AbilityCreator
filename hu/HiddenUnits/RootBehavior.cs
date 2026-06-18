using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Landfall.TABS;
using RootMotion.FinalIK;
using UnityEngine;
using UnityEngine.Events;

namespace HiddenUnits
{
	// Token: 0x02000023 RID: 35
	public class RootBehavior : MonoBehaviour
	{
		// Token: 0x060000C5 RID: 197 RVA: 0x0000A31B File Offset: 0x0000851B
		private void Start()
		{
			this.team = base.GetComponent<TeamHolder>();
		}

		// Token: 0x060000C6 RID: 198 RVA: 0x0000A32C File Offset: 0x0000852C
		private void Update()
		{
			this.counter += Time.deltaTime;
			bool flag = this.unitTarget && !this.joint && Vector3.Distance(this.tip.position, this.unitTarget.data.mainRig.position) < this.attachDistance && this.unitTarget.data.mainRig.GetComponents<ConfigurableJoint>().Length < 3;
			if (flag)
			{
				base.StartCoroutine(this.AttachJoint());
			}
			else
			{
				bool flag2 = this.unitTarget && this.joint && this.doConstantDamage;
				if (flag2)
				{
					this.unitTarget.data.healthHandler.TakeDamage(this.damage * Time.deltaTime, Vector3.zero, null, DamageType.Default);
				}
			}
		}

		// Token: 0x060000C7 RID: 199 RVA: 0x0000A417 File Offset: 0x00008617
		public void DoRooting()
		{
			base.StartCoroutine(this.ChooseTarget());
		}

		// Token: 0x060000C8 RID: 200 RVA: 0x0000A427 File Offset: 0x00008627
		private IEnumerator ChooseTarget()
		{
			this.SetTarget();
			bool flag = this.unitTarget;
			if (flag)
			{
				float t = 0f;
				Vector3 beginPosition = this.moveTarget.position;
				while (t < 1f && !this.joint && this.unitTarget)
				{
					t += Time.deltaTime * this.followSpeed;
					this.moveTarget.position = Vector3.Lerp(beginPosition, this.unitTarget.data.mainRig.position, Mathf.Clamp(t, 0f, 1f));
					yield return null;
				}
				beginPosition = default(Vector3);
			}
			yield break;
		}

		// Token: 0x060000C9 RID: 201 RVA: 0x0000A436 File Offset: 0x00008636
		public IEnumerator AttachJoint()
		{
			bool flag = !this.unitTarget;
			if (flag)
			{
				yield break;
			}
			foreach (Rigidbody rig in this.rigs)
			{
				rig.isKinematic = false;
				rig = null;
			}
			List<Rigidbody>.Enumerator enumerator = default(List<Rigidbody>.Enumerator);
			base.GetComponent<CCDIK>().enabled = false;
			this.joint = this.unitTarget.data.mainRig.gameObject.AddComponent<FixedJoint>();
			this.joint.connectedBody = this.tip.GetComponent<Rigidbody>();
			this.joint.breakForce = this.breakForce;
			this.joint.breakTorque = this.breakForce;
			bool flag2 = this.lerpToCenterOfTip;
			if (flag2)
			{
				base.StartCoroutine(this.LerpJoint());
			}
			bool flag3 = !this.doConstantDamage;
			if (flag3)
			{
				this.unitTarget.data.healthHandler.TakeDamage(this.damage, Vector3.zero, null, DamageType.Default);
			}
			this.hitEvent.Invoke();
			RemoveAfterSeconds seconds = base.GetComponent<RemoveAfterSeconds>();
			yield return new WaitUntil(() => this.counter >= seconds.seconds - 1f);
			bool flag4 = this.joint;
			if (flag4)
			{
				Object.Destroy(this.joint);
			}
			yield break;
		}

		// Token: 0x060000CA RID: 202 RVA: 0x0000A445 File Offset: 0x00008645
		public IEnumerator LerpJoint()
		{
			bool flag = !this.joint;
			if (flag)
			{
				yield break;
			}
			float t = 0f;
			Vector3 initialVector = this.joint.connectedAnchor;
			while (t < 1f && this.joint)
			{
				this.joint.autoConfigureConnectedAnchor = false;
				t += Time.deltaTime * this.adjustTime;
				this.joint.connectedAnchor = Vector3.Lerp(initialVector, Vector3.zero, Mathf.Clamp(t, 0f, 1f));
				yield return null;
			}
			yield break;
		}

		// Token: 0x060000CB RID: 203 RVA: 0x0000A454 File Offset: 0x00008654
		public void SetTarget()
		{
			RaycastHit[] source = Physics.SphereCastAll(base.transform.position, this.targetRange, Vector3.up, 0.1f, LayerMask.GetMask(new string[]
			{
				"MainRig"
			}));
			Unit[] array = (from hit in source
			select hit.transform.root.GetComponent<Unit>() into x
			where x && !x.data.Dead && x.Team != this.team.team
			orderby (x.data.mainRig.transform.position - base.transform.position).magnitude
			select x).Distinct<Unit>().ToArray<Unit>();
			bool flag = array.Length != 0;
			if (flag)
			{
				this.unitTarget = array[0];
			}
		}

		// Token: 0x0400013C RID: 316
		private float counter;

		// Token: 0x0400013D RID: 317
		private Unit unitTarget;

		// Token: 0x0400013E RID: 318
		private FixedJoint joint;

		// Token: 0x0400013F RID: 319
		private TeamHolder team;

		// Token: 0x04000140 RID: 320
		[Header("Root Settings")]
		public List<Rigidbody> rigs = new List<Rigidbody>();

		// Token: 0x04000141 RID: 321
		public Transform moveTarget;

		// Token: 0x04000142 RID: 322
		public Transform tip;

		// Token: 0x04000143 RID: 323
		public float targetRange = 3f;

		// Token: 0x04000144 RID: 324
		public float followSpeed = 1f;

		// Token: 0x04000145 RID: 325
		[Header("Joint Settings")]
		public float adjustTime = 1f;

		// Token: 0x04000146 RID: 326
		public float attachDistance = 1f;

		// Token: 0x04000147 RID: 327
		public float breakForce = 100000f;

		// Token: 0x04000148 RID: 328
		public bool lerpToCenterOfTip;

		// Token: 0x04000149 RID: 329
		[Header("Damage Settings")]
		public UnityEvent hitEvent = new UnityEvent();

		// Token: 0x0400014A RID: 330
		public float damage = 20f;

		// Token: 0x0400014B RID: 331
		public bool doConstantDamage;
	}
}
