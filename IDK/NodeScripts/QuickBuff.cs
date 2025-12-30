using ExitGames.Client.Photon.StructWrapping;
using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IDK.NodeScripts
{
    public class QuickBuff : IBehaviorNode
    {
        public override IEnumerator RunNode(SavedNode savedNode, Unit unit, List<Node.Connection> connections, string[] fields, NodeRunner nodeRunner)
        {
            Unit[] units = connections.GetNode(NodeBlueprint.ConnectionType.ReciveUnit).GetValuePoolSmart(unit).GetValues<Unit>();
            for (int i = 0; i < units.Length; i++)
            {
                Buff(unit, fields[1].QuickParse(), fields[2], fields[0].QuickParse());
            }
            yield return savedNode.TriggerConnection(nodeRunner);
            yield return new WaitForSeconds(fields[0].QuickParse());
            for (int i = 0; i < units.Length; i++)
            {
                Debuff(unit, fields[1].QuickParse(), fields[2]);
            }
        }
        public void Buff(Unit unit, float buff, string mode, float length)
        {
            if (mode == "Heal only" || mode == "All")
                unit.data.health += (unit.data.maxHealth * buff) / 10;
            try
            {
                if (mode == "Speed up" || mode == "All")
                    unit.GetComponentInChildren<MovementHandler>().multiplier += buff;
            }
            catch { }
            Weapon[] weapons = unit.GetComponentsInChildren<Weapon>();
            for (int i = 0; i < weapons.Length; i++)
            {
                if (mode == "Increase damage" || mode == "All")
                {
                    weapons[i].levelMultiplier += (weapons[i].levelMultiplier * buff) / 10;
                }
            }
            if (mode == "Speed up attacks" || mode == "All")
            {
                unit.gameObject.AddComponent<AttackSpeedOverTimeEffect>().attackSpeedToAdd = buff;
                unit.gameObject.GetComponent<AttackSpeedOverTimeEffect>().decaySpeed = buff/length;
                unit.gameObject.GetComponent<AttackSpeedOverTimeEffect>().procEvent = new UnityEngine.Events.UnityEvent();
                unit.gameObject.GetComponent<AttackSpeedOverTimeEffect>().DoEffect();
            }

        }
        public void Debuff(Unit unit, float buff, string mode)
        {
            try
            {
                if (mode == "Heal only" || mode == "All")
                    unit.GetComponentInChildren<MovementHandler>().multiplier -= buff;
            }
            catch { }

            Weapon[] weapons = unit.GetComponentsInChildren<Weapon>();
            for (int i = 0; i < weapons.Length; i++)
            {
                if (mode == "Increase damage" || mode == "All")
                {
                    weapons[i].levelMultiplier -= (weapons[i].levelMultiplier * buff) / 10;
                }
            }
        }
    }
}