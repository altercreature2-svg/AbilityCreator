using AC.Node_Related_Scripts.NodeRunning;
using AC.Node_Related_Scripts.NodeRunning.Instructions.Courtines;
using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AC.NodeScripts
{
    public class RotateTowardsNode : IBehaviorNode
    {
        public IEnumerator<CoroutineReturn> Execute(NodeEnv env)
        {
            var unitsEnum = env.GetValues(NodeBlueprint.ConnectionClass.ReciveUnit);
            var gosEnum = env.GetValues(NodeBlueprint.ConnectionClass.ReciveGameObject);
            foreach (var item in unitsEnum )
            {
                if (!(item.value is Unit u))
                    continue;
                foreach (var item2 in gosEnum)
                {
                    if (!(item2.value is GameObject go))
                        continue;
                    switch (env.GetField(0))
                    {
                        case "Head":
                            Rotate(go, u.data.head);
                            break;
                        case "Torso":
                            Rotate(go, u.data.torso);
                            break;
                        case "Right Arm":
                            Rotate(go, u.data.rightArm);
                            break;
                        case "Left Arm":
                            Rotate(go, u.data.leftArm);
                            break;
                        case "Right Hand":
                            Rotate(go, u.data.rightHand);
                            break;
                        case "Left Hand":
                            Rotate(go, u.data.leftHand);
                            break;
                        case "Right Knee":
                            Rotate(go, u.data.legRight);
                            break;
                        case "Left Knee":
                            Rotate(go, u.data.legLeft);
                            break;
                        case "Right Foot":
                            Rotate(go, u.data.footRight);
                            break;
                        case "Left Foot":
                            Rotate(go, u.data.footLeft);
                            break;
                        default:
                            Rotate(go, u.data.mainRig.transform);
                            break;
                    }
                }
            }
            yield return new CoroutineReturn(CoroutineReturn.CourtineType.ContinueBranch);
        }
        public IEnumerator<CoroutineReturn> Cache(NodeEnv env)
        {
            return null;
        }
        public void Rotate(GameObject a, Transform b)
        {
            Vector3 dir = b.transform.position - a.transform.position;
            a.transform.rotation = Quaternion.LookRotation(dir);
        }
    }
}