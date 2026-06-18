using AC.Node_Related_Scripts.NodeRunning;
using AC.Node_Related_Scripts.NodeRunning.Instructions.Courtines;
using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AC.NodeScripts
{
    public class QuickBuff : IBehaviorNode
    {
        float length;
        float buff;
        string mode;
        public IEnumerator<CoroutineReturn> Execute(NodeEnv env)
        {
            var unitsEnum = env.GetValues(NodeBlueprint.ConnectionClass.ReciveUnit);
            foreach (var item in unitsEnum)
            {
                if (!(item.value is Unit u))
                    continue;
                DeBuff(env, buff, mode, length);
                env.runner.StartCoroutine(ScheduleDebuff(length, buff, mode, env));
            }
            yield return new CoroutineReturn(CoroutineReturn.CourtineType.ContinueBranch);
        }
        public IEnumerator<CoroutineReturn> Cache(NodeEnv env)
        {
            length = env.GetField(0).QuickParse();
            buff = env.GetField(1).QuickParse();
            mode = env.GetField(2);
            return null;
        }
        public IEnumerator ScheduleDebuff(float time, float buff, string mode, NodeEnv env)
        {
            yield return new WaitForSeconds(time);
            DeBuff(env, buff, mode, time);
        }
        public void Buff(NodeEnv env, float buff, string mode, float length)
        {
            switch (mode)
            {
                case "Heal Only":
                    env.unit.data.health += (env.unit.data.maxHealth * buff) / 10;
                    break;
                case "Increase damage":
                    // THIS IS FASTER THAN REFLECTION, STOP BEING STUPID
                    Weapon[] allWeapons = env.cacheSystem.GetCachedComponentsInChildren<Weapon>(env.unit.gameObject);
                    for (int i = 0; i < allWeapons.Length; i++)
                    {
                        float oldLevelMultiplier = allWeapons[i].levelMultiplier;
                        allWeapons[i].levelMultiplier += (allWeapons[i].levelMultiplier * buff) / 10;
                        
                        Level level = env.cacheSystem.GetCachedComponent<Level>(allWeapons[i].gameObject);
                        if (level)
                            level.levelMultiplier += (level.levelMultiplier * buff) / 10;

                        if (allWeapons[i] is MeleeWeapon meleeWeapon)
                        {
                            CollisionWeapon collisionWeapon = env.cacheSystem.GetCachedComponent<CollisionWeapon>(meleeWeapon.gameObject);
                            if (!collisionWeapon)
                                continue;
                            collisionWeapon.damage /= oldLevelMultiplier;
                            collisionWeapon.damage *= meleeWeapon.levelMultiplier;
                        }
                    }
                    break;
                case "Speed up":
                    MovementHandler movementHandler = env.cacheSystem.GetCachedComponentInChildren<MovementHandler>(env.unit.gameObject);
                    if (movementHandler)
                        movementHandler.multiplier += buff;
                    break;
                case "Speed up attacks":
                    AttackSpeedOverTimeEffect attackSpeedOverTimeEffect = env.unit.gameObject.AddComponent<AttackSpeedOverTimeEffect>();
                    attackSpeedOverTimeEffect.attackSpeedToAdd = buff;
                    attackSpeedOverTimeEffect.decaySpeed = buff / length;
                    attackSpeedOverTimeEffect.procEvent = new UnityEngine.Events.UnityEvent();
                    attackSpeedOverTimeEffect.DoEffect();
                    break;
                default:
                    Buff(env, buff, "Increase damage", length);
                    Buff(env, buff, "Speed up", length);
                    Buff(env, buff, "Speed up attacks", length);
                    Buff(env, buff, "Heal Only", length);
                    break;
            }
        }
        public void DeBuff(NodeEnv env, float buff, string mode, float length)
        {
            switch (mode)
            {
                case "Increase damage":
                    // this works shut up
                    Weapon[] allWeapons = env.cacheSystem.GetCachedComponentsInChildren<Weapon>(env.unit.gameObject);
                    for (int i = 0; i < allWeapons.Length; i++)
                    {
                        float oldLevelMultiplier = allWeapons[i].levelMultiplier;
                        allWeapons[i].levelMultiplier -= (allWeapons[i].levelMultiplier * buff) / 10;

                        Level level = env.cacheSystem.GetCachedComponent<Level>(allWeapons[i].gameObject);
                        if (level)
                            level.levelMultiplier -= (level.levelMultiplier * buff) / 10;

                        if (allWeapons[i] is MeleeWeapon meleeWeapon)
                        {
                            CollisionWeapon collisionWeapon = env.cacheSystem.GetCachedComponent<CollisionWeapon>(meleeWeapon.gameObject);
                            if (!collisionWeapon)
                                continue;
                            collisionWeapon.damage /= oldLevelMultiplier;
                            collisionWeapon.damage *= meleeWeapon.levelMultiplier;
                        }
                    }
                    break;
                case "Speed up":
                    MovementHandler movementHandler = env.cacheSystem.GetCachedComponentInChildren<MovementHandler>(env.unit.gameObject);
                    if (movementHandler)
                        movementHandler.multiplier -= buff;
                    break;
                default:
                    DeBuff(env, buff, "Increase damage", length);
                    DeBuff(env, buff, "Speed up", length);
                    break;
            }

        }
    }
}