using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Landfall.TABS;
using UnityEngine;

namespace HiddenUnits
{
	// Token: 0x0200001F RID: 31
	public class MeleeWeaponLightning : CollisionWeaponEffect
	{
		// Token: 0x060000B4 RID: 180 RVA: 0x00009B7D File Offset: 0x00007D7D
		private void Start()
		{
			this.ownUnit = base.GetComponent<Weapon>().connectedData.unit;
		}

		// Token: 0x060000B5 RID: 181 RVA: 0x00009B98 File Offset: 0x00007D98
		public override void DoEffect(Transform hitTransform, Collision collision)
		{
			bool flag = !hitTransform.root.GetComponent<Unit>() || hitTransform.root.GetComponent<Unit>().Team == this.ownUnit.Team || hitTransform.root.GetComponent<Unit>().data.Dead;
			if (!flag)
			{
				this.oldTarget = hitTransform.root.GetComponent<Unit>();
				this.hitList.Add(this.oldTarget);
				this.oldTarget.data.healthHandler.TakeDamage(this.damage, Vector3.zero, null, DamageType.Default);
				this.SetTarget(base.transform.position);
				base.StartCoroutine(this.DoLightning());
			}
		}

		// Token: 0x060000B6 RID: 182 RVA: 0x00009C59 File Offset: 0x00007E59
		public IEnumerator DoLightning()
		{
			int num;
			for (int i = 0; i < this.chainCount; i = num + 1)
			{
				for (int j = 0; j < this.consecutiveChains; j = num + 1)
				{
					bool flag = this.target && this.target.data && this.target.data.healthHandler && this.lineObject && this.oldTarget;
					if (flag)
					{
						GameObject line = Object.Instantiate<GameObject>(this.lineObject, this.oldTarget.transform, true);
						line.transform.FindChildRecursive("T1").position = this.oldTarget.data.mainRig.position;
						line.transform.FindChildRecursive("T2").position = this.target.data.mainRig.position;
						this.target.data.healthHandler.TakeDamage(this.damage, Vector3.zero, null, DamageType.Default);
						this.hitList.Add(this.target);
						this.oldTarget = this.target;
						line = null;
					}
					bool flag2 = this.oldTarget;
					if (flag2)
					{
						this.SetTarget(this.oldTarget.data.mainRig.position);
					}
					num = j;
				}
				yield return new WaitForSeconds(0.1f);
				num = i;
			}
			this.hitList.Clear();
			yield break;
		}

		// Token: 0x060000B7 RID: 183 RVA: 0x00009C68 File Offset: 0x00007E68
		public void SetTarget(Vector3 source)
		{
			this.target = null;
			RaycastHit[] source2 = Physics.SphereCastAll(base.transform.position, this.maxTargetRange, Vector3.up, 0.1f, LayerMask.GetMask(new string[]
			{
				"MainRig"
			}));
			Unit[] array = (from hit in source2
			select hit.transform.root.GetComponent<Unit>() into x
			where x && !x.data.Dead && x.Team != this.ownUnit.Team && !this.hitList.Contains(x)
			orderby (x.data.mainRig.transform.position - source).magnitude
			select x).Distinct<Unit>().ToArray<Unit>();
			bool flag = array.Length != 0;
			if (flag)
			{
				this.target = array[0];
			}
		}

		// Token: 0x04000125 RID: 293
		private Unit target;

		// Token: 0x04000126 RID: 294
		private Unit oldTarget;

		// Token: 0x04000127 RID: 295
		private Unit ownUnit;

		// Token: 0x04000128 RID: 296
		private List<Unit> hitList = new List<Unit>();

		// Token: 0x04000129 RID: 297
		public float maxTargetRange = 6f;

		// Token: 0x0400012A RID: 298
		public int chainCount = 20;

		// Token: 0x0400012B RID: 299
		public float damage = 1000f;

		// Token: 0x0400012C RID: 300
		public GameObject lineObject;

		// Token: 0x0400012D RID: 301
		public int consecutiveChains = 1;
	}
}
