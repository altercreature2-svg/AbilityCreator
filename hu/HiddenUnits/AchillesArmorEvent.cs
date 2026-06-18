using System;
using UnityEngine;
using UnityEngine.Events;

namespace HiddenUnits
{
	// Token: 0x0200000D RID: 13
	public class AchillesArmorEvent : MonoBehaviour
	{
		// Token: 0x06000051 RID: 81 RVA: 0x000053EA File Offset: 0x000035EA
		public void OnArmorActivated()
		{
			this.armorActivationEvent.Invoke();
		}

		// Token: 0x06000052 RID: 82 RVA: 0x000053F9 File Offset: 0x000035F9
		public void OnArmorDeactivated()
		{
			this.armorDeactivationEvent.Invoke();
		}

		// Token: 0x04000091 RID: 145
		public UnityEvent armorActivationEvent = new UnityEvent();

		// Token: 0x04000092 RID: 146
		public UnityEvent armorDeactivationEvent = new UnityEvent();
	}
}
