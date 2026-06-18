using System;
using Landfall.TABS.GameState;
using Photon.Bolt;
using UnityEngine;

// Token: 0x02000007 RID: 7
public class PhoenixWings : MonoBehaviour
{
	// Token: 0x0600002D RID: 45 RVA: 0x00003BE0 File Offset: 0x00001DE0
	private void Start()
	{
		this.data = base.transform.root.GetComponentInChildren<DataHandler>();
		this.rigHolder = this.data.GetComponent<RigidbodyHolder>();
		this.data.takeFallDamage = false;
		this.data.canFall = false;
		bool flag = this.data.footRight;
		if (flag)
		{
			this.rightFootRig = this.data.footRight.GetComponent<Rigidbody>();
		}
		bool flag2 = this.data.footLeft;
		if (flag2)
		{
			this.leftFootRig = this.data.footLeft.GetComponent<Rigidbody>();
		}
		this.hipRig = this.data.hip.GetComponent<Rigidbody>();
		bool flag3 = this.data.head;
		if (flag3)
		{
			this.headRig = this.data.head.GetComponent<Rigidbody>();
		}
		AnimationHandler component = this.data.GetComponent<AnimationHandler>();
		bool flag4 = component;
		if (flag4)
		{
			component.multiplier = 0.5f;
		}
		this.heightVariance *= Random.value;
		this.time = Random.Range(0f, 1000f);
		Balance component2 = this.data.GetComponent<Balance>();
		bool flag5 = component2;
		if (flag5)
		{
			component2.enabled = false;
		}
		this.m_gameStateManager = ServiceLocator.GetService<GameStateManager>();
		bool flag6 = this.setUnitMainRigKinematic && BoltNetwork.IsClient;
		if (flag6)
		{
			this.data.mainRig.isKinematic = true;
		}
	}

	// Token: 0x0600002E RID: 46 RVA: 0x00003D6C File Offset: 0x00001F6C
	private void FixedUpdate()
	{
		bool flag = (!this.useWingsInPlacement && this.m_gameStateManager.GameState != GameState.BattleState) || !this.useWings;
		if (!flag)
		{
			bool flag2 = this.data.unit.m_PreferedDistance > this.data.distanceToTarget;
			RaycastHit raycastHit;
			Physics.Raycast(new Ray(base.transform.position, Vector3.down), ref raycastHit, this.flightCurve.keys[this.flightCurve.keys.Length - 1].time, this.mask);
			bool flag3 = raycastHit.transform;
			if (flag3)
			{
				float num = raycastHit.distance + Mathf.Cos((Time.time + this.time) * this.variationSpeed) * this.heightVariance;
				this.data.mainRig.AddTorque(this.rotationTorque * Vector3.Angle(this.data.mainRig.transform.up, this.data.groundedMovementDirectionObject.forward) * Vector3.Cross(this.data.mainRig.transform.up, this.data.groundedMovementDirectionObject.forward), 5);
				bool flag4 = this.headRig;
				if (flag4)
				{
					this.headRig.AddForce(Vector3.up * this.flightForce * this.headM * this.flightCurve.Evaluate(num), 5);
				}
				this.data.mainRig.AddForce(Vector3.up * this.flightForce * this.flightCurve.Evaluate(num), 5);
				bool flag5 = this.rightFootRig;
				if (flag5)
				{
					this.rightFootRig.AddForce(Vector3.up * this.flightForce * this.legForceMultiplier * 0.5f * this.flightCurve.Evaluate(num), 5);
				}
				bool flag6 = this.rightFootRig;
				if (flag6)
				{
					this.leftFootRig.AddForce(Vector3.up * this.flightForce * this.legForceMultiplier * 0.5f * this.flightCurve.Evaluate(num), 5);
				}
				this.data.TouchGround(raycastHit.point, raycastHit.normal, null);
			}
		}
	}

	// Token: 0x0600002F RID: 47 RVA: 0x0000400A File Offset: 0x0000220A
	public void EnableFlight()
	{
		this.useWings = true;
	}

	// Token: 0x06000030 RID: 48 RVA: 0x00004014 File Offset: 0x00002214
	public void DiableFlight()
	{
		this.useWings = false;
	}

	// Token: 0x0400003D RID: 61
	public LayerMask mask;

	// Token: 0x0400003E RID: 62
	public AnimationCurve flightCurve;

	// Token: 0x0400003F RID: 63
	public float heightVariance = 0.5f;

	// Token: 0x04000040 RID: 64
	public float variationSpeed = 0.5f;

	// Token: 0x04000041 RID: 65
	public float flightForce;

	// Token: 0x04000042 RID: 66
	public float legForceMultiplier = 1f;

	// Token: 0x04000043 RID: 67
	private DataHandler data;

	// Token: 0x04000044 RID: 68
	private RigidbodyHolder rigHolder;

	// Token: 0x04000045 RID: 69
	private Rigidbody rightFootRig;

	// Token: 0x04000046 RID: 70
	private Rigidbody leftFootRig;

	// Token: 0x04000047 RID: 71
	private Rigidbody hipRig;

	// Token: 0x04000048 RID: 72
	private Rigidbody headRig;

	// Token: 0x04000049 RID: 73
	public float headM = 0.5f;

	// Token: 0x0400004A RID: 74
	private float time;

	// Token: 0x0400004B RID: 75
	public bool useWings = true;

	// Token: 0x0400004C RID: 76
	public bool useWingsInPlacement = true;

	// Token: 0x0400004D RID: 77
	[Tooltip("Enable if units move erratically on the client side of ProjectMars games. Only enable if you are sure Wings.cs is causing erratic movement.")]
	public bool setUnitMainRigKinematic;

	// Token: 0x0400004E RID: 78
	private GameStateManager m_gameStateManager;

	// Token: 0x0400004F RID: 79
	public float rotationTorque = 10f;
}
