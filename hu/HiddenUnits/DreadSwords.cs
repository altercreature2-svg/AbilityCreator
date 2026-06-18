using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Landfall.TABC;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000004 RID: 4
public class DreadSwords : MonoBehaviour
{
	// Token: 0x14000001 RID: 1
	// (add) Token: 0x06000014 RID: 20 RVA: 0x00002CA4 File Offset: 0x00000EA4
	// (remove) Token: 0x06000015 RID: 21 RVA: 0x00002CDC File Offset: 0x00000EDC
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event DreadSwords.AttackedEventHandler Attacked;

	// Token: 0x06000016 RID: 22 RVA: 0x00002D11 File Offset: 0x00000F11
	private void Awake()
	{
		this.swordPoints = base.GetComponentsInChildren<ShootPosition>();
	}

	// Token: 0x06000017 RID: 23 RVA: 0x00002D20 File Offset: 0x00000F20
	private void Start()
	{
		this.data = base.transform.root.GetComponentInChildren<DataHandler>();
		List<Renderer> list = new List<Renderer>();
		for (int i = 0; i < this.swordPoints.Length; i++)
		{
			SpookySword spookySword = this.CreateNewSword(this.swordPoints[i].transform.position + Vector3.up * 2f, this.swordPoints[i].transform.rotation);
			Renderer[] componentsInChildren = spookySword.gameObject.GetComponentsInChildren<Renderer>();
			bool flag = componentsInChildren != null && componentsInChildren.Length != 0;
			if (flag)
			{
				list.AddRange(componentsInChildren);
			}
			this.swords.Add(spookySword);
		}
		bool flag2 = this.data != null && this.data.unit != null;
		if (flag2)
		{
			this.data.unit.AddRenderersToShowHide(list.ToArray(), this.data.unit.IsSpawnedInBlindPlacement);
		}
		for (int j = 0; j < this.swords.Count; j++)
		{
			bool flag3 = !(this.swords[j].gameObject == null);
			if (flag3)
			{
				this.swords[j].gameObject.transform.position = this.swordPoints[j].transform.position;
				this.swords[j].gameObject.transform.rotation = this.swordPoints[j].transform.rotation;
			}
		}
	}

	// Token: 0x06000018 RID: 24 RVA: 0x00002EDC File Offset: 0x000010DC
	private void Update()
	{
		bool flag = this.done;
		if (!flag)
		{
			bool flag2 = this.data && this.data.Dead;
			if (flag2)
			{
				this.done = true;
				for (int i = 0; i < this.swords.Count; i++)
				{
					this.attackID = i;
					this.Attack(this.data.mainRig, this.attackID);
				}
			}
			float num = Mathf.Clamp(Time.deltaTime, 0f, 0.02f);
			bool flag3 = this.data.weaponHandler;
			if (flag3)
			{
				this.attackspeedMulti = this.data.weaponHandler.attackSpeedMultiplier;
			}
			bool flag4 = this.data.unit == null || !this.data.unit.IsRemotelyControlled;
			this.counter += Time.deltaTime * this.attackspeedMulti;
			bool flag5 = flag4 && this.data.targetMainRig && this.counter > this.attackRate;
			if (flag5)
			{
				float num2 = 999f;
				for (int j = 0; j < this.swordPoints.Length; j++)
				{
					float num3 = Vector3.Angle(this.data.targetMainRig.position - base.transform.position, this.swordPoints[j].gameObject.transform.position - base.transform.position);
					bool flag6 = num3 < num2 && this.swords[j].sinceSpawn > 1.5f;
					if (flag6)
					{
						num2 = num3;
						this.attackID = j;
					}
				}
				bool flag7 = this.data.targetMainRig && this.data.distanceToTarget < this.throwRange;
				if (flag7)
				{
					this.Attack(this.data.targetMainRig, this.attackID);
				}
			}
			for (int k = 0; k < this.swords.Count; k++)
			{
				bool flag8 = !(this.swords[k].gameObject == null);
				if (flag8)
				{
					this.swords[k].sinceSpawn += num;
					this.swords[k].gameObject.transform.position = this.swordPoints[k].transform.position;
					this.swords[k].gameObject.transform.rotation = this.swordPoints[k].transform.rotation;
				}
			}
		}
	}

	// Token: 0x06000019 RID: 25 RVA: 0x000031DC File Offset: 0x000013DC
	public void Attack(Rigidbody target, int useAttackID)
	{
		this.counter = 0f;
		bool flag = useAttackID < 0 || useAttackID >= this.swords.Count;
		if (flag)
		{
			useAttackID = Random.Range(0, this.swords.Count);
		}
		SpookySword spookySword = this.swords[useAttackID];
		bool flag2 = spookySword != null;
		if (flag2)
		{
			base.StartCoroutine(this.DoAttack(spookySword, target));
			this.shootEvent.Invoke();
			bool flag3 = !this.done;
			if (flag3)
			{
				this.swords[useAttackID] = this.CreateNewSword(this.swordPoints[useAttackID].gameObject.transform.position, this.swordPoints[useAttackID].gameObject.transform.rotation);
			}
			else
			{
				spookySword.gameObject.GetComponent<ProjectileHit>().ignoreTeamMates = false;
			}
			DreadSwords.AttackedEventHandler attacked = this.Attacked;
			if (attacked != null)
			{
				attacked(target, useAttackID);
			}
		}
	}

