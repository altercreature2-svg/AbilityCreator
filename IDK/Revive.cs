
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Landfall.TABS;
using Landfall.TABS.AI;
using Landfall.TABS.AI.Components.Modifiers;
using Landfall.TABS.GameMode;
using NaughtyAttributes;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Events;

namespace AC
{
    
    public class Revive : MonoBehaviour
    {
        public string _ = "this totally not just stolen from hidden units :pray: sorry big B dont whoop me";


        // Token: 0x040001B0 RID: 432

        private Unit Unit;

        // Token: 0x040001B1 RID: 433
        private EyeSpawner EyeSpawner;

        // Token: 0x040001B2 RID: 434
        [Header("Revive Settings")]
        public UnityEvent preReviveEvent = new UnityEvent();

        // Token: 0x040001B3 RID: 435
        public UnityEvent reviveEvent = new UnityEvent();

        // Token: 0x040001B4 RID: 436
        public float reviveDelay = 4f;

        // Token: 0x040001B5 RID: 437
        [Range(0f, 1f)]
        public float reviveHealthMultiplier = 0.5f;

        // Token: 0x040001B6 RID: 438
        public bool openEyes = true;

        // Token: 0x040001B7 RID: 439
        [Header("Weapon Settings")]
        public List<GameObject> reviveAbilities;

        // Token: 0x040001B8 RID: 440
        public bool letGoOfWeapons;

        // Token: 0x040001B9 RID: 441
        public bool useWeaponsAfterRevive = true;

        // Token: 0x040001BA RID: 442
        public GameObject rightWeaponToSpawn;

        // Token: 0x040001BB RID: 443
        public GameObject leftWeaponToSpawn;

        // Token: 0x040001BC RID: 444
        private GameObject RightWeaponOriginal;

        // Token: 0x040001BD RID: 445
        private GameObject LeftWeaponOriginal;

        // Token: 0x040001BE RID: 446
        public bool holdWithTwoHands;

        // Token: 0x040001BF RID: 447
        public float removeWeaponsAfterSeconds;
        // Token: 0x06000133 RID: 307 RVA: 0x0000B2A8 File Offset: 0x000094A8
        public void Awake()
        {
            this.Unit = base.transform.root.GetComponent<Unit>();
            this.EyeSpawner = this.Unit.GetComponentInChildren<EyeSpawner>();
            this.Unit.data.healthHandler.willBeRewived = true;
            bool flag = this.Unit.data.weaponHandler.rightWeapon != null && this.Unit.data.weaponHandler.rightWeapon.GetComponent<Holdable>();
            if (flag)
            {
                this.RightWeaponOriginal = this.Unit.data.weaponHandler.rightWeapon.gameObject;
                bool flag2 = !this.letGoOfWeapons;
                if (flag2)
                {
                    this.Unit.data.weaponHandler.rightWeapon.GetComponent<Holdable>().ignoreDissarm = true;
                }
            }
            bool flag3 = this.Unit.data.weaponHandler.leftWeapon != null && this.Unit.data.weaponHandler.leftWeapon.GetComponent<Holdable>();
            if (flag3)
            {
                this.LeftWeaponOriginal = this.Unit.data.weaponHandler.leftWeapon.gameObject;
                bool flag4 = !this.letGoOfWeapons;
                if (flag4)
                {
                    this.Unit.data.weaponHandler.leftWeapon.GetComponent<Holdable>().ignoreDissarm = true;
                }
            }
            bool flag5 = this.Unit.GetComponentInChildren<AddRigidbodyOnDeath>();
            if (flag5)
            {
                foreach (AddRigidbodyOnDeath addRigidbodyOnDeath in this.Unit.GetComponentsInChildren<AddRigidbodyOnDeath>())
                {
                    this.Unit.data.healthHandler.RemoveDieAction(new System.Action(addRigidbodyOnDeath.Die));
                    Object.Destroy(addRigidbodyOnDeath);
                }
            }
            bool flag6 = this.Unit.GetComponentInChildren<SinkOnDeath>();
            if (flag6)
            {
                foreach (SinkOnDeath sinkOnDeath in this.Unit.GetComponentsInChildren<SinkOnDeath>())
                {
                    this.Unit.data.healthHandler.RemoveDieAction(new System.Action(sinkOnDeath.Sink)) ;
                    Object.Destroy(sinkOnDeath);
                }
            }
            bool flag7 = this.Unit.GetComponentInChildren<RemoveJointsOnDeath>();
            if (flag7)
            {
                foreach (RemoveJointsOnDeath removeJointsOnDeath in this.Unit.GetComponentsInChildren<RemoveJointsOnDeath>())
                {
                    this.Unit.data.healthHandler.RemoveDieAction(new System.Action(removeJointsOnDeath.Die));
                    Object.Destroy(removeJointsOnDeath);
                }
            }
            bool flag8 = this.Unit.GetComponentInChildren<DisableAllSkinnedClothes>();
            if (flag8)
            {
                foreach (DisableAllSkinnedClothes disableAllSkinnedClothes in this.Unit.GetComponentsInChildren<DisableAllSkinnedClothes>())
                {
                    Object.Destroy(disableAllSkinnedClothes);
                }
            }
        }

