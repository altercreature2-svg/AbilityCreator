using System;
using System.Collections.Generic;
using UnityEngine;

namespace HiddenUnits
{
	// Token: 0x0200001B RID: 27
	public class HUChangeLayerOfRigibodies : MonoBehaviour
	{
		// Token: 0x060000A5 RID: 165 RVA: 0x00007A2C File Offset: 0x00005C2C
		private void Start()
		{
			this.rigHolder = base.transform.root.GetComponentInChildren<RigidbodyHolder>();
			this.rigs = new List<Rigidbody>();
			this.layers = new List<int>();
			this.colliderLayers = new List<int>();
			this.rigs.AddRange(this.rigHolder.AllRigs);
			this.colliders = base.transform.root.GetComponentInChildren<DataHandler>().transform.GetComponentsInChildren<Collider>();
			this.weaponHandler = base.transform.root.GetComponentInChildren<WeaponHandler>();
			bool flag = this.weaponHandler && this.includeWeapons;
			if (flag)
			{
				bool flag2 = this.weaponHandler.rightWeapon;
				if (flag2)
				{
					this.rigs.Add(this.weaponHandler.rightWeapon.rigidbody);
				}
				bool flag3 = this.weaponHandler.leftWeapon;
				if (flag3)
				{
					this.rigs.Add(this.weaponHandler.leftWeapon.rigidbody);
				}
			}
		}

		// Token: 0x060000A6 RID: 166 RVA: 0x00007B40 File Offset: 0x00005D40
		public void ChangeLayer()
		{
			for (int i = 0; i < this.rigs.Count; i++)
			{
				bool flag = this.rigs[i];
				if (flag)
				{
					this.layers.Add(this.rigs[i].gameObject.layer);
					this.rigs[i].gameObject.layer = 20;
				}
			}
			for (int j = 0; j < this.colliders.Length; j++)
			{
				bool flag2 = this.colliders[j];
				if (flag2)
				{
					this.colliderLayers.Add(this.colliders[j].gameObject.layer);
					this.colliders[j].gameObject.layer = 20;
				}
			}
		}

		// Token: 0x060000A7 RID: 167 RVA: 0x00007C20 File Offset: 0x00005E20
		public void ResetLayer()
		{
			for (int i = 0; i < this.rigs.Count; i++)
			{
				bool flag = this.rigs[i];
				if (flag)
				{
					this.rigs[i].gameObject.layer = this.layers[i];
				}
			}
			for (int j = 0; j < this.colliders.Length; j++)
			{
				bool flag2 = this.colliders[j];
				if (flag2)
				{
					this.colliders[j].gameObject.layer = this.colliderLayers[j];
				}
			}
		}

		// Token: 0x0400011A RID: 282
		public bool includeWeapons = true;

		// Token: 0x0400011B RID: 283
		private RigidbodyHolder rigHolder;

		// Token: 0x0400011C RID: 284
		private List<Rigidbody> rigs;

		// Token: 0x0400011D RID: 285
		private List<int> layers;

		// Token: 0x0400011E RID: 286
		private List<int> colliderLayers;

		// Token: 0x0400011F RID: 287
		private Collider[] colliders;

		// Token: 0x04000120 RID: 288
		private WeaponHandler weaponHandler;
	}
}
