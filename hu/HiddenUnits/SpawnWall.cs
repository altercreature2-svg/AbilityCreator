using System;
using System.Collections;
using System.Collections.Generic;
using Landfall.TABS;
using Photon.Bolt;
using TFBGames;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000008 RID: 8
public class SpawnWall : MonoBehaviour, IRemotelyControllable
{
	// Token: 0x17000002 RID: 2
	// (get) Token: 0x06000032 RID: 50 RVA: 0x00004079 File Offset: 0x00002279
	// (set) Token: 0x06000033 RID: 51 RVA: 0x00004081 File Offset: 0x00002281
	public bool IsRemotelyControlled { get; private set; }

	// Token: 0x06000034 RID: 52 RVA: 0x0000408C File Offset: 0x0000228C
	private void Start()
	{
		this.data = base.transform.root.GetComponentInChildren<DataHandler>();
		this.rootObject = base.transform.root.gameObject;
		bool flag = this.giveTarget;
		if (flag)
		{
			this.TargetChecker = base.GetComponent<CheckClosestUnitTargets>();
		}
		bool flag2 = this.spawnOnAwake;
		if (flag2)
		{
			base.StartCoroutine(this.SpawnUnit(base.transform.position, base.transform.rotation));
		}
	}

	// Token: 0x06000035 RID: 53 RVA: 0x00004110 File Offset: 0x00002310
	public void SpawnUpwardsOnLastGroundPos()
	{
		bool flag = this.data;
		if (flag)
		{
			base.StartCoroutine(this.SpawnUnit(this.data.groundMapPosition, Quaternion.LookRotation(Vector3.up)));
		}
		else
		{
			this.SpawnUpwards();
		}
	}

	// Token: 0x06000036 RID: 54 RVA: 0x0000415C File Offset: 0x0000235C
	public void SpawnUpwardsOnTarget()
	{
		Vector3 position = base.transform.position;
		bool flag = !this.data;
		if (flag)
		{
			this.data = this.rootObject.GetComponent<Unit>().data;
		}
		bool flag2 = this.data && this.data.targetData;
		if (flag2)
		{
			position = this.data.targetData.mainRig.position;
			bool flag3 = this.maxRange != 0f && this.data.distanceToTarget > this.maxRange;
			if (flag3)
			{
				return;
			}
		}
		base.StartCoroutine(this.SpawnUnit(position, Quaternion.LookRotation(Vector3.up + Random.insideUnitSphere * this.spread * 0.01f)));
	}

	// Token: 0x06000037 RID: 55 RVA: 0x00004240 File Offset: 0x00002440
	public void SpawnOnAndTowardsTarget()
	{
		Vector3 position = base.transform.position;
		bool flag = !this.data;
		if (flag)
		{
			this.data = this.rootObject.GetComponent<Unit>().data;
		}
		bool flag2 = this.data && this.data.targetData;
		if (flag2)
		{
			position = this.data.targetData.mainRig.position;
			bool flag3 = this.maxRange != 0f && this.data.distanceToTarget > this.maxRange;
			if (flag3)
			{
				return;
			}
		}
		Vector3 a = new Vector3(this.data.targetData.mainRig.position.x - this.data.mainRig.position.x, 0f, this.data.targetData.mainRig.position.z - this.data.mainRig.position.z);
		base.StartCoroutine(this.SpawnUnit(position, Quaternion.LookRotation(a + Random.insideUnitSphere * this.spread * 0.01f)));
	}

	// Token: 0x06000038 RID: 56 RVA: 0x00004390 File Offset: 0x00002590
	public void SpawnUpwardsOnSpawnerTarget()
	{
		Vector3 position = base.transform.position;
		DataHandler dataHandler = null;
		TeamHolder component = base.transform.root.GetComponent<TeamHolder>();
		bool flag = component.spawner;
		if (flag)
		{
			dataHandler = component.spawner.transform.root.GetComponent<Unit>().data;
		}
		bool flag2 = dataHandler && dataHandler.targetData;
		if (flag2)
		{
			position = dataHandler.targetData.mainRig.position;
			bool flag3 = this.maxRange != 0f && dataHandler.distanceToTarget > this.maxRange;
			if (flag3)
			{
				return;
			}
		}
		base.StartCoroutine(this.SpawnUnit(position, Quaternion.LookRotation(Vector3.up + Random.insideUnitSphere * this.spread * 0.01f)));
	}

	// Token: 0x06000039 RID: 57 RVA: 0x00004478 File Offset: 0x00002678
	public void SpawnUpwards()
	{
		base.StartCoroutine(this.SpawnUnit(base.transform.position, Quaternion.LookRotation(Vector3.up + Random.insideUnitSphere * this.spread * 0.01f)));
	}

