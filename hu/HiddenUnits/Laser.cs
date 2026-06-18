using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000006 RID: 6
public class Laser : MonoBehaviour
{
	// Token: 0x06000027 RID: 39 RVA: 0x00003A5E File Offset: 0x00001C5E
	public void Awake()
	{
		this.line = base.GetComponent<LineRenderer>();
	}

	// Token: 0x06000028 RID: 40 RVA: 0x00003A6D File Offset: 0x00001C6D
	public void Activate()
	{
		base.StartCoroutine(this.Animate(true));
	}

	// Token: 0x06000029 RID: 41 RVA: 0x00003A7E File Offset: 0x00001C7E
	public void Deactivate()
	{
		base.StartCoroutine(this.Animate(false));
	}

	// Token: 0x0600002A RID: 42 RVA: 0x00003A8F File Offset: 0x00001C8F
	public IEnumerator Animate(bool activating)
	{
		float t = 0f;
		while (t < 1f)
		{
			t += Time.deltaTime;
			this.line.widthMultiplier = Mathf.Lerp(activating ? 0f : this.scaleMultiplier, activating ? this.scaleMultiplier : 0f, Mathf.Clamp(t, 0f, 1f));
			yield return null;
		}
		yield break;
	}

	// Token: 0x0600002B RID: 43 RVA: 0x00003AA8 File Offset: 0x00001CA8
	public void Update()
	{
		this.line.SetPosition(0, this.p2.transform.position);
		RaycastHit raycastHit;
		bool flag = Physics.Raycast(this.p2.transform.position, this.p2.transform.forward, ref raycastHit, this.maxDistance, this.layer);
		if (flag)
		{
			bool flag2 = raycastHit.collider;
			if (flag2)
			{
				this.line.SetPosition(1, raycastHit.point);
				this.p1.transform.position = raycastHit.point;
				this.hitEvent.Invoke();
			}
		}
		else
		{
			this.line.SetPosition(1, this.p2.transform.forward * this.maxDistance);
			this.p1.transform.position = this.p2.transform.forward * this.maxDistance;
		}
	}

	// Token: 0x04000036 RID: 54
	private LineRenderer line;

	// Token: 0x04000037 RID: 55
	[Header("Line Settings")]
	public GameObject p1;

	// Token: 0x04000038 RID: 56
	public GameObject p2;

	// Token: 0x04000039 RID: 57
	public float maxDistance = 10f;

	// Token: 0x0400003A RID: 58
	[Header("Animation")]
	public float scaleMultiplier = 1f;

	// Token: 0x0400003B RID: 59
	[Header("Hit")]
	public UnityEvent hitEvent = new UnityEvent();

	// Token: 0x0400003C RID: 60
	public LayerMask layer;
}
