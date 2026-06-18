using System;
using System.Collections;
using Landfall.TABS;
using UnityEngine;
using UnityEngine.Events;

namespace HiddenUnits
{
	// Token: 0x02000022 RID: 34
	public class ProjectileHitScale : ProjectileHitEffect
	{
		// Token: 0x060000C1 RID: 193 RVA: 0x0000A1E0 File Offset: 0x000083E0
		private void Awake()
		{
			bool infiniteScalingEnabled = HUMain.InfiniteScalingEnabled;
			if (infiniteScalingEnabled)
			{
				this.scaleLimit = 9999;
			}
		}

		// Token: 0x060000C2 RID: 194 RVA: 0x0000A204 File Offset: 0x00008404
		public override bool DoEffect(HitData hit)
		{
			bool flag = !hit.rigidbody || !hit.transform.root.GetComponent<Unit>() || (hit.transform.root.GetComponent<Unit>() && hit.transform.root.GetComponent<Unit>().data.Dead) || (hit.transform.root.GetComponent<Unit>() && hit.transform.root.GetComponent<Unit>().Team == base.GetComponent<TeamHolder>().team) || hit.transform.GetComponent<ProjectileHitScale.Scaling>();
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				base.StartCoroutine(this.DoScaling(hit));
				result = true;
			}
			return result;
		}

		// Token: 0x060000C3 RID: 195 RVA: 0x0000A2D4 File Offset: 0x000084D4
		private IEnumerator DoScaling(HitData hit)
		{
			hit.transform.gameObject.AddComponent<ProjectileHitScale.Scaling>();
			yield return new WaitForSeconds(this.scaleDelay);
			this.scaleEvent.Invoke();
			float t = 0f;
			Vector3 originalVector = hit.transform.localScale;
			while (t < this.scaleCurve.keys[this.scaleCurve.keys.Length - 1].time && hit.transform.localScale.magnitude > 0.1f)
			{
				hit.transform.localScale = originalVector * (this.scaleCurve.Evaluate(t) * this.scaleCurveMultiplier);
				t += Time.deltaTime;
				yield return null;
			}
			this.scaleCount++;
			bool flag = this.scaleCount >= this.scaleLimit;
			if (flag)
			{
				Object.Destroy(hit.transform.GetComponent<ProjectileHitScale.Scaling>());
			}
			yield break;
		}

		// Token: 0x04000136 RID: 310
		private int scaleCount;

		// Token: 0x04000137 RID: 311
		public float scaleDelay = 0.5f;

		// Token: 0x04000138 RID: 312
		public int scaleLimit = 1;

		// Token: 0x04000139 RID: 313
		public AnimationCurve scaleCurve;

		// Token: 0x0400013A RID: 314
		public float scaleCurveMultiplier = 1f;

		// Token: 0x0400013B RID: 315
		public UnityEvent scaleEvent = new UnityEvent();

		// Token: 0x02000051 RID: 81
		public class Scaling : MonoBehaviour
		{
		}
	}
}
