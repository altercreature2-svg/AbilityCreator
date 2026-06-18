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
using static RootMotion.FinalIK.RagdollUtility;

namespace AC.NodeScripts
{
    public class AddForceNode : IBehaviorNode
    {
        public IEnumerator<CoroutineReturn> Execute(NodeEnv env)
        {
            var gameobjects = env.GetValues(NodeBlueprint.ConnectionClass.ReciveGameObject);
            FixedPool<Rigidbody> rigs = new FixedPool<Rigidbody>(gameobjects.Length);
            foreach (var item in gameobjects)
            {
                float x = env.GetField(0).QuickParse();
                float y = env.GetField(1).QuickParse();
                float z = env.GetField(2).QuickParse();
                if (!(item.value is GameObject go))
                    continue;
                Rigidbody rb = env.cacheSystem.GetCachedComponent<Rigidbody>(go);
                MoveTransform mt = env.cacheSystem.GetCachedComponent<MoveTransform>(go);
                Transform transform = go.transform;
                if (rb)
                {
                    
                    rb.AddForce(transform.forward * x * 10);
                    rb.AddForce(transform.up * y * 10);
                    rb.AddForce(transform.right * z * 10);
                }
                if (mt)
                {
                    mt.velocity += transform.forward * x;
                    mt.velocity += transform.up * y;
                    mt.velocity += transform.right * z;
                }
                
            }
            yield return new CoroutineReturn(CoroutineReturn.CourtineType.ContinueBranch);
            
        }
        public IEnumerator<CoroutineReturn> Cache(NodeEnv env)
        {
            return null;
        }
    }
}