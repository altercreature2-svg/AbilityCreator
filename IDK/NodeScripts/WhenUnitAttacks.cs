using AC.Node_Related_Scripts;
using AC.Node_Related_Scripts.NodeRunning;
using AC.Node_Related_Scripts.NodeRunning.Instructions.Courtines;
using Landfall.TABS;
using System;
using System.Collections.Generic;

namespace AC.NodeScripts
{
    public class WhenUnitAttacks : ITriggerNode
    {
        public IEnumerator<CoroutineReturn> Execute(NodeEnv env)
        {
            return null;
        }
        public IEnumerator<CoroutineReturn> Cache(NodeEnv env)
        {
            
            env.unit.data.weaponHandler.AttackStarted += (target, ignore1, ignore2, ignore3, ignore4) => OnAttack(env, target);
            return null;
        }
        public void OnAttack(NodeEnv env, Unit u)
        {
            env.AddValue(NodeBlueprint.ConnectionClass.GiveUnit,u);
        }
    
    }

}