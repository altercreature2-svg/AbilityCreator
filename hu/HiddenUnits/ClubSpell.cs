using System;
using System.Collections;
using Landfall.TABS;
using UnityEngine;

// Token: 0x02000002 RID: 2
public class ClubSpell : TargetableEffect
{
	// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
	public void Start()
	{
		this.startPos = base.transform.position;
	}

	// Token: 0x06000002 RID: 2 RVA: 0x00002064 File Offset: 0x00000264
	private IEnumerator Go()
	{
		float t2 = this.upCurve.keys[this.upCurve.keys.Length - 1].time;
		float c2 = 0f;
		while (c2 < t2)
		{
			c2 += Time.deltaTime;
			this.rig.transform.localPosition = new Vector3(0f, this.upCurve.Evaluate(c2) + this.rockStartY, 0f);
			yield return null;
		}
		this.rig.isKinematic = false;
		t2 = this.forceCurve.keys[this.forceCurve.keys.Length - 1].time;
		c2 = 0f;
		this.GetDirection(this.rig.position, this.target.position, this.target);
		this.rig.useGravity = false;
		while (c2 < t2)
		{
			c2 += Time.deltaTime;
			this.rig.velocity = this.direction * (this.forceCurve.Evaluate(c2) * this.force);
			yield return null;
		}
		this.rig.useGravity = true;
		yield break;
	}

	// Token: 0x06000003 RID: 3 RVA: 0x00002073 File Offset: 0x00000273
	public override void DoEffect(Transform startPoint, Transform endPoint)
	{
	}

	// Token: 0x06000004 RID: 4 RVA: 0x00002076 File Offset: 0x00000276
	public override void DoEffect(Vector3 startPoint, Vector3 endPoint, Rigidbody targetRig = null)
	{
		this.target = targetRig;
		this.rig = base.GetComponentInChildren<Rigidbody>();
		this.rockStartY = this.rig.transform.localPosition.y;
		base.StartCoroutine(this.Go());
	}

	// Token: 0x06000005 RID: 5 RVA: 0x000020B4 File Offset: 0x000002B4
	public void DoSpell()
	{
		Rigidbody targetMainRig = base.transform.root.GetComponent<Unit>().data.targetMainRig;
		this.DoEffect(this.startPos, targetMainRig.position, targetMainRig);
	}

	// Token: 0x06000006 RID: 6 RVA: 0x000020F4 File Offset: 0x000002F4
	private void GetDirection(Vector3 startPoint, Vector3 endPoint, Rigidbody targetRig)
	{
		Vector3 a = endPoint + targetRig.velocity * (this.prediction * 0.1f * Vector3.Distance(startPoint, endPoint));
		this.direction = (a - startPoint).normalized;
	}

	// Token: 0x04000001 RID: 1
	public AnimationCurve upCurve;

	// Token: 0x04000002 RID: 2
	public AnimationCurve forceCurve;

	// Token: 0x04000003 RID: 3
	public float force;

	// Token: 0x04000004 RID: 4
	private float rockStartY;

	// Token: 0x04000005 RID: 5
	private Rigidbody rig;

	// Token: 0x04000006 RID: 6
	private Vector3 direction;

	// Token: 0x04000007 RID: 7
	private Vector3 startPos;

	// Token: 0x04000008 RID: 8
	public float prediction = 1f;

	// Token: 0x04000009 RID: 9
	private Rigidbody target;
}
