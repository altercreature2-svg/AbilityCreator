using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace HiddenUnits
{
	// Token: 0x02000027 RID: 39
	public class TriggerChickenStickRoam : MonoBehaviour
	{
		// Token: 0x060000E3 RID: 227 RVA: 0x0000ACC4 File Offset: 0x00008EC4
		public void StartRoaming()
		{
			foreach (ChickenSticks chickenSticks in base.transform.root.GetComponentsInChildren<ChickenSticks>())
			{
				chickenSticks.Roam();
			}
			base.StartCoroutine(this.OnStartRoaming());
		}

		// Token: 0x060000E4 RID: 228 RVA: 0x0000AD0C File Offset: 0x00008F0C
		public IEnumerator OnStartRoaming()
		{
			yield return new WaitUntil(() => base.transform.root.GetComponentsInChildren<ChickenSticks>().ToList<ChickenSticks>().TrueForAll((ChickenSticks x) => x.currentState == ChickenSticks.ClubState.Roaming));
			this.roamBeginEvent.Invoke();
			yield return new WaitUntil(() => base.transform.root.GetComponentsInChildren<ChickenSticks>().ToList<ChickenSticks>().TrueForAll((ChickenSticks x) => x.currentState == ChickenSticks.ClubState.Idle));
			this.roamEndEvent.Invoke();
			yield break;
		}

		// Token: 0x04000167 RID: 359
		public UnityEvent roamBeginEvent = new UnityEvent();

		// Token: 0x04000168 RID: 360
		public UnityEvent roamEndEvent = new UnityEvent();
	}
}
