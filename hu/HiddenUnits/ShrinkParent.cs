using System;
using UnityEngine;

namespace HiddenUnits
{
	// Token: 0x02000025 RID: 37
	public class ShrinkParent : MonoBehaviour
	{
		// Token: 0x060000DA RID: 218 RVA: 0x0000AB60 File Offset: 0x00008D60
		public void Shrink()
		{
			this.scaling = true;
		}

		// Token: 0x060000DB RID: 219 RVA: 0x0000AB6C File Offset: 0x00008D6C
		private void Update()
		{
			this.counter += Time.deltaTime;
			bool flag = this.scaling && this.counter < 1f;
			if (flag)
			{
				base.transform.parent.localScale -= Vector3.one * (this.scaler * Time.deltaTime);
			}
		}

		// Token: 0x0400015C RID: 348
		private float counter;

		// Token: 0x0400015D RID: 349
		private bool scaling;

		// Token: 0x0400015E RID: 350
		public float scaler = 0.5f;
	}
}
