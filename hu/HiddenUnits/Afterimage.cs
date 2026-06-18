using System;
using System.Collections;
using Landfall.TABS;
using Landfall.TABS.AI;
using UnityEngine;

namespace HiddenUnits
{
	// Token: 0x0200000F RID: 15
	public class Afterimage : MonoBehaviour
	{
		// Token: 0x06000057 RID: 87 RVA: 0x00005648 File Offset: 0x00003848
		public void Start()
		{
			base.GetComponent<UnitSpawner>().unitBlueprint = base.transform.root.GetComponent<Unit>().unitBlueprint;
		}

		// Token: 0x06000058 RID: 88 RVA: 0x0000566B File Offset: 0x0000386B
		public void SpawnAfterimage()
		{
			base.StartCoroutine(this.Spawn());
		}

		// Token: 0x06000059 RID: 89 RVA: 0x0000567B File Offset: 0x0000387B
		public IEnumerator Spawn()
		{
			base.transform.position = base.transform.root.GetComponent<Unit>().data.mainRig.position;
			Unit u = base.GetComponent<UnitSpawner>().Spawn();
			u.name = "AFTERIMAGE";
			u.transform.position = new Vector3(base.transform.root.GetComponent<Unit>().data.mainRig.position.x, base.transform.root.GetComponent<Unit>().data.mainRig.position.y - 1.353508f, base.transform.root.GetComponent<Unit>().data.mainRig.position.z);
			u.data.GetComponent<UnitColorHandler>().SetMaterial(this.imgMaterial);
			u.GetComponent<UnitAPI>().forceSupressFromWinCondition = true;
			u.targetingPriorityMultiplier = 0.1f;
			foreach (ConditionalEvent move in u.GetComponentsInChildren<ConditionalEvent>())
			{
				Object.Destroy(move.gameObject);
				move = null;
			}
			ConditionalEvent[] array = null;
			Object.Instantiate<GameObject>(this.poofEffect, u.data.mainRig.position, this.poofEffect.transform.rotation);
			yield return new WaitForSeconds(0.1f);
			Object.Instantiate<GameObject>(this.poofEffect, base.transform.root.GetComponent<Unit>().data.mainRig.position, this.poofEffect.transform.rotation);
			yield return new WaitForSeconds(this.fadeTime - 0.1f);
			Object.Instantiate<GameObject>(this.poofEffect, u.data.mainRig.position, this.poofEffect.transform.rotation);
			yield return new WaitForSeconds(this.destroyDelay);
			foreach (TrailRenderer trail in u.GetComponentsInChildren<TrailRenderer>())
			{
				trail.transform.SetParent(null);
				trail.emitting = false;
				trail.gameObject.AddComponent<RemoveAfterSeconds>().seconds = trail.time * 1.5f;
				trail = null;
			}
			TrailRenderer[] array2 = null;
			u.DestroyUnit();
			yield break;
		}

		// Token: 0x04000096 RID: 150
		public Material imgMaterial;

		// Token: 0x04000097 RID: 151
		public GameObject poofEffect;

		// Token: 0x04000098 RID: 152
		public float fadeTime = 5f;

		// Token: 0x04000099 RID: 153
		public float destroyDelay = 0.2f;
	}
}
