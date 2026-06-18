using System;
using System.Collections.Generic;
using System.Linq;
using Landfall.TABS;
using UnityEngine;

namespace HiddenUnits
{
	// Token: 0x0200000E RID: 14
	public class AddEffectIfTargetingSelf : MonoBehaviour
	{
		// Token: 0x06000054 RID: 84 RVA: 0x00005428 File Offset: 0x00003628
		public void DoCheck()
		{
			RaycastHit[] array = Physics.SphereCastAll(base.transform.position, this.checkRadius, Vector3.up, 0.1f, LayerMask.GetMask(new string[]
			{
				"MainRig"
			}));
			List<Unit> list = new List<Unit>();
			foreach (RaycastHit raycastHit in array)
			{
				bool flag = raycastHit.transform.root.GetComponent<Unit>() && !list.Contains(raycastHit.transform.root.GetComponent<Unit>());
				if (flag)
				{
					list.Add(raycastHit.rigidbody.transform.root.GetComponent<Unit>());
				}
			}
			Unit[] array3 = (from Unit unit in list
			where !unit.data.Dead && unit.data.targetData.unit && unit.data.targetData.unit == base.transform.root.GetComponent<Unit>()
			select unit).ToArray<Unit>();
			bool flag2 = array3.Length != 0;
			if (flag2)
			{
				foreach (Unit unit2 in array3)
				{
					UnitEffectBase unitEffectBase = UnitEffectBase.AddEffectToTarget(unit2.transform.gameObject, this.effect);
					bool flag3 = unitEffectBase == null;
					if (flag3)
					{
						GameObject gameObject = Object.Instantiate<GameObject>(this.effect.gameObject, unit2.transform.root);
						gameObject.transform.position = unit2.transform.position;
						unitEffectBase = gameObject.GetComponent<UnitEffectBase>();
						TeamHolder.AddTeamHolder(gameObject, base.transform.root.gameObject);
						unitEffectBase.DoEffect();
					}
					else
					{
						bool flag4 = !this.onlyOnce;
						if (flag4)
						{
							unitEffectBase.Ping();
						}
					}
				}
			}
		}

		// Token: 0x04000093 RID: 147
		public float checkRadius;

		// Token: 0x04000094 RID: 148
		public bool onlyOnce;

		// Token: 0x04000095 RID: 149
		public UnitEffectBase effect;
	}
}
