using System;
using System.Collections;
using System.Linq;
using Landfall.TABS;
using UnityEngine;
using UnityEngine.Events;

namespace HiddenUnits
{
	// Token: 0x02000018 RID: 24
	public class HadesTotem : MonoBehaviour
	{
		// Token: 0x06000090 RID: 144 RVA: 0x00006F58 File Offset: 0x00005158
		private void Start()
		{
			this.egg = base.GetComponentInParent<TeamHolder>().spawner.GetComponentInChildren<HadesEgg>();
		}

		// Token: 0x06000091 RID: 145 RVA: 0x00006F71 File Offset: 0x00005171
		public void Drain()
		{
			base.StartCoroutine(this.DoDrain());
		}

		// Token: 0x06000092 RID: 146 RVA: 0x00006F81 File Offset: 0x00005181
		public IEnumerator DoDrain()
		{
			Unit[] targets = this.SetTargets();
			bool flag = targets.Length != 0;
			if (flag)
			{
				int num;
				for (int i = 0; i < targets.Length; i = num + 1)
				{
					bool flag2 = i >= this.limitPerDrain;
					if (flag2)
					{
						yield break;
					}
					GameObject spawnedObject = Object.Instantiate<GameObject>(this.objectToSpawn, base.transform.position, base.transform.rotation);
					foreach (TargetableEffect targetableEffect in spawnedObject.GetComponents<TargetableEffect>())
					{
						targetableEffect.DoEffect(base.transform, targets[i].data.mainRig.transform);
						targetableEffect.DoEffect(base.transform.position, targets[i].data.mainRig.position, targets[i].data.mainRig);
						targetableEffect = null;
					}
					TargetableEffect[] array = null;
					this.egg.AddHealth(this.healthToDrain);
					this.egg.hitList.Add(targets[i]);
					this.drainEvent.Invoke();
					base.StartCoroutine(this.RemoveUnitFromList(targets[i]));
					yield return new WaitForSeconds(this.delayPerDrain);
					spawnedObject = null;
					num = i;
				}
				bool flag3 = !this.egg.hasHatched;
				if (flag3)
				{
					GameObject spawnedObjectEgg = Object.Instantiate<GameObject>(this.objectToSpawn, base.transform.position, base.transform.rotation);
					Object.Destroy(spawnedObjectEgg.GetComponent<AddTargetableEffect>());
					foreach (TargetableEffect targetableEffect2 in spawnedObjectEgg.GetComponents<TargetableEffect>())
					{
						targetableEffect2.DoEffect(base.transform, this.egg.transform);
						targetableEffect2 = null;
					}
					TargetableEffect[] array2 = null;
					spawnedObjectEgg = null;
				}
			}
			yield break;
		}

		// Token: 0x06000093 RID: 147 RVA: 0x00006F90 File Offset: 0x00005190
		public Unit[] SetTargets()
		{
			RaycastHit[] source = Physics.SphereCastAll(base.transform.position, this.radius, Vector3.up, 0.1f, this.layerMask);
			return (from hit in source
			select hit.transform.root.GetComponent<Unit>() into x
			where base.GetComponentInParent<TeamHolder>() && x && !x.data.Dead && x.Team != base.GetComponentInParent<TeamHolder>().team && !this.egg.hitList.Contains(x)
			orderby (x.data.mainRig.transform.position - base.transform.position).magnitude
			select x).Distinct<Unit>().ToArray<Unit>();
		}

		// Token: 0x06000094 RID: 148 RVA: 0x0000701F File Offset: 0x0000521F
		public IEnumerator RemoveUnitFromList(Unit unit)
		{
			yield return new WaitForSeconds(1f);
			bool flag = unit;
			if (flag)
			{
				this.egg.hitList.Remove(unit);
			}
			yield break;
		}

		// Token: 0x040000F8 RID: 248
		public UnityEvent drainEvent = new UnityEvent();

		// Token: 0x040000F9 RID: 249
		private HadesEgg egg;

		// Token: 0x040000FA RID: 250
		public LayerMask layerMask;

		// Token: 0x040000FB RID: 251
		public GameObject objectToSpawn;

		// Token: 0x040000FC RID: 252
		public float radius = 4f;

		// Token: 0x040000FD RID: 253
		public float delayPerDrain = 0.05f;

		// Token: 0x040000FE RID: 254
		public float healthToDrain = 150f;

		// Token: 0x040000FF RID: 255
		public int limitPerDrain = 3;
	}
}
