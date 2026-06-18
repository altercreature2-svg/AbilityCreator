using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Bolt;
using TFBGames;
using UnityEngine;

// Token: 0x02000003 RID: 3
public class DodgeMovePhoenix : Move, IRemotelyControllable
{
	// Token: 0x17000001 RID: 1
	// (get) Token: 0x06000008 RID: 8 RVA: 0x00002153 File Offset: 0x00000353
	// (set) Token: 0x06000009 RID: 9 RVA: 0x0000215B File Offset: 0x0000035B
	public bool IsRemotelyControlled { get; private set; }

	// Token: 0x0600000A RID: 10 RVA: 0x00002164 File Offset: 0x00000364
	private void Start()
	{
		this.data = base.transform.root.GetComponentInChildren<DataHandler>();
		this.allRigs = this.data.GetComponent<RigidbodyHolder>();
		for (int i = 0; i < this.moves.Length; i++)
		{
			bool flag = this.moves[i].forceDirection == CombatMoveDataInstance.ForceDirection.RotateTowardsPossCamElseTarget;
			if (flag)
			{
				this.possess = MainCam.instance.GetComponentInParent<CameraAbilityPossess>();
			}
		}
		bool flag2 = this.randomForceMultiplier;
		if (flag2)
		{
			this.randomSeed = (float)Random.Range(-1, 1);
			this.forceMultiplier = Random.Range(this.forceMultiplier * 0.7f, this.forceMultiplier);
			bool flag3 = this.randomSeed < 0f;
			if (flag3)
			{
				this.forceMultiplier = 0f - this.forceMultiplier;
			}
		}
	}

	// Token: 0x0600000B RID: 11 RVA: 0x00002237 File Offset: 0x00000437
	public void DoMove(Transform targetObj)
	{
		this.targetObject = targetObj;
		this.DoMove();
	}

	// Token: 0x0600000C RID: 12 RVA: 0x00002248 File Offset: 0x00000448
	public void DoMove()
	{
		bool flag = !this.data;
		if (flag)
		{
			this.data = base.transform.root.GetComponentInChildren<DataHandler>();
			this.allRigs = this.data.GetComponent<RigidbodyHolder>();
		}
		this.DoMove(null, this.data.targetMainRig, this.data.targetData);
	}

	// Token: 0x0600000D RID: 13 RVA: 0x000022B0 File Offset: 0x000004B0
	public override void DoMove(Rigidbody enemyWeapon, Rigidbody enemyTorso, DataHandler targetData)
	{
		bool flag = this.cancelSelf;
		if (flag)
		{
			base.StopAllCoroutines();
		}
		bool flag2 = !enemyWeapon && !enemyTorso;
		if (!flag2)
		{
			for (int i = 0; i < this.moves.Length; i++)
			{
				CombatMoveDataInstance move = this.moves[i];
				bool flag3 = this.IsAllowedToDoMoveInMultiplayer(move);
				if (flag3)
				{
					base.StartCoroutine(this.DoMoveSequence(move, enemyWeapon, enemyTorso, targetData));
				}
			}
		}
	}