        // Token: 0x06000134 RID: 308 RVA: 0x0000B5A0 File Offset: 0x000097A0
        public void DoRevive()
        {
            base.StartCoroutine(this.Revival());
        }

        // Token: 0x06000135 RID: 309 RVA: 0x0000B5B0 File Offset: 0x000097B0
        public IEnumerator Revival()
        {
            this.Unit.data.Dead = false;
            this.Unit.dead = false;
            this.Unit.data.hasBeenRevived = true;
            this.Unit.data.ragdollControl = 1f;
            this.Unit.data.muscleControl = 1f;
            this.Unit.data.health = this.Unit.data.maxHealth;
            try
            {
                foreach (GooglyEye eye in GetComponentsInChildren<GooglyEye>())
                {
                    eye.dead.SetActive(false);
                    eye.currentEyeState = 0;
                    eye.SetState(0);
                    GooglyEyes.instance?.AddEye(eye);
                }

            }
            catch
            {
                
            }
            bool flag16 = this.Unit.unitBlueprint.MovementComponents != null && this.Unit.unitBlueprint.MovementComponents.Count > 0;
            if (flag16)
            {
                foreach (IMovementComponent mov in this.Unit.unitBlueprint.MovementComponents)
                {
                    try
                    {
                        MethodInfo mi = (MethodInfo)this.Unit.api.CallMethod("CreateGenericRemoveComponentData", new object[]
                        {
                        mov.GetType()
                        });
                        mi.Invoke(this.Unit.GetComponent<GameObjectEntity>().EntityManager, new object[]
                        {
                        this.Unit.GetComponent<GameObjectEntity>().Entity
                        });
                    } catch { }
                }
                
            }
            this.Unit.data.healthHandler.deathEvent.RemoveAllListeners();
            foreach (AddRigidbodyOnDeath rigidbodyOnDeath in this.Unit.GetComponentsInChildren<AddRigidbodyOnDeath>())
            {
                try
                {
                    this.Unit.data.healthHandler.RemoveDieAction(new System.Action(rigidbodyOnDeath.Die));
                }
                catch { }
            }
            foreach (DeathEvent deathEvent in this.Unit.GetComponentsInChildren<DeathEvent>())
            {
                try
                {
                    this.Unit.data.healthHandler.RemoveDieAction(new System.Action(deathEvent.Die));
                } catch { }
            }
            try { ServiceLocator.GetService<UnitHealthbars>().HandleUnitSpawned(this.Unit); } catch {};
            this.Unit.api.SetTargetingType(this.Unit.unitBlueprint.TargetingComponent);
            this.Unit.api.UpdateECSValues();
            this.Unit.InitializeUnit(this.Unit.Team);
            
            
            yield break;
        }

        
    }
}
