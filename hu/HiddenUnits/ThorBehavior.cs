using System;
using System.Collections.Generic;
using Landfall.TABS;
using TGCore.Library;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000009 RID: 9
public class ThorBehavior : MonoBehaviour
{
	// Token: 0x06000045 RID: 69 RVA: 0x0000492D File Offset: 0x00002B2D
	private void Start()
	{
		this.unit = base.transform.root.GetComponent<Unit>();
	}

	// Token: 0x06000046 RID: 70 RVA: 0x00004948 File Offset: 0x00002B48
	public void GrabHammerRight()
	{
		bool flag = !this.holdingRight;
		if (flag)
		{
			this.holdingRight = true;
			this.grabEvent.Invoke();
			foreach (Collider collider in this.unit.GetComponentInChildren<HandRight>().GetComponentsInChildren<Collider>())
			{
				collider.enabled = false;
			}
			bool flag2 = this.unit.holdingHandler;
			if (flag2)
			{
				this.unit.WeaponHandler.fistRefernce = null;
				bool flag3 = this.unit.holdingHandler.rightObject;
				if (flag3)
				{
					GameObject gameObject = this.unit.holdingHandler.rightObject.gameObject;
					this.unit.holdingHandler.LetGoOfWeapon(gameObject);
					Object.Destroy(gameObject);
				}
				this.SetWeapon(this.unit, this.unit.Team, this.weaponToGrab, new PropItemData(), HoldingHandler.HandType.Right, this.unit.data.mainRig.rotation, new List<GameObject>(), false);
			}
			else
			{
				bool flag4 = this.unit.GetComponentInChildren<HoldingHandlerMulti>();
				if (flag4)
				{
					HoldingHandlerMulti componentInChildren = this.unit.GetComponentInChildren<HoldingHandlerMulti>();
					foreach (HandRight handRight in componentInChildren.mainHands)
					{
						componentInChildren.SetWeapon(handRight.gameObject, this.weaponToGrab);
					}
				}
			}
		}
	}

	// Token: 0x06000047 RID: 71 RVA: 0x00004AE8 File Offset: 0x00002CE8
	public void GrabHammerLeft()
	{
		bool flag = !this.holdingLeft;
		if (flag)
		{
			this.holdingLeft = true;
			foreach (Collider collider in this.unit.GetComponentInChildren<HandLeft>().GetComponentsInChildren<Collider>())
			{
				collider.enabled = false;
			}
			bool flag2 = this.unit.holdingHandler;
			if (flag2)
			{
				this.weaponToGrab.GetComponent<Holdable>().holdableData.snapConnect = false;
				bool flag3 = this.unit.holdingHandler.leftObject;
				if (flag3)
				{
					GameObject gameObject = this.unit.holdingHandler.leftObject.gameObject;
					this.unit.holdingHandler.LetGoOfWeapon(gameObject);
					Object.Destroy(gameObject);
				}
				this.unit.holdingHandler.leftHandActivity = HoldingHandler.HandActivity.HoldingRightObject;
			}
			else
			{
				bool flag4 = this.unit.GetComponentInChildren<HoldingHandlerMulti>();
				if (flag4)
				{
					HoldingHandlerMulti componentInChildren = this.unit.GetComponentInChildren<HoldingHandlerMulti>();
					foreach (HandLeft handLeft in componentInChildren.otherHands)
					{
						componentInChildren.SetWeapon(handLeft.gameObject, this.weaponToGrab);
					}
				}
			}
		}
	}

	// Token: 0x06000048 RID: 72 RVA: 0x00004C50 File Offset: 0x00002E50
	public void UnGrabHammer()
	{
		bool flag = this.holdingLeft || this.holdingRight;
		if (flag)
		{
			this.holdingLeft = false;
			this.holdingRight = false;
			this.ungrabEvent.Invoke();
			bool flag2 = this.unit.holdingHandler;
			if (flag2)
			{
				this.unit.holdingHandler.LetGoOfWeapon(this.weaponToGrab);
			}
			else
			{
				bool flag3 = this.unit.GetComponentInChildren<HoldingHandlerMulti>();
				if (flag3)
				{
					HoldingHandlerMulti componentInChildren = this.unit.GetComponentInChildren<HoldingHandlerMulti>();
					componentInChildren.LetGoOfAll(false);
				}
			}
		}
	}