	// Token: 0x0600003A RID: 58 RVA: 0x000044C8 File Offset: 0x000026C8
	public void SpawnAtObjectRotation()
	{
		Vector3 a = base.transform.rotation * Vector3.forward;
		base.StartCoroutine(this.SpawnUnit(base.transform.position, Quaternion.LookRotation(a + Random.insideUnitSphere * this.spread * 0.01f)));
	}

	// Token: 0x0600003B RID: 59 RVA: 0x0000452C File Offset: 0x0000272C
	public void SpawnTowardsTarget()
	{
		bool flag = !this.data;
		if (flag)
		{
			this.data = this.rootObject.GetComponent<Unit>().data;
		}
		bool flag2 = this.data.targetData;
		if (flag2)
		{
			base.StartCoroutine(this.SpawnUnit(base.transform.position, Quaternion.LookRotation((this.data.targetData.mainRig.position - this.data.mainRig.position).normalized + Random.insideUnitSphere * this.spread * 0.01f)));
		}
	}

	// Token: 0x0600003C RID: 60 RVA: 0x000045E8 File Offset: 0x000027E8
	public void SpawnTowardsTargetWithoutY()
	{
		bool flag = !this.data;
		if (flag)
		{
			this.data = this.rootObject.GetComponent<Unit>().data;
		}
		bool flag2 = this.data.targetData;
		if (flag2)
		{
			Quaternion rotation = Quaternion.LookRotation(new Vector3(this.data.targetData.mainRig.position.x - this.data.mainRig.position.x, 0f, this.data.targetData.mainRig.position.z - this.data.mainRig.position.z).normalized + Random.insideUnitSphere * this.spread * 0.01f);
			base.StartCoroutine(this.SpawnUnit(Vector3.Lerp(this.data.targetData.mainRig.position, base.transform.position, this.percentage), rotation));
		}
	}

	// Token: 0x0600003D RID: 61 RVA: 0x0000470C File Offset: 0x0000290C
	public void SpawnCharacterForward()
	{
		base.StartCoroutine(this.SpawnUnit(base.transform.position, Quaternion.LookRotation(base.transform.root.GetComponent<Unit>().data.mainRig.transform.forward + Random.insideUnitSphere * this.spread * 0.01f)));
	}

	// Token: 0x0600003E RID: 62 RVA: 0x0000477A File Offset: 0x0000297A
	public void Spawn(Vector3 position, Vector3 direction)
	{
		base.StartCoroutine(this.SpawnUnit(position, Quaternion.LookRotation(direction + Random.insideUnitSphere * this.spread * 0.01f)));
	}

