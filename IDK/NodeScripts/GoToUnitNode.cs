using AC.Node_Related_Scripts.NodeRunning;
using AC.Node_Related_Scripts.NodeRunning.Instructions.Courtines;
using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AC.NodeScripts
{
    public class GoToUnitNode : IBehaviorNode
    {
        public IEnumerator<CoroutineReturn> Execute(NodeEnv env)
        {
            var units = env.GetValues(NodeBlueprint.ConnectionClass.ReciveUnit);
            var gameobjects = env.GetValues(NodeBlueprint.ConnectionClass.ReciveGameObject);
            string option = env.GetField(0);
            string option2 = env.GetField(1);
            foreach (var item in units)
            {
                if (!(item.value is Unit u))
                    continue;
                foreach (var item2 in gameobjects)
                {
                    if (!(item2.value is GameObject go))
                        continue;
                    if (option == "Both")
                    {
                        if (option2 == "Head")
                        {
                            TeleportObject(go, u.data.head);
                        }
                        if (option2 == "Torso")
                        {
                            TeleportObject(go, u.data.head);
                        }
                        if (option2 == "Right Arm")
                        {
                            TeleportObject(go, u.data.rightArm);
                        }
                        if (option2 == "Left Arm")
                        {
                            TeleportObject(go, u.data.leftArm);
                        }
                        if (option2 == "Right Hand")
                        {
                            TeleportObject(go, u.data.rightHand);
                        }
                        if (option2 == "Left Hand")
                        {
                            TeleportObject(go, u.data.leftHand);
                        }
                        if (option2 == "Right Knee")
                        {
                            TeleportObject(go, u.data.legRight);
                        }
                        if (option2 == "Left Knee")
                        {
                            TeleportObject(go, u.data.legLeft);
                        }
                        if (option2 == "Right Foot")
                        {
                            TeleportObject(go, u.data.footRight);
                        }
                        if (option2 == "Left Foot")
                        {
                            TeleportObject(go, u.data.footLeft);
                        }
                    }
                    else if (option == "Position only")
                    {
                        if (option2 == "Head")
                        {
                            TeleportPos(go, u.data.head);
                        }
                        if (option2 == "Torso")
                        {
                            TeleportPos(go, u.data.head);
                        }
                        if (option2 == "Right Arm")
                        {
                            TeleportPos(go, u.data.rightArm);
                        }
                        if (option2 == "Left Arm")
                        {
                            TeleportPos(go, u.data.leftArm);
                        }
                        if (option2 == "Right Hand")
                        {
                            TeleportPos(go, u.data.rightHand);
                        }
                        if (option2 == "Left Hand")
                        {
                            TeleportPos(go, u.data.leftHand);
                        }
                        if (option2 == "Right Knee")
                        {
                            TeleportPos(go, u.data.legRight);
                        }
                        if (option2 == "Left Knee")
                        {
                            TeleportPos(go, u.data.legLeft);
                        }
                        if (option2 == "Right Foot")
                        {
                            TeleportPos(go, u.data.footRight);
                        }
                        if (option2 == "Left Foot")
                        {
                            TeleportPos(go, u.data.footLeft);
                        }
                    }
                    else
                    {
                        if (option2 == "Head")
                        {
                            TeleportRot(go, u.data.head);
                        }
                        if (option2 == "Torso")
                        {
                            TeleportRot(go, u.data.head);
                        }
                        if (option2 == "Right Arm")
                        {
                            TeleportRot(go, u.data.rightArm);
                        }
                        if (option2 == "Left Arm")
                        {
                            TeleportRot(go, u.data.leftArm);
                        }
                        if (option2 == "Right Hand")
                        {
                            TeleportRot(go, u.data.rightHand);
                        }
                        if (option2 == "Left Hand")
                        {
                            TeleportRot(go, u.data.leftHand);
                        }
                        if (option2 == "Right Knee")
                        {
                            TeleportRot(go, u.data.legRight);
                        }
                        if (option2 == "Left Knee")
                        {
                            TeleportRot(go, u.data.legLeft);
                        }
                        if (option2 == "Right Foot")
                        {
                            TeleportRot(go, u.data.footRight);
                        }
                        if (option2 == "Left Foot")
                        {
                            TeleportRot(go, u.data.footLeft);
                        }
                    }

                }
            }
            yield return new CoroutineReturn(CoroutineReturn.CourtineType.ContinueBranch);
        }
        public IEnumerator<CoroutineReturn> Cache(NodeEnv env)
        {
            return null;
        }
        public void TeleportObject(GameObject a, Transform b)
        {
            a.transform.position = b.position;
            a.transform.rotation = b.rotation;
        }
        public void TeleportPos(GameObject a, Transform b)
        {
            a.transform.position = b.position;
        }
        public void TeleportRot(GameObject a, Transform b)
        {
            a.transform.rotation = b.rotation;
        }
    }
}