using System;
using System.Collections;
using System.Collections.Generic;
using Landfall.TABS;
using Landfall.TABS.AI.Systems;
using Photon.Bolt;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Events;

namespace HiddenUnits
{
	// Token: 0x02000015 RID: 21
	public class EmperorPoofEffect : MonoBehaviour
	{
		// Token: 0x06000084 RID: 132 RVA: 0x00006C64 File Offset: 0x00004E64
		private void Start()
		{
			this.unit = base.GetComponentInParent<Unit>();
			this.m_teamSystem = World.Active.GetOrCreateManager<TeamSystem>();
			this.canDo = base.GetComponent<CanDoUnitEvents>();
			this.counter = Random.Range(0f, 0.5f);
			bool meshparticle = this.Meshparticle;
			if (meshparticle)
			{
				this.part = base.GetComponentInChildren<ParticleSystem>();
				this.emiss = this.part.shape;
				this.emiss.skinnedMeshRenderer = base.transform.root.GetComponentInChildren<SkinnedMeshRenderer>();
			}
			bool flag = this.setUnitMainRigKinematic && BoltNetwork.IsClient;
			if (flag)
			{
				this.unit.data.mainRig.isKinematic = true;
			}
		}

		// Token: 0x06000085 RID: 133 RVA: 0x00006D24 File Offset: 0x00004F24
		private void Update()
		{
			bool flag = this.automatic && this.canDo.canDoStuff && !this.done;
			if (flag)
			{
				this.counter += Time.deltaTime;
				bool flag2 = this.counter > 1f;
				if (flag2)
				{
					this.done = true;
					base.StartCoroutine(this.DoPoof());
				}
			}
		}

		// Token: 0x06000086 RID: 134 RVA: 0x00006D92 File Offset: 0x00004F92
		private IEnumerator DoPoof()
		{
			bool flag = this.part;
			if (flag)
			{
				this.part.Emit(25);
			}
			yield return new WaitForSeconds(this.moveDelay);
			List<Unit> list = (this.unit.Team == Team.Blue) ? this.m_teamSystem.GetTeamUnits(Team.Red) : this.m_teamSystem.GetTeamUnits(Team.Blue);
			Unit unit = null;
			int num2;
			for (int i = 0; i < list.Count; i = num2 + 1)
			{
				float num = Vector3.Distance(base.transform.position, list[i].data.mainRig.position);
				bool flag2 = unit;
				if (flag2)
				{
					bool flag3 = Random.value <= 0.2f || !this.useRandom;
					if (flag3)
					{
						bool flag4 = this.unitTarget == EmperorPoofEffect.UnitTarget.Furthest && num > this.currentDistance;
						if (flag4)
						{
							this.currentDistance = num;
							unit = list[i];
						}
						bool flag5 = this.unitTarget == EmperorPoofEffect.UnitTarget.Closest && num < this.currentDistance;
						if (flag5)
						{
							this.currentDistance = num;
							unit = list[i];
						}
					}
				}
				else
				{
					this.currentDistance = num;
					unit = list[i];
				}
				num2 = i;
			}
			bool flag6 = unit;
			if (flag6)
			{
				base.transform.root.position - this.unit.data.mainRig.position;
				Vector3 vector = (unit.data.mainRig.transform.position - this.unit.data.mainRig.position).normalized * ((unit.data.mainRig.transform.position - this.unit.data.mainRig.position).magnitude + this.distanceFromUnit);
				Debug.DrawLine(base.transform.position, unit.data.mainRig.transform.position, Color.blue, 1.5f);
				DataHandler componentInChildren = base.transform.root.GetComponentInChildren<DataHandler>();
				for (int j = 0; j < componentInChildren.transform.childCount; j = num2 + 1)
				{
					Transform child = componentInChildren.transform.GetChild(j);
					child.position += vector + Vector3.up * this.distanceAboveUnit;
					bool flag7 = this.unitTarget == EmperorPoofEffect.UnitTarget.Furthest;
					if (flag7)
					{
						child.Rotate(Vector3.up * 180f);
					}
					child = null;
					num2 = j;
				}
				WeaponHandler component = componentInChildren.GetComponent<WeaponHandler>();
				bool flag8 = component;
				if (flag8)
				{
					bool flag9 = component.rightWeapon;
					if (flag9)
					{
						component.rightWeapon.transform.position += vector + Vector3.up * this.distanceAboveUnit;
					}
					bool flag10 = component.leftWeapon;
					if (flag10)
					{
						component.leftWeapon.transform.position += vector + Vector3.up * this.distanceAboveUnit;
					}
				}
				this.followers = new List<PhysicsFollowBodyPart>();
				this.followers.AddRange(base.transform.root.GetComponentsInChildren<PhysicsFollowBodyPart>());
				for (int k = 0; k < this.followers.Count; k = num2 + 1)
				{
					this.followers[k].transform.position += vector + Vector3.up * this.distanceAboveUnit;
					num2 = k;
				}
				vector = default(Vector3);
				componentInChildren = null;
				component = null;
			}
			UnityEvent unityEvent = this.poofEvent;
			if (unityEvent != null)
			{
				unityEvent.Invoke();
			}
			bool flag11 = this.part;
			if (flag11)
			{
				this.part.Play();
			}
			yield break;
		}

		// Token: 0x06000087 RID: 135 RVA: 0x00006DA1 File Offset: 0x00004FA1
		public void DoThePoof()
		{
			base.StartCoroutine(this.DoPoof());
		}

		// Token: 0x040000DA RID: 218
		public EmperorPoofEffect.UnitTarget unitTarget;

		// Token: 0x040000DB RID: 219
		public bool automatic = true;

		// Token: 0x040000DC RID: 220
		public float moveDelay = 0.05f;

		// Token: 0x040000DD RID: 221
		public float distanceFromUnit = 2f;

		// Token: 0x040000DE RID: 222
		public float distanceAboveUnit = 1.5f;

		// Token: 0x040000DF RID: 223
		public bool useRandom = true;

		// Token: 0x040000E0 RID: 224
		public bool Meshparticle = true;

		// Token: 0x040000E1 RID: 225
		[Tooltip("Enable for units that have erratic movement after teleporting in a ProjectMars game.")]
		public bool setUnitMainRigKinematic;

		// Token: 0x040000E2 RID: 226
		public UnityEvent poofEvent;

		// Token: 0x040000E3 RID: 227
		private TeamSystem m_teamSystem;

		// Token: 0x040000E4 RID: 228
		private Unit unit;

		// Token: 0x040000E5 RID: 229
		private ParticleSystem.ShapeModule emiss;

		// Token: 0x040000E6 RID: 230
		private ParticleSystem part;

		// Token: 0x040000E7 RID: 231
		private List<PhysicsFollowBodyPart> followers;

		// Token: 0x040000E8 RID: 232
		private float currentDistance;

		// Token: 0x040000E9 RID: 233
		private CanDoUnitEvents canDo;

		// Token: 0x040000EA RID: 234
		private bool done;

		// Token: 0x040000EB RID: 235
		private float counter;

		// Token: 0x02000040 RID: 64
		public enum UnitTarget
		{
			// Token: 0x040001FA RID: 506
			Furthest,
			// Token: 0x040001FB RID: 507
			Closest
		}
	}
}