	// Token: 0x0600003F RID: 63 RVA: 0x000047B0 File Offset: 0x000029B0
	private IEnumerator SpawnUnit(Vector3 position, Quaternion rotation)
	{
		bool flag = this.TargetChecker;
		if (flag)
		{
			this.targets = new List<Unit>();
			this.targets = this.TargetChecker.GetTargets(new float?(this.maxRange));
		}
		bool flag2 = this.waitBeforeFirstSpawn;
		if (flag2)
		{
			this.spawnedSinceDelay = this.allowedToSpawn;
		}
		int numberSpawned = 0;
		while (numberSpawned < this.numberToSpawn)
		{
			this.InitializeSpawn();
			bool flag3 = !this.IsAllowedToSpawnInMultiplayer();
			if (flag3)
			{
				yield return null;
			}
			bool flag4 = this.data && this.data.Dead && !this.spawnIfUnitDead;
			if (flag4)
			{
				yield return null;
			}
			bool flag5 = this.useAlternatingSpawnPos;
			if (flag5)
			{
				this.altSpawnPos = new List<AlternatingSpawnPos>();
				this.altSpawnPos.AddRange(base.transform.parent.GetComponentsInChildren<AlternatingSpawnPos>());
			}
			bool flag6 = this.spawnedSinceDelay < this.allowedToSpawn;
			if (flag6)
			{
				bool flag7 = this.useAlternatingSpawnPos && this.altSpawnPos.Count > 0;
				if (flag7)
				{
					bool flag8 = this.currentSpawnPosNumber > this.altSpawnPos.Count - 1;
					if (flag8)
					{
						this.currentSpawnPosNumber = 0;
					}
					position = this.altSpawnPos[this.currentSpawnPosNumber].transform.position;
					bool flag9 = this.numberToSpawn - numberSpawned <= this.altSpawnPos.Count && this.altSpawnPos[this.currentSpawnPosNumber].useLastSpawnEvent;
					if (flag9)
					{
						this.altSpawnPos[this.currentSpawnPosNumber].InvokeLastSpawnEvent();
					}
					else
					{
						this.altSpawnPos[this.currentSpawnPosNumber].InvokeSpawnEvent();
					}
				}
				GameObject gameObject = (!this.isProjectile || !(this.projectilesSpawnManager != null)) ? Object.Instantiate<GameObject>(this.objectToSpawn, position, rotation) : this.projectilesSpawnManager.SpawnProjectile(this.objectToSpawn, position, rotation);
				TeamHolder.AddTeamHolder(gameObject, base.transform.gameObject);
				TeamHolder component = gameObject.GetComponent<TeamHolder>();
				bool flag10 = this.giveSpawnerWeapon;
				if (flag10)
				{
					component.spawnerWeapon = base.transform.GetComponentInParent<Weapon>().gameObject;
				}
				this.spawnedSinceDelay += 1f;
				bool flag11 = this.parentToMe;
				if (flag11)
				{
					gameObject.transform.SetParent(base.transform, true);
				}
				bool flag12 = this.followMe;
				if (flag12)
				{
					FollowTransform followTransform = gameObject.gameObject.AddComponent<FollowTransform>();
					followTransform.target = base.transform;
					followTransform.destroyOnTargetNull = false;
					followTransform = null;
				}
				bool flag13 = this.useRootSizeOfSpawner;
				if (flag13)
				{
					gameObject.transform.localScale = base.transform.root.localScale;
				}
				SpellTarget component2 = gameObject.GetComponent<SpellTarget>();
				bool flag14 = this.TargetChecker && this.targets.Count > 0;
				if (flag14)
				{
					bool flag15 = numberSpawned < this.targets.Count;
					if (flag15)
					{
						this.targetPlace = numberSpawned;
					}
					else
					{
						this.targetPlace = numberSpawned - this.targets.Count * Mathf.FloorToInt((float)(numberSpawned / this.targets.Count));
					}
				}
				bool flag16 = component2;
				if (flag16)
				{
					bool flag17 = this.TargetChecker && this.giveTarget && this.targetPlace < this.targets.Count;
					if (flag17)
					{
						this.currentTarget = this.targets[this.targetPlace];
						DataHandler componentInChildren = this.currentTarget.GetComponentInChildren<DataHandler>();
						Vector3 position2 = base.transform.position;
						Vector3 position3 = componentInChildren.mainRig.position;
						Rigidbody mainRig = componentInChildren.mainRig;
						component2.DoEffect(position2, position3, mainRig);
						componentInChildren = null;
						position2 = default(Vector3);
						position3 = default(Vector3);
						mainRig = null;
					}
					else
					{
						component2.GetTarget();
					}
				}
				TeslaCannon component3 = gameObject.GetComponent<TeslaCannon>();
				bool flag18 = component3 && this.spawnerProjecile;
				if (flag18)
				{
					component3.maxTargetChecker = this.spawnerProjecile.transform.GetComponent<TeslaCannon>().maxTargetChecker;
					bool flag19 = this.TargetChecker && this.giveTarget && component3.maxTargetChecker && component3.maxTargetChecker.CheckIfAllowedToHit();
					if (flag19)
					{
						Unit component4 = base.transform.root.GetComponent<Unit>();
						bool flag20 = this.targets.Count > 0;
						if (flag20)
						{
							bool flag21 = !this.allowRootTarget && component4 && this.targets[this.targetPlace] == component4;
							if (flag21)
							{
								this.targets.Remove(this.targets[this.targetPlace]);
							}
							this.currentTarget = this.targets[this.targetPlace];
							DataHandler componentInChildren2 = this.currentTarget.GetComponentInChildren<DataHandler>();
							TeamHolder component5 = base.GetComponent<TeamHolder>();
							component.team = component5.team;
							component.spawner = component5.spawner;
							component3.PlayEffect(componentInChildren2.mainRig.transform, base.transform, component5.spawner);
							componentInChildren2 = null;
							component5 = null;
						}
						else
						{
							Object.Destroy(base.gameObject);
						}
						component4 = null;
					}
					else
					{
						Object.Destroy(base.gameObject);
					}
				}
				int num = numberSpawned;
				numberSpawned = num + 1;
				this.currentSpawnPosNumber++;
				UnityEvent unityEvent = this.spawnEvent;
				if (unityEvent != null)
				{
					unityEvent.Invoke();
				}
				gameObject = null;
				component = null;
				component2 = null;
				component3 = null;
			}
			else
			{
				this.spawnedSinceDelay = 0f;
				float seconds = (!this.useRandom) ? this.timeBetweenSpawns : Random.Range(this.timeBetweenSpawns * this.minRandom, this.timeBetweenSpawns * this.maxRandom);
				yield return new WaitForSeconds(seconds);
			}
		}
		this.targetPlace = 0;
		yield break;
	}