	// Token: 0x06000049 RID: 73 RVA: 0x00004CE8 File Offset: 0x00002EE8
	public Weapon SetWeapon(Unit unit, Team team, GameObject weaponObject, PropItemData weaponData, HoldingHandler.HandType handType, Quaternion rotation, List<GameObject> objects, bool isUnitEditor = false)
	{
		WeaponHandler componentInChildren = unit.GetComponentInChildren<WeaponHandler>();
		bool flag = !componentInChildren;
		Weapon result;
		if (flag)
		{
			MultipleWeaponHandler componentInChildren2 = unit.GetComponentInChildren<MultipleWeaponHandler>();
			bool flag2 = componentInChildren2;
			if (flag2)
			{
				componentInChildren2.SetWeapon(weaponObject, handType, false);
			}
			result = null;
		}
		else
		{
			Torso componentInChildren3 = unit.GetComponentInChildren<Torso>();
			bool flag3 = weaponObject;
			if (flag3)
			{
				weaponObject.transform.position = componentInChildren3.transform.position + componentInChildren3.transform.forward * 0.5f;
				weaponObject.transform.rotation = rotation;
				weaponObject.gameObject.transform.SetParent(unit.transform);
				objects.Add(weaponObject);
				Weapon component = weaponObject.GetComponent<Weapon>();
				WeaponItem component2 = weaponObject.GetComponent<WeaponItem>();
				Holdable component3 = weaponObject.GetComponent<Holdable>();
				weaponObject.GetComponent<MeleeWeapon>();
				RangeWeapon component4 = weaponObject.GetComponent<RangeWeapon>();
				bool flag4 = unit.unitBlueprint.removeCloseRangeMiss && component4;
				if (flag4)
				{
					component4.extraCDInMelee = 0f;
					component4.extraSpreadInMelee = 0f;
				}
				bool flag5 = component3;
				if (flag5)
				{
					bool flag6 = component3.useAlternaticeForIceGiant && unit.GetComponent<IceGiant>();
					if (flag6)
					{
						component3.holdableData.relativePosition = component3.iceGiantRelativePosition;
					}
					bool flag7 = handType == HoldingHandler.HandType.Right;
					if (flag7)
					{
						unit.holdingHandler.rightHandActivity = HoldingHandler.HandActivity.HoldingRightObject;
						bool holdinigWithTwoHands = unit.unitBlueprint.holdinigWithTwoHands;
						if (holdinigWithTwoHands)
						{
							unit.holdingHandler.leftHandActivity = HoldingHandler.HandActivity.HoldingRightObject;
						}
						unit.holdingHandler.rightObject = component3;
					}
					else
					{
						unit.holdingHandler.leftHandActivity = HoldingHandler.HandActivity.HoldingLeftObject;
						unit.holdingHandler.leftObject = component3;
						HoldableDataInstance holdableData = component3.holdableData;
						holdableData.relativePosition.x = holdableData.relativePosition.x + unit.unitBlueprint.weaponSeparation;
						HoldableDataInstance holdableData2 = component3.holdableData;
						holdableData2.relativePosition.x = holdableData2.relativePosition.x * -1f;
						HoldableDataInstance holdableData3 = component3.holdableData;
						holdableData3.upRotation.x = holdableData3.upRotation.x * -1f;
						HoldableDataInstance holdableData4 = component3.holdableData;
						holdableData4.forwardRotation.x = holdableData4.forwardRotation.x * -1f;
					}
					component3.holdingHandler = unit.holdingHandler;
				}
				else
				{
					Transform transform = (handType != HoldingHandler.HandType.Right) ? unit.GetComponentInChildren<HandLeft>().transform : unit.GetComponentInChildren<HandRight>().transform;
					weaponObject.transform.position = transform.position;
					weaponObject.transform.rotation = transform.rotation;
					weaponObject.transform.parent = transform;
				}
				componentInChildren.SetWeapon(component, handType);
				component2.Initialize(team);
				bool flag8 = weaponData != null;
				if (flag8)
				{
					component2.SetPropData(weaponData, team);
				}
				result = component;
			}
			else
			{
				result = null;
			}
		}
		return result;
	}

	// Token: 0x04000075 RID: 117
	private Unit unit;

	// Token: 0x04000076 RID: 118
	private bool holdingRight;

	// Token: 0x04000077 RID: 119
	private bool holdingLeft;

	// Token: 0x04000078 RID: 120
	public GameObject weaponToGrab;

	// Token: 0x04000079 RID: 121
	public UnityEvent grabEvent = new UnityEvent();

	// Token: 0x0400007A RID: 122
	public UnityEvent ungrabEvent = new UnityEvent();
}