	// Token: 0x0600000E RID: 14 RVA: 0x00002330 File Offset: 0x00000530
	private IEnumerator DoMoveSequence(CombatMoveDataInstance move, Rigidbody enemyWeapon, Rigidbody enemyTorso, DataHandler targetData)
	{
		float t = move.forceCurve.keys[move.forceCurve.keys.Length - 1].time;
		float c = 0f;
		bool flag = move.useAlternateForceProjectMarsClient && BoltNetwork.IsClient;
		if (flag)
		{
			move.force = move.alternateClientForce;
		}
		move.randomMultiplier = move.randomCurve.Evaluate(Random.value);
		List<Rigidbody> rigs = new List<Rigidbody>();
		bool flag2 = move.rigidbodyToMove == CombatMoveDataInstance.RigidBodyToMove.Head;
		if (flag2)
		{
			rigs.Add(this.data.head.GetComponent<Rigidbody>());
		}
		else
		{
			bool flag3 = move.rigidbodyToMove == CombatMoveDataInstance.RigidBodyToMove.Torso;
			if (flag3)
			{
				bool flag4 = this.data.torso;
				if (flag4)
				{
					rigs.Add(this.data.torso.GetComponent<Rigidbody>());
				}
			}
			else
			{
				bool flag5 = move.rigidbodyToMove == CombatMoveDataInstance.RigidBodyToMove.Hip;
				if (flag5)
				{
					bool flag6 = this.data.hip;
					if (flag6)
					{
						rigs.Add(this.data.hip.GetComponent<Rigidbody>());
					}
				}
				else
				{
					bool flag7 = move.rigidbodyToMove == CombatMoveDataInstance.RigidBodyToMove.FootLeft;
					if (flag7)
					{
						bool flag8 = this.data.footLeft;
						if (flag8)
						{
							rigs.Add(this.data.footLeft.GetComponent<Rigidbody>());
						}
					}
					else
					{
						bool flag9 = move.rigidbodyToMove == CombatMoveDataInstance.RigidBodyToMove.FootRight;
						if (flag9)
						{
							bool flag10 = this.data.footRight;
							if (flag10)
							{
								rigs.Add(this.data.footRight.GetComponent<Rigidbody>());
							}
						}
						else
						{
							bool flag11 = move.rigidbodyToMove == CombatMoveDataInstance.RigidBodyToMove.HandRight;
							if (flag11)
							{
								bool flag12 = this.data.rightHand;
								if (flag12)
								{
									rigs.Add(this.data.rightHand.GetComponent<Rigidbody>());
								}
							}
							else
							{
								bool flag13 = move.rigidbodyToMove == CombatMoveDataInstance.RigidBodyToMove.HandLeft;
								if (flag13)
								{
									bool flag14 = this.data.leftHand;
									if (flag14)
									{
										rigs.Add(this.data.leftHand.GetComponent<Rigidbody>());
									}
								}
								else
								{
									bool flag15 = move.rigidbodyToMove == CombatMoveDataInstance.RigidBodyToMove.AllRigs;
									if (flag15)
									{
										rigs.AddRange(this.allRigs.AllRigs);
									}
									else
									{
										bool flag16 = move.rigidbodyToMove == CombatMoveDataInstance.RigidBodyToMove.MainWeapon;
										if (flag16)
										{
											WeaponHandler weaponHandler = this.data.weaponHandler;
											bool flag17 = weaponHandler;
											if (flag17)
											{
												bool flag18 = weaponHandler.rightWeapon && weaponHandler.rightWeapon.rigidbody;
												if (flag18)
												{
													rigs.Add(weaponHandler.rightWeapon.rigidbody);
												}
												else
												{
													bool flag19 = weaponHandler.leftWeapon && weaponHandler.leftWeapon.rigidbody;
													if (flag19)
													{
														rigs.Add(weaponHandler.leftWeapon.rigidbody);
													}
												}
											}
											weaponHandler = null;
										}
										else
										{
											bool flag20 = move.rigidbodyToMove == CombatMoveDataInstance.RigidBodyToMove.This;
											if (flag20)
											{
												rigs.Add(base.GetComponent<Rigidbody>());
											}
											else
											{
												bool flag21 = move.rigidbodyToMove == CombatMoveDataInstance.RigidBodyToMove.Specific && move.specificRig;
												if (flag21)
												{
													rigs.Add(move.specificRig);
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}
		bool includeWeapons = move.includeWeapons;
		if (includeWeapons)
		{
			WeaponHandler weaponHandler2 = this.data.weaponHandler;
			bool flag22 = weaponHandler2;
			if (flag22)
			{
				bool flag23 = weaponHandler2.leftWeapon && weaponHandler2.leftWeapon.rigidbody;
				if (flag23)
				{
					rigs.Add(weaponHandler2.leftWeapon.rigidbody);
				}
				bool flag24 = weaponHandler2.rightWeapon && weaponHandler2.rightWeapon.rigidbody;
				if (flag24)
				{
					rigs.Add(weaponHandler2.rightWeapon.rigidbody);
				}
			}
			weaponHandler2 = null;
		}
		Vector3 forceDirection = Vector3.zero;
		bool flag25 = rigs.Count >= 1;
		if (flag25)
		{
			forceDirection = this.GetDirection(move, enemyWeapon, enemyTorso, rigs[0], targetData);
			int num4;
			for (int num = rigs.Count - 1; num >= 0; num = num4 - 1)
			{
				bool flag26 = rigs[num] == null;
				if (flag26)
				{
					rigs.RemoveAt(num);
				}
				num4 = num;
			}
		}
		float massM = 1f;
		bool flag27 = this.divideForceByMass;
		if (flag27)
		{
			float num2 = 0f;
			int num4;
			for (int i = 0; i < rigs.Count; i = num4 + 1)
			{
				num2 += rigs[i].mass / (float)rigs.Count;
				num4 = i;
			}
			massM = 1f / num2;
		}
		while (c < t)
		{
			bool flag28 = this.CheckConditions();
			if (flag28)
			{
				int num4;
				for (int j = 0; j < rigs.Count; j = num4 + 1)
				{
					bool setDirectionContiniouiouss = move.setDirectionContiniouiouss;
					if (setDirectionContiniouiouss)
					{
						forceDirection = this.GetDirection(move, enemyWeapon, enemyTorso, rigs[0], targetData);
					}
					bool flag29 = !this.usedAsMovement || !this.data || !this.data.GetComponent<AnimationHandler>() || this.data.GetComponent<AnimationHandler>().currentState != 0;
					if (flag29)
					{
						bool flag30 = move.force != 0f && rigs[j];
						if (flag30)
						{
							rigs[j].AddForce(forceDirection * massM * move.randomMultiplier * this.forceMultiplier * move.force * move.forceCurve.Evaluate(c), 5);
						}
						bool flag31 = move.torque != 0f && rigs[j];
						if (flag31)
						{
							rigs[j].AddTorque(forceDirection * massM * this.forceMultiplier * move.torque * move.forceCurve.Evaluate(c), 5);
						}
					}
					num4 = j;
				}
			}
			float num3 = 1f;
			num3 *= this.animationSpeed;
			bool flag32 = move.forceCurve.Evaluate(c) > 0f;
			if (flag32)
			{
				num3 *= this.animationSpeedWhenPositiveCurve;
			}
			c += Time.fixedDeltaTime * num3;
			yield return new WaitForFixedUpdate();
		}
		yield break;
	}

	// Token: 0x0600000F RID: 15 RVA: 0x0000235C File Offset: 0x0000055C
	private bool CheckConditions()
	{
		bool result = true;
		bool flag = this.maxRange != 0f && this.data && this.data.distanceToTarget > this.maxRange;
		if (flag)
		{
			result = false;
		}
		bool flag2 = this.minRange != 0f && this.data && this.data.distanceToTarget < this.minRange;
		if (flag2)
		{
			result = false;
		}
		return result;
	}

	// Token: 0x06000010 RID: 16 RVA: 0x000023E4 File Offset: 0x000005E4
	private Vector3 GetDirection(CombatMoveDataInstance move, Rigidbody enemyWeapon, Rigidbody enemyTorso, Rigidbody ownRig, DataHandler targetData)
	{
		bool flag = !enemyTorso;
		if (flag)
		{
			enemyTorso = this.data.targetData.mainRig;
		}
		Vector3 vector = Vector3.zero;
		bool flag2 = ownRig == null;
		Vector3 result;
		if (flag2)
		{
			result = vector;
		}
		else
		{
			bool flag3 = move.forceDirection == CombatMoveDataInstance.ForceDirection.Up;
			if (flag3)
			{
				vector = Vector3.up;
			}
			bool flag4 = move.forceDirection == CombatMoveDataInstance.ForceDirection.TorwardTarget && ownRig && enemyTorso;
			if (flag4)
			{
				vector = enemyTorso.position - ownRig.position;
				bool normalize = move.normalize;
				if (normalize)
				{
					vector = vector.normalized;
				}
			}
			bool flag5 = move.forceDirection == CombatMoveDataInstance.ForceDirection.TowardsTargetHead && ownRig && targetData && targetData.head;
			if (flag5)
			{
				vector = targetData.head.position + targetData.head.transform.forward * 0.1f + targetData.head.transform.up * 0.15f - ownRig.position;
				bool normalize2 = move.normalize;
				if (normalize2)
				{
					vector = vector.normalized;
				}
			}
			bool flag6 = move.forceDirection == CombatMoveDataInstance.ForceDirection.AwayFromTargetWeapon;
			if (flag6)
			{
				bool flag7 = enemyWeapon;
				if (flag7)
				{
					vector = ownRig.position - (enemyWeapon.worldCenterOfMass + enemyWeapon.velocity * move.predictionAmount);
					bool normalize3 = move.normalize;
					if (normalize3)
					{
						vector = vector.normalized;
					}
					bool ignoreY = move.ignoreY;
					if (ignoreY)
					{
						vector = new Vector3(vector.x, 0f, vector.y);
					}
					bool flag8 = this.data.cantFallForSeconds < 0.5f;
					if (flag8)
					{
						this.data.cantFallForSeconds = 0.5f;
					}
				}
				else
				{
					bool flag9 = enemyTorso;
					if (flag9)
					{
						vector = -(enemyTorso.position - ownRig.position);
						bool normalize4 = move.normalize;
						if (normalize4)
						{
							vector = vector.normalized;
						}
					}
				}
			}
			bool flag10 = move.forceDirection == CombatMoveDataInstance.ForceDirection.CharacterForward;
			if (flag10)
			{
				vector = this.data.characterForwardObject.forward;
			}
			else
			{
				bool flag11 = move.forceDirection == CombatMoveDataInstance.ForceDirection.CharacterRight;
				if (flag11)
				{
					vector = this.data.characterForwardObject.right;
				}
				else
				{
					bool flag12 = move.forceDirection == CombatMoveDataInstance.ForceDirection.CrossUpAndAwayFromAttacker && ownRig && enemyTorso;
					if (flag12)
					{
						vector = Vector3.Cross(Vector3.up, ownRig.position - enemyTorso.position);
						bool normalize5 = move.normalize;
						if (normalize5)
						{
							vector = vector.normalized;
						}
					}
					else
					{
						bool flag13 = move.forceDirection == CombatMoveDataInstance.ForceDirection.CrossUpAndTowardsUnitTarget && ownRig && this.data.targetMainRig;
						if (flag13)
						{
							vector = Vector3.Cross(Vector3.up, ownRig.position - this.data.targetMainRig.position);
							bool normalize6 = move.normalize;
							if (normalize6)
							{
								vector = vector.normalized;
							}
						}
						else
						{
							bool flag14 = move.forceDirection == CombatMoveDataInstance.ForceDirection.RigUp && ownRig;
							if (flag14)
							{
								vector = ownRig.transform.up;
							}
							else
							{
								bool flag15 = move.forceDirection == CombatMoveDataInstance.ForceDirection.RotateTowardsPossCamElseTarget && enemyTorso && ownRig;
								if (flag15)
								{
									vector = ((!this.possess || !this.possess.currentUnit || !this.data || !this.data.unit || !(this.possess.currentUnit == this.data.unit)) ? (-Vector3.Cross(enemyTorso.position - ownRig.position, ownRig.transform.forward).normalized * Vector3.Angle(enemyTorso.position - ownRig.position, ownRig.transform.forward)) : (-Vector3.Cross(MainCam.instance.transform.forward, ownRig.transform.forward).normalized * Vector3.Angle(MainCam.instance.transform.forward, ownRig.transform.forward)));
								}
								else
								{
									bool flag16 = move.forceDirection == CombatMoveDataInstance.ForceDirection.RotateTowardsTarget && enemyTorso && ownRig;
									if (flag16)
									{
										vector = -Vector3.Cross(enemyTorso.position - ownRig.position, ownRig.transform.forward).normalized * Vector3.Angle(enemyTorso.position - ownRig.position, ownRig.transform.forward);
									}
									else
									{
										bool flag17 = move.forceDirection == CombatMoveDataInstance.ForceDirection.RotateTowardsTargetHead && targetData && targetData.head && ownRig;
										if (flag17)
										{
											vector = -Vector3.Cross(targetData.head.position + targetData.head.transform.forward * 0.1f + targetData.head.transform.up * 0.15f - ownRig.position, ownRig.transform.forward).normalized * Vector3.Angle(targetData.head.position + targetData.head.transform.forward * 0.1f + targetData.head.transform.up * 0.15f - ownRig.position, ownRig.transform.forward);
										}
										else
										{
											bool flag18 = move.forceDirection == CombatMoveDataInstance.ForceDirection.AwayFromTargetObject && this.targetObject && ownRig;
											if (flag18)
											{
												vector = ownRig.transform.position - this.targetObject.transform.position;
												bool normalize7 = move.normalize;
												if (normalize7)
												{
													vector = vector.normalized;
												}
											}
											else
											{
												bool flag19 = move.forceDirection == CombatMoveDataInstance.ForceDirection.CrossUpAndAwayFromTargetObject && this.targetObject && ownRig;
												if (flag19)
												{
													vector = Vector3.Cross(Vector3.up, ownRig.position - this.targetObject.position);
													bool normalize8 = move.normalize;
													if (normalize8)
													{
														vector = vector.normalized;
													}
												}
												else
												{
													bool flag20 = move.forceDirection == CombatMoveDataInstance.ForceDirection.InWalkDirection;
													if (flag20)
													{
														vector = this.data.groundedMovementDirectionObject.forward;
													}
													else
													{
														bool flag21 = move.forceDirection == CombatMoveDataInstance.ForceDirection.RotateTowardsWalkDirection;
														if (flag21)
														{
															vector = Vector3.Cross(ownRig.transform.forward, this.data.groundedMovementDirectionObject.forward).normalized * Vector3.Angle(ownRig.transform.forward, this.data.groundedMovementDirectionObject.forward);
														}
														else
														{
															bool flag22 = move.randomizeDirection && Random.value > 0.5f;
															if (flag22)
															{
																vector *= -1f;
															}
															else
															{
																bool flag23 = move.forceDirection == CombatMoveDataInstance.ForceDirection.TowardTargetWithoutY && ownRig && enemyTorso;
																if (flag23)
																{
																	vector = new Vector3(enemyTorso.position.x - ownRig.position.x, 0f, enemyTorso.position.z - ownRig.position.z);
																}
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			result = vector;
		}
		return result;
	}

	// Token: 0x06000011 RID: 17 RVA: 0x00002C39 File Offset: 0x00000E39
	public void SetIsRemotelyControlled(bool isRemotelyControlled)
	{
		this.IsRemotelyControlled = isRemotelyControlled;
	}

	// Token: 0x06000012 RID: 18 RVA: 0x00002C44 File Offset: 0x00000E44
	private bool IsAllowedToDoMoveInMultiplayer(CombatMoveDataInstance move)
	{
		bool flag = !this.allowForMartian && this.IsRemotelyControlled;
		return !flag || move.rigidbodyToMove == CombatMoveDataInstance.RigidBodyToMove.MainWeapon;
	}

	// Token: 0x0400000A RID: 10
	[HideInInspector]
	public Transform targetObject;

	// Token: 0x0400000B RID: 11
	public float animationSpeed = 1f;

	// Token: 0x0400000C RID: 12
	public float animationSpeedWhenPositiveCurve = 1f;

	// Token: 0x0400000D RID: 13
	public float forceMultiplier = 1f;

	// Token: 0x0400000E RID: 14
	public bool divideForceByMass;

	// Token: 0x0400000F RID: 15
	public bool cancelSelf;

	// Token: 0x04000010 RID: 16
	public bool usedAsMovement;

	// Token: 0x04000011 RID: 17
	public bool randomForceMultiplier;

	// Token: 0x04000012 RID: 18
	public float minRange;

	// Token: 0x04000013 RID: 19
	public float maxRange;

	// Token: 0x04000014 RID: 20
	public CombatMoveDataInstance[] moves;

	// Token: 0x04000015 RID: 21
	[SerializeField]
	[Tooltip("See the comments in code.")]
	protected bool allowForMartian;

	// Token: 0x04000016 RID: 22
	private float randomSeed;

	// Token: 0x04000017 RID: 23
	private DataHandler data;

	// Token: 0x04000018 RID: 24
	private RigidbodyHolder allRigs;

	// Token: 0x04000019 RID: 25
	private CameraAbilityPossess possess;
}