	// Token: 0x0600001A RID: 26 RVA: 0x000032DC File Offset: 0x000014DC
	private void OnDestroy()
	{
		base.StopAllCoroutines();
		for (int i = 0; i < this.swords.Count; i++)
		{
			this.swords[i].gameObject != null;
			Object.Destroy(this.swords[i].gameObject);
		}
	}

	// Token: 0x0600001B RID: 27 RVA: 0x0000333C File Offset: 0x0000153C
	private IEnumerator DoAttack(SpookySword attackSword, Rigidbody targ)
	{
		bool flag = !attackSword.gameObject;
		if (flag)
		{
			yield break;
		}
		float counter2 = 0f;
		float t = this.throwCurve.keys[this.throwCurve.keys.Length - 1].time;
		ProjectileStick stick = attackSword.gameObject.GetComponent<ProjectileStick>();
		attackSword.gameObject.GetComponent<RaycastTrail>().enabled = true;
		while (counter2 < t && (!stick || !stick.stuck))
		{
			bool flag2 = attackSword.gameObject == null || targ == null;
			if (flag2)
			{
				yield break;
			}
			float num = Mathf.Clamp(Time.deltaTime, 0f, 0.02f);
			counter2 += Time.deltaTime;
			attackSword.move.velocity = Vector3.Lerp(attackSword.move.velocity, (targ.position - attackSword.gameObject.transform.position).normalized * this.throwSpeed * this.throwCurve.Evaluate(counter2), num * 8f);
			attackSword.gameObject.transform.rotation = Quaternion.Lerp(attackSword.gameObject.transform.rotation, quaternion.LookRotation(targ.position - attackSword.gameObject.transform.position, Vector3.up), num * 7f);
			yield return null;
		}
		counter2 = 0f;
		while (counter2 < 0.5f && (!stick || !stick.stuck))
		{
			float num2 = Mathf.Clamp(Time.deltaTime, 0f, 0.02f);
			counter2 += Time.deltaTime;
			bool flag3 = attackSword.gameObject != null;
			if (flag3)
			{
				attackSword.gameObject.transform.rotation = Quaternion.Lerp(attackSword.gameObject.transform.rotation, quaternion.LookRotation(attackSword.move.velocity, Vector3.up), num2 * 15f);
			}
			yield return null;
		}
		yield break;
	}

	// Token: 0x0600001C RID: 28 RVA: 0x0000335C File Offset: 0x0000155C
	private SpookySword CreateNewSword(Vector3 pos, quaternion rot)
	{
		SpookySword spookySword = new SpookySword
		{
			gameObject = Object.Instantiate<GameObject>(this.sourceSword, pos, rot)
		};
		spookySword.move = spookySword.gameObject.GetComponent<MoveTransform>();
		spookySword.gameObject.FetchComponent<TeamHolder>().team = this.data.team;
		spookySword.gameObject.GetComponentInChildren<Landfall.TABC.CodeAnimation>().speedMultiplier = this.attackspeedMulti;
		return spookySword;
	}

	// Token: 0x0400001B RID: 27
	public UnityEvent shootEvent;

	// Token: 0x0400001C RID: 28
	private ShootPosition[] swordPoints;

	// Token: 0x0400001D RID: 29
	private List<SpookySword> swords = new List<SpookySword>();

	// Token: 0x0400001E RID: 30
	public GameObject sourceSword;

	// Token: 0x0400001F RID: 31
	public float throwRange = 10f;

	// Token: 0x04000020 RID: 32
	public float followSpeed = 1f;

	// Token: 0x04000021 RID: 33
	public float throwSpeed = 100f;

	// Token: 0x04000022 RID: 34
	public AnimationCurve throwCurve;

	// Token: 0x04000023 RID: 35
	private float counter;

	// Token: 0x04000024 RID: 36
	private DataHandler data;

	// Token: 0x04000025 RID: 37
	private bool done;

	// Token: 0x04000026 RID: 38
	public float attackRate;

	// Token: 0x04000027 RID: 39
	private float attackspeedMulti = 1f;

	// Token: 0x04000028 RID: 40
	private int attackID;

	// Token: 0x0200002E RID: 46
	// (Invoke) Token: 0x06000109 RID: 265
	public delegate void AttackedEventHandler(Rigidbody target, int useAttackID);
}
