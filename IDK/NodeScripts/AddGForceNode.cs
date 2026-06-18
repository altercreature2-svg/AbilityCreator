using AC.Help;
using AC.Node_Related_Scripts.NodeRunning;
using AC.Node_Related_Scripts.NodeRunning.Instructions.Courtines;
using Landfall.TABC;
using Landfall.TABS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace AC.NodeScripts
{
    public class AddGlobalForceNode : IBehaviorNode
    {
        public IEnumerator<CoroutineReturn> Execute(NodeEnv env)
        {
            var gameobjects = env.GetValues(NodeBlueprint.ConnectionClass.ReciveGameObject);
            foreach (var item in gameobjects)
            {
                float x = env.GetField(0).QuickParse();
                float y = env.GetField(1).QuickParse();
                float z = env.GetField(2).QuickParse();
                Vector3 vector = new Vector3(x, y, z);
                if (!(item.value is GameObject go))
                    continue;
                Rigidbody rb = env.cacheSystem.GetCachedComponent<Rigidbody>(go);
                MoveTransform mt = env.cacheSystem.GetCachedComponent<MoveTransform>(go);
                if (rb)
                    rb.AddForce(vector * 10);
                if (mt)
                    mt.velocity += vector;
            }
            yield return new CoroutineReturn(CoroutineReturn.CourtineType.ContinueBranch);

        }
        public IEnumerator<CoroutineReturn> Cache(NodeEnv env)
        {
            return null;
        }
    }
}