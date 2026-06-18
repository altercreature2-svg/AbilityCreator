using System;
using System.Collections;
using System.Linq;
using Landfall.TABS;
using UnityEngine;

namespace HiddenUnits
{
	// Token: 0x02000010 RID: 16
	public class AngelBubble : MonoBehaviour
	{
		// Token: 0x0600005B RID: 91 RVA: 0x000056AC File Offset: 0x000038AC
		private void Start()
		{
			this.effectTransform = base.transform.GetChild(0);
			Collider[] source = Physics.OverlapSphere(this.effectTransform.position, this.radius);
			this.hitRigs = (from x in source
			select x.attachedRigidbody into x
			where x
			select x).Distinct<Rigidbody>().ToArray<Rigidbody>();
			this.hitUnits = (from hit in source
			select hit.transform.root.GetComponent<Unit>() into x
			where x && !x.data.Dead
			orderby (x.data.mainRig.transform.position - base.transform.position).magnitude
			select x).Distinct<Unit>().ToArray<Unit>();
			base.StartCoroutine(this.AnimateSpell());
		}

		// Token: 0x0600005C RID: 92 RVA: 0x000057B3 File Offset: 0x000039B3
		private IEnumerator AnimateSpell()
		{
			float time = this.upCurve.keys[this.upCurve.keys.Length - 1].time;
			float t = 0f;
			Vector3 startPos = this.effectTransform.position;
			while (t < time)
			{
				t += Time.deltaTime;
				this.Pull();
				this.effectTransform.position = startPos + Vector3.up * this.upCurve.Evaluate(t);
				yield return null;
			}
			yield break;
		}

		// Token: 0x0600005D RID: 93 RVA: 0x000057C4 File Offset: 0x000039C4
		private void Pull()
		{
			foreach (Rigidbody rigidbody in from x in this.hitRigs
			where x
			select x)
			{
				Vector3 position = this.effectTransform.position;
				float num = this.influenceCurve.Evaluate(Vector3.Distance(rigidbody.position, position));
				float num2 = Random.Range(0.5f, 1f);
				WilhelmPhysicsFunctions.AddForceWithMinWeight(rigidbody, num2 * this.force * num * (position + Random.insideUnitSphere * 2f - rigidbody.position).normalized, 0, this.minMassCap);
				rigidbody.AddForce(-200f * num2 * Time.deltaTime * rigidbody.velocity, 5);
			}
			foreach (Unit unit in this.hitUnits)
			{
				unit.data.healthHandler.TakeDamage(this.damageOverTime * Time.deltaTime, Vector3.up, null, DamageType.Default);
			}
		}

		// Token: 0x0400009A RID: 154
		private Rigidbody[] hitRigs;

		// Token: 0x0400009B RID: 155
		private Unit[] hitUnits;

		// Token: 0x0400009C RID: 156
		private Transform effectTransform;

		// Token: 0x0400009D RID: 157
		[Header("Damage Settings")]
		public float radius = 5f;

		// Token: 0x0400009E RID: 158
		public float force = 1000f;

		// Token: 0x0400009F RID: 159
		public float minMassCap = 60f;

		// Token: 0x040000A0 RID: 160
		public float damageOverTime = 60f;

		// Token: 0x040000A1 RID: 161
		[Header("Curve Settings")]
		public AnimationCurve influenceCurve;

		// Token: 0x040000A2 RID: 162
		public AnimationCurve upCurve;
	}
}
