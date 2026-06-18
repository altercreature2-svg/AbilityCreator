using AC.Node_Related_Scripts.NodeRunning;
using AC.Node_Related_Scripts.NodeRunning.Instructions.Courtines;
using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AC.NodeScripts
{
    public class TransformRotationNode : IBehaviorNode // son i dont give a fuck that transform position and transform rotation have different meanings of local
    {
        public float x, y, z;
        public bool local;
        public bool set;
        public IEnumerator<CoroutineReturn> Execute(NodeEnv env)
        {
            var gameObjects = env.GetValues(NodeBlueprint.ConnectionClass.ReciveGameObject);
           
            foreach (var item in gameObjects)
            {
                if (!(item.value is GameObject go))
                    continue;
                Transform transform = go.transform;
                Vector3 rotation = transform.rotation.eulerAngles;
                Vector3 localRotation = transform.localRotation.eulerAngles;
                Vector3 rotationOffset = new Vector3(x, y, z);
                switch (local)
                {
                    case true:
                        switch (set)
                        {
                            case true:
                                transform.localRotation = Quaternion.Euler(rotationOffset);
                                break;
                            case false:
                                transform.localRotation = Quaternion.Euler(localRotation + rotationOffset);
                                break;
                        }
                        break;
                    case false:
                        switch (set)
                        {
                            case true:
                                transform.rotation = Quaternion.Euler(rotationOffset);
                                break;
                            case false:
                                transform.rotation = Quaternion.Euler(rotation + rotationOffset);
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
    }
}