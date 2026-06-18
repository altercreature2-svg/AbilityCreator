using AC.Help;
using AC.Help_Componets;
using AC.Node_Related_Scripts.NodeRunning;
using AC.Node_Related_Scripts.NodeRunning.Instructions.Courtines;
using BitCode.Extensions;
using Landfall.TABC;
using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace AC.NodeScripts
{
    public class HoldPostionNode : IBehaviorNode
    {
        public IEnumerator<CoroutineReturn> Execute(NodeEnv env)
        {
            var units = env.GetValues(NodeBlueprint.ConnectionClass.ReciveUnit);
            var gameObjects = env.GetValues(NodeBlueprint.ConnectionClass.ReciveGameObject);
            float pause = env.GetField(0).QuickParse();
            float length = env.GetField(0).QuickParse();
            bool x = env.GetField(1) != "Off";
            bool y = env.GetField(2) != "Off";
            bool z = env.GetField(3) != "Off";
            FixedPool<GameObject> pool = new FixedPool<GameObject>(gameObjects.Length);
            foreach (var item in gameObjects)
            {
                if (!(item.value is GameObject go))
                    continue;
                pool.Insert(go);
            }

            foreach (var item in units)
            {
                if (!(item.value is Unit u))
                    continue;
                u.gameObject.AddComponent<HoldPosition>().Go(length, pool.ToArray().Select(n => env.cacheSystem.GetCachedComponent<Rigidbody>(n)).ToArray(), x, y, z);
            }
            yield return new CoroutineReturn(CoroutineReturn.CourtineType.ContinueBranch);
        }
        public IEnumerator<CoroutineReturn> Cache(NodeEnv env)
        {
            return null;
        }
    }
}