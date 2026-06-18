using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HiddenUnits
{
	// Token: 0x02000017 RID: 23
	public class HadesSpawnObjects : MonoBehaviour
	{
		// Token: 0x0600008C RID: 140 RVA: 0x00006EC4 File Offset: 0x000050C4
		public void Start()
		{
			this.teamHolder = (base.GetComponent<TeamHolder>() ?? base.gameObject.AddComponent<TeamHolder>());
			bool flag = this.spawnOnStart;
			if (flag)
			{
				this.SpawnObjects();
			}
		}

		// Token: 0x0600008D RID: 141 RVA: 0x00006F00 File Offset: 0x00005100
		public void SpawnObjects()
		{
			base.StartCoroutine(this.DoSpawning());
		}

		// Token: 0x0600008E RID: 142 RVA: 0x00006F10 File Offset: 0x00005110
		public IEnumerator DoSpawning()
		{
			int num;
			for (int i = 0; i < this.amountToSpawn; i = num + 1)
			{
				Vector3 point = new Vector3(base.transform.position.x + Random.Range(-this.radius, this.radius), base.transform.position.y + 2f, base.transform.position.z + Random.Range(-this.radius, this.radius));
				GameObject spawnedObject = Object.Instantiate<GameObject>(this.objectsToSpawn[Random.Range(0, this.objectsToSpawn.Count)], point, Quaternion.LookRotation(Vector3.up));
				TeamHolder team = spawnedObject.AddComponent<TeamHolder>();
				team.team = this.teamHolder.team;
				team.spawner = this.teamHolder.spawner;
				yield return new WaitForSeconds(this.delayPerSpawn);
				point = default(Vector3);
				spawnedObject = null;
				team = null;
				num = i;
			}
			yield break;
		}

		// Token: 0x040000F2 RID: 242
		private TeamHolder teamHolder;

		// Token: 0x040000F3 RID: 243
		public List<GameObject> objectsToSpawn = new List<GameObject>();

		// Token: 0x040000F4 RID: 244
		public int amountToSpawn = 10;

		// Token: 0x040000F5 RID: 245
		public float radius = 8f;

		// Token: 0x040000F6 RID: 246
		public float delayPerSpawn = 0.08f;

		// Token: 0x040000F7 RID: 247
		public bool spawnOnStart = true;
	}
}
