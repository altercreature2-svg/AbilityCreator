using System;
using Landfall.TABS.GameState;
using UnityEngine;

namespace HiddenUnits
{
	// Token: 0x0200001E RID: 30
	public class MapCringe : GameStateListener
	{
		// Token: 0x060000B1 RID: 177 RVA: 0x00009AE8 File Offset: 0x00007CE8
		public override void OnEnterBattleState()
		{
		}

		// Token: 0x060000B2 RID: 178 RVA: 0x00009AEC File Offset: 0x00007CEC
		public override void OnEnterPlacementState()
		{
			base.gameObject.SetActive(false);
			bool flag = this.myClone;
			if (flag)
			{
				Object.Destroy(this.myClone);
			}
			this.myClone = Object.Instantiate<GameObject>(base.gameObject, base.transform.position, base.transform.rotation, base.transform.parent);
			this.myClone.SetActive(true);
			Object.Destroy(this.myClone.GetComponent<MapCringe>());
		}

		// Token: 0x04000124 RID: 292
		private GameObject myClone;
	}
}