	// Token: 0x06000040 RID: 64 RVA: 0x000047D0 File Offset: 0x000029D0
	private void InitializeSpawn()
	{
		bool flag = !this.didInitialize;
		if (flag)
		{
			this.didInitialize = true;
			bool flag2 = this.objectToSpawn != null && this.objectToSpawn.GetComponent<Projectile>() != null;
			if (flag2)
			{
				this.isProjectile = true;
				this.projectilesSpawnManager = ServiceLocator.GetService<ProjectilesSpawnManager>();
			}
		}
	}

	// Token: 0x06000041 RID: 65 RVA: 0x0000482E File Offset: 0x00002A2E
	public void SetIsRemotelyControlled(bool isRemotelyControlled)
	{
		this.IsRemotelyControlled = isRemotelyControlled;
	}

	// Token: 0x06000042 RID: 66 RVA: 0x0000483C File Offset: 0x00002A3C
	private bool IsAllowedToSpawnInMultiplayer()
	{
		this.InitializeForMultiplayer();
		bool flag = !this.isUnit && !this.isProjectile;
		return flag || !this.IsRemotelyControlled;
	}

	// Token: 0x06000043 RID: 67 RVA: 0x0000487C File Offset: 0x00002A7C
	private void InitializeForMultiplayer()
	{
		bool flag = !this.didInitializeForMultiplayer;
		if (flag)
		{
			this.didInitializeForMultiplayer = true;
			bool flag2 = BoltNetwork.IsRunning && this.objectToSpawn != null;
			if (flag2)
			{
				this.isUnit = (this.objectToSpawn.GetComponent<Unit>() != null);
			}
		}
	}

	// Token: 0x04000050 RID: 80
	public GameObject objectToSpawn;

	// Token: 0x04000051 RID: 81
	public float percentage = 0.5f;

	// Token: 0x04000052 RID: 82
	public bool spawnOnAwake;

	// Token: 0x04000053 RID: 83
	public int numberToSpawn = 1;

	// Token: 0x04000054 RID: 84
	public bool spawnIfUnitDead = true;

	// Token: 0x04000055 RID: 85
	public bool parentToMe;

	// Token: 0x04000056 RID: 86
	public bool followMe;

	// Token: 0x04000057 RID: 87
	public bool useRootSizeOfSpawner;

	// Token: 0x04000058 RID: 88
	public bool giveTarget;

	// Token: 0x04000059 RID: 89
	public bool allowRootTarget;

	// Token: 0x0400005A RID: 90
	public bool useAlternatingSpawnPos;

	// Token: 0x0400005B RID: 91
	public bool giveSpawnerWeapon;

	// Token: 0x0400005C RID: 92
	public float spread;

	// Token: 0x0400005D RID: 93
	public float timeBetweenSpawns = 0.02f;

	// Token: 0x0400005E RID: 94
	public bool waitBeforeFirstSpawn;

	// Token: 0x0400005F RID: 95
	public bool useRandom;

	// Token: 0x04000060 RID: 96
	public float minRandom = 1f;

	// Token: 0x04000061 RID: 97
	public float maxRandom = 1f;

	// Token: 0x04000062 RID: 98
	public float maxRange;

	// Token: 0x04000063 RID: 99
	private bool didInitialize;

	// Token: 0x04000064 RID: 100
	public UnityEvent spawnEvent;

	// Token: 0x04000065 RID: 101
	private ProjectilesSpawnManager projectilesSpawnManager;

	// Token: 0x04000066 RID: 102
	private bool isProjectile;

	// Token: 0x04000067 RID: 103
	private GameObject rootObject;

	// Token: 0x04000068 RID: 104
	private DataHandler data;

	// Token: 0x04000069 RID: 105
	private CheckClosestUnitTargets TargetChecker;

	// Token: 0x0400006A RID: 106
	private float allowedToSpawn = 1f;

	// Token: 0x0400006B RID: 107
	private float spawnedSinceDelay;

	// Token: 0x0400006C RID: 108
	private List<Unit> targets;

	// Token: 0x0400006D RID: 109
	private Unit currentTarget;

	// Token: 0x0400006E RID: 110
	private List<AlternatingSpawnPos> altSpawnPos;

	// Token: 0x0400006F RID: 111
	private int targetPlace;

	// Token: 0x04000070 RID: 112
	private int currentSpawnPosNumber;

	// Token: 0x04000071 RID: 113
	[HideInInspector]
	public GameObject spawnerProjecile;

	// Token: 0x04000072 RID: 114
	private bool didInitializeForMultiplayer;

	// Token: 0x04000073 RID: 115
	private bool isUnit;
}
