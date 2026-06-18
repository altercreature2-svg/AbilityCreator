using AC.Node_Related_Scripts.NodeRunning;
using AC.Node_Related_Scripts.NodeRunning.Instructions.Courtines;
using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using UnityEngine;

namespace AC.NodeScripts
{
    public class TransformPositionNode : IBehaviorNode
    {
        public float x, y, z;
        public bool local;
        public bool set;
        public IEnumerator<CoroutineReturn> Execute(NodeEnv env)
        {
            var gameObjects = env.GetValues(NodeBlueprint.ConnectionClass.ReciveGameObject);
            Vector3 vector = new Vector3(x, y, z);
            foreach (var item in gameObjects)
            {
                if (!(item.value is GameObject go))
                    continue;
                Transform transform = go.transform; 
                switch (local)
                {
                    case true:
                        switch (set)
                        {
                            case true:
                                SetLocalPosition(go, vector);
                                break;
                            case false:
                                AddLocalPosition(go, vector);
                                break;
                        }
                        break;
                    case false:
                        switch (set)
                        {
                            case true:
                                transform.position = vector;
                                break;
                            case false:
                                transform.position += vector;
                                break;
                        }
                        break;
                }
            }
            yield return new CoroutineReturn(CoroutineReturn.CourtineType.ContinueBranch);
        }
        public IEnumerator<CoroutineReturn> Cache(NodeEnv env)
        {
            local = env.GetField(3) != "Global";
            set = env.GetField(4) != "Add";
            x = env.GetField(0).QuickParse();
            y = env.GetField(1).QuickParse();
            z = env.GetField(2).QuickParse();
            return null;
        }
        public void SetLocalPosition(GameObject me, Vector3 vector3)
        {
            Vector3 forward = vector3.z * me.transform.forward;
            Vector3 upward = vector3.y * me.transform.up;
            Vector3 right = vector3.x * me.transform.right;
            me.transform.localPosition = forward + upward + right;
        }
        public void AddLocalPosition(GameObject me, Vector3 vector3)
        {
            Vector3 forward = vector3.z * me.transform.forward;
            Vector3 upward = vector3.y * me.transform.up;
            Vector3 right = vector3.x * me.transform.right;
            me.transform.localPosition += forward + upward + right;
        }
    }
}