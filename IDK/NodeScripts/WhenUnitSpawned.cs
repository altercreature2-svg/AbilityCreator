using AC.Node_Related_Scripts.NodeRunning;
using AC.Node_Related_Scripts.NodeRunning.Instructions.Courtines;
using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AC.NodeScripts
{
    public class WhenUnitSpawned : ITriggerNode
    {
        public IEnumerator<CoroutineReturn> Execute(NodeEnv env)
        {
            return null;
        }
        public IEnumerator<CoroutineReturn> Cache(NodeEnv env)
        {
            yield return new CoroutineReturn(CoroutineReturn.CourtineType.WaitUntil, arg0: 0, arg1: () => env.unit.data); // cause it'll only get the data once the unit is spawned properly
            env.RunTrigger();
        }

    }
}