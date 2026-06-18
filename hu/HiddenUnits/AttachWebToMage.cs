using System;
using UnityEngine;

namespace HiddenUnits
{
	// Token: 0x02000011 RID: 17
	public class AttachWebToMage : MonoBehaviour
	{
		// Token: 0x06000060 RID: 96 RVA: 0x00005992 File Offset: 0x00003B92
		public void Start()
		{
			this.Attach();
		}

		// Token: 0x06000061 RID: 97 RVA: 0x0000599C File Offset: 0x00003B9C
		public void Attach()
		{
			bool flag = !base.transform.parent.parent.GetComponent<TeamHolder>() || !base.transform.parent.parent.GetComponent<TeamHolder>().spawnerWeapon || !base.transform.parent.parent.GetComponent<TeamHolder>().spawnerWeapon.transform;
			if (!flag)
			{
				base.transform.SetParent(base.transform.parent.parent.GetComponent<TeamHolder>().spawnerWeapon.transform);
			}
		}
	}
}
