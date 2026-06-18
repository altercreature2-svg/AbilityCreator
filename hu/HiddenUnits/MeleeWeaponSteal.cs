using System;
using System.Collections.Generic;
using Landfall.TABS;
using UnityEngine;

namespace HiddenUnits
{
	// Token: 0x02000020 RID: 32
	public class MeleeWeaponSteal : CollisionWeaponEffect
	{
		// Token: 0x060000B9 RID: 185 RVA: 0x00009D64 File Offset: 0x00007F64
		public override void DoEffect(Transform hitTransform, Collision collision)
		{
			bool flag = hitTransform.GetComponent<Rigidbody>() && hitTransform.root.GetComponent<Unit>() && hitTransform.root.GetComponent<Unit>().Team != base.GetComponent<Weapon>().connectedData.unit.Team && !hitTransform.root.GetComponent<Unit>().data.Dead;
			if (flag)
			{
				GameObject gameObject = null;
				HoldingHandler.HandType handType = HoldingHandler.HandType.Right;
				Unit component = hitTransform.root.GetComponent<Unit>();
				bool flag2 = component.data.health > 600f || component.data.Dead;
				if (!flag2)
				{
					HoldingHandler componentInChildren = component.GetComponentInChildren<HoldingHandler>();
					bool flag3 = componentInChildren;
					if (flag3)
					{
						component.GetComponentInChildren<WeaponHandler>().fistRefernce = null;
						bool flag4 = componentInChildren.rightObject;
						if (flag4)
						{
							gameObject = componentInChildren.rightObject.gameObject;
							handType = HoldingHandler.HandType.Right;
							componentInChildren.LetGoOfWeapon(gameObject);
							Object.Destroy(componentInChildren.rightObject);
						}
						else
						{
							bool flag5 = componentInChildren.leftObject;
							if (flag5)
							{
								gameObject = componentInChildren.leftObject.gameObject;
								handType = HoldingHandler.HandType.Left;
								componentInChildren.LetGoOfWeapon(gameObject);
								Object.Destroy(componentInChildren.leftObject);
							}
						}
					}
					bool flag6 = gameObject != null;
					if (flag6)
					{
						gameObject.transform.SetParent(null);
						bool flag7 = base.transform.root.GetComponentInChildren<HoldingHandler>();
						if (flag7)
						{
							base.transform.root.GetComponentInChildren<HoldingHandler>().LetGoOfWeapon(base.gameObject);
							base.gameObject.AddComponent<RemoveAfterSeconds>().shrink = true;
							Weapon weapon = base.transform.root.GetComponent<Unit>().unitBlueprint.SetWeapon(base.transform.root.GetComponent<Unit>(), base.transform.root.GetComponent<Unit>().Team, gameObject, new PropItemData(), handType, base.transform.root.GetComponent<Unit>().data.mainRig.rotation, new List<GameObject>(), false);
							base.transform.root.GetComponentInChildren<HoldingHandler>().leftHandActivity = componentInChildren.leftHandActivity;
							bool flag8 = weapon.GetComponent<ConfigurableJoint>();
							if (flag8)
							{
								foreach (ConfigurableJoint obj in weapon.GetComponentsInChildren<ConfigurableJoint>())
								{
									Object.Destroy(obj);
								}
							}
							Object.Destroy(gameObject);
						}
					}
				}
			}
		}
	}
}
