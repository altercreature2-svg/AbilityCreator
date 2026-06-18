using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Landfall.TABS;
using UnityEngine;

// Token: 0x02000005 RID: 5
public class HadezHands : MonoBehaviour
{
	// Token: 0x0600001E RID: 30 RVA: 0x00003410 File Offset: 0x00001610
	public void Start()
	{
		for (int i = 0; i < base.transform.childCount; i++)
		{
			AttackArm attackArm = new AttackArm();
			attackArm.targetObj = base.transform.GetChild(i).Find("Target").gameObject;
			attackArm.restPosObj = base.transform.GetChild(i).Find("RestPos").gameObject;
			attackArm.lerpSpeed = Random.Range(0.5f, 1.5f);
			attackArm.smoothTargetPos = base.transform.position;
			attackArm.targetPos = base.transform.position;
			this.attackArms.Add(attackArm);
		}
		this.unit = base.GetComponentInParent<Weapon>().connectedData.unit;
		this.targetTrans = base.transform.parent;
		base.transform.SetParent(this.unit.transform);
	}

	// Token: 0x0600001F RID: 31 RVA: 0x0000350C File Offset: 0x0000170C
	private void Update()
	{
		bool flag = this.targetTrans == null || this.unit.data.Dead;
		if (flag)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			base.transform.position = this.targetTrans.position;
			base.transform.rotation = Quaternion.Lerp(base.transform.rotation, this.targetTrans.rotation, Time.deltaTime * 2.5f);
			for (int i = 0; i < this.attackArms.Count; i++)
			{
				this.attackArms[i].counter += Time.deltaTime;
				Vector3 vector = Vector3.Lerp(this.attackArms[i].smoothTargetPos, this.attackArms[i].targetPos, Time.deltaTime * 10f);
				bool flag2 = this.attackArms[i].armState == AttackArm.ArmState.Free;
				if (flag2)
				{
					this.attackArms[i].targetPos = Vector3.Lerp(this.attackArms[i].targetPos, this.attackArms[i].restPosObj.transform.position + this.GetPerlinPos(this.attackArms[i].lerpSpeed) * 2f, Time.deltaTime * 3f * this.attackArms[i].lerpSpeed);
					this.attackArms[i].smoothTargetPos = vector;
					this.snapSpeed = 0f;
				}
				else
				{
					this.snapSpeed += Time.deltaTime;
					this.attackArms[i].smoothTargetPos = Vector3.Lerp(vector, this.attackArms[i].targetPos, this.snapSpeed);
				}
				bool flag3 = this.attackArms[i].heldUnit;
				if (flag3)
				{
					this.attackArms[i].targetObj.transform.position = Vector3.Lerp(this.attackArms[i].smoothTargetPos, this.attackArms[i].heldUnit.data.mainRig.position, this.followMainRigAmount);
				}
				else
				{
					this.attackArms[i].targetObj.transform.position = this.attackArms[i].smoothTargetPos;
				}
			}
			this.CheckAttack(this.attackArms[Random.Range(0, this.attackArms.Count)]);
		}
	}

	// Token: 0x06000020 RID: 32 RVA: 0x000037DC File Offset: 0x000019DC
	private void CheckAttack(AttackArm attack)
	{
		Unit unit = this.SetTarget();
		bool flag = attack.counter > 3f && this.unit.data.distanceToTarget <= 10f && attack.armState == AttackArm.ArmState.Free && unit != null;
		if (flag)
		{
			this.hitList.Add(unit);
			attack.counter = 0f;
			base.StartCoroutine(this.Attack(attack, unit));
		}
	}

	// Token: 0x06000021 RID: 33 RVA: 0x00003854 File Offset: 0x00001A54
	private IEnumerator Attack(AttackArm attack, Unit targetUnit)
	{
		attack.armState = AttackArm.ArmState.Holding;
		attack.heldUnit = targetUnit;
		float c2 = 0f;
		bool flag = this.swingRef != "";
		if (flag)
		{
			ServiceLocator.GetService<SoundPlayer>().PlaySoundEffect(this.swingRef, 1f, base.transform.position, SoundEffectVariations.MaterialType.Default, null, 1f);
		}
		while (c2 < 3f && targetUnit && targetUnit.data.mainRig && !targetUnit.data.Dead)
		{
			Vector3 a = targetUnit.data.mainRig.position - attack.restPosObj.transform.position;
			attack.targetPos = attack.restPosObj.transform.position + a * this.reachCurve.Evaluate(c2);
			targetUnit.data.healthHandler.TakeDamage(100f * Time.deltaTime, Vector3.up, null, DamageType.Default);
			c2 += Time.deltaTime * Time.timeScale;
			yield return null;
			a = default(Vector3);
		}
		attack.armState = AttackArm.ArmState.Free;
		c2 = 0f;
		float t2 = AnimationCurveFunctions.GetAnimLength(this.goBackToHoldCurve);
		while (c2 < t2 && targetUnit && targetUnit.data.mainRig)
		{
			Vector3 a2 = targetUnit.data.mainRig.position - attack.restPosObj.transform.position;
			attack.targetPos = attack.restPosObj.transform.position + a2 * this.goBackToHoldCurve.Evaluate(c2);
			c2 += Time.deltaTime;
			this.followMainRigAmount = this.goBackToHoldCurveFollowMainRigAmount.Evaluate(c2);
			yield return null;
			a2 = default(Vector3);
		}
		this.hitList.Remove(attack.heldUnit);
		attack.heldUnit = null;
		yield break;
	}

	// Token: 0x06000022 RID: 34 RVA: 0x00003874 File Offset: 0x00001A74
	private Vector3 GetPerlinPos(float input)
	{
		input *= 0.2f;
		Vector3 result = new Vector3(0f, 0f, 0f);
		result.x += Mathf.PerlinNoise(Time.time * input, 0f);
		result.y += Mathf.PerlinNoise(Time.time * input, Time.time * input);
		result.z += Mathf.PerlinNoise(0f, Time.time * input);
		result.x -= 0.5f;
		result.y -= 0.5f;
		result.z -= 0.5f;
		return result;
	}

	// Token: 0x06000023 RID: 35 RVA: 0x0000392C File Offset: 0x00001B2C
	public Unit SetTarget()
	{
		Unit[] array = (from Unit unit in Object.FindObjectsOfType<Unit>()
		where !unit.data.Dead && unit.Team != base.transform.root.GetComponent<Unit>().Team && !this.hitList.Contains(unit) && (unit.data.mainRig.transform.position - base.transform.position).magnitude <= 10f
		orderby (unit.data.mainRig.transform.position - base.transform.position).magnitude
		select unit).ToArray<Unit>();
		bool flag = array.Length != 0;
		Unit result;
		if (flag)
		{
			result = array[0];
		}
		else
		{
			result = null;
		}
		return result;
	}

	// Token: 0x0400002A RID: 42
	public string swingRef;

	// Token: 0x0400002B RID: 43
	public string hitRef;

	// Token: 0x0400002C RID: 44
	private Transform targetTrans;

	// Token: 0x0400002D RID: 45
	private List<AttackArm> attackArms = new List<AttackArm>();

	// Token: 0x0400002E RID: 46
	public AnimationCurve reachCurve;

	// Token: 0x0400002F RID: 47
	public AnimationCurve goBackToHoldCurve;

	// Token: 0x04000030 RID: 48
	public AnimationCurve goBackToHoldCurveFollowMainRigAmount;

	// Token: 0x04000031 RID: 49
	public AnimationCurve throwCurve;

	// Token: 0x04000032 RID: 50
	private float followMainRigAmount;

	// Token: 0x04000033 RID: 51
	private Unit unit;

	// Token: 0x04000034 RID: 52
	private float snapSpeed;

	// Token: 0x04000035 RID: 53
	private List<Unit> hitList = new List<Unit>();
}
