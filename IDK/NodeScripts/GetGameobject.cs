using AC.Node_Related_Scripts.NodeRunning;
using AC.Node_Related_Scripts.NodeRunning.Instructions.Courtines;
using Landfall.TABS;
using Landfall.TABS.UnitEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AC.NodeScripts
{
    public class GetGameobject : IValueNode
    {
        public IEnumerator<CoroutineReturn> Execute(NodeEnv env)
        {
            env.ClearValue(NodeBlueprint.ConnectionClass.GiveGameObject);
            var unitsEnum = env.GetValues(NodeBlueprint.ConnectionClass.ReciveUnit);
            foreach (var item in unitsEnum)
            {
                if (!(item.value is Unit u))
                    continue;
                switch (env.GetField(0))
                {
                    case "Root":
                        env.AddValue(NodeBlueprint.ConnectionClass.GiveGameObject, u.gameObject);
                        break;
                    case "Torso":
                        env.AddValue(NodeBlueprint.ConnectionClass.GiveGameObject, u.data.torso?.gameObject);
                        break;
                    case "Hip":
                        env.AddValue(NodeBlueprint.ConnectionClass.GiveGameObject, u.data.hip?.gameObject);
                        break;
                    case "Head":
                        env.AddValue(NodeBlueprint.ConnectionClass.GiveGameObject, u.data.head?.gameObject);
                        break;
                    case "Right Arm":
                        env.AddValue(NodeBlueprint.ConnectionClass.GiveGameObject, u.data.rightArm?.gameObject);
                        break;
                    case "Left Arm":
                        env.AddValue(NodeBlueprint.ConnectionClass.GiveGameObject, u.data.leftArm?.gameObject);
                        break;
                    case "Right Hand":
                        env.AddValue(NodeBlueprint.ConnectionClass.GiveGameObject, u.data.rightHand?.gameObject);
                        break;
                    case "Left Hand":
                        env.AddValue(NodeBlueprint.ConnectionClass.GiveGameObject, u.data.leftHand?.gameObject);
                        break;
                    case "Right Knee":
                        env.AddValue(NodeBlueprint.ConnectionClass.GiveGameObject, u.data.legRight?.gameObject);
                        break;
                    case "Left Knee":
                        env.AddValue(NodeBlueprint.ConnectionClass.GiveGameObject, u.data.legLeft?.gameObject);
                        break;
                    case "Right Foot":
                        env.AddValue(NodeBlueprint.ConnectionClass.GiveGameObject, u.data.footRight?.gameObject);
                        break;
                    case "Left Foot":
                        env.AddValue(NodeBlueprint.ConnectionClass.GiveGameObject, u.data.footLeft?.gameObject);
                        break;
                    case "Mesh":
                        SkinnedMeshRenderer skinned = env.cacheSystem.GetCachedComponentInChildren<SkinnedMeshRenderer>(u.unitBlueprint.UnitBase);
                        GameObject mesh = env.cacheSystem.GetCachedComponentsInChildren<SkinnedMeshRenderer>(u.gameObject).FirstOrDefault(n => n.sharedMesh == skinned.sharedMesh).transform.parent.gameObject;
                        env.AddValue(NodeBlueprint.ConnectionClass.GiveGameObject, mesh);
                        break;
                    case "Both Foots":
                        env.AddValue(NodeBlueprint.ConnectionClass.GiveGameObject, u.data.footRight?.gameObject);
                        env.AddValue(NodeBlueprint.ConnectionClass.GiveGameObject, u.data.footLeft?.gameObject);
                        break;
                    case "Both Hands":
                        env.AddValue(NodeBlueprint.ConnectionClass.GiveGameObject, u.data.rightHand?.gameObject);
                        env.AddValue(NodeBlueprint.ConnectionClass.GiveGameObject, u.data.leftHand?.gameObject);
                        break;
                    default:
                        env.AddValue(NodeBlueprint.ConnectionClass.GiveGameObject, u.data.torso?.gameObject);
                        env.AddValue(NodeBlueprint.ConnectionClass.GiveGameObject, u.data.hip?.gameObject);
                        env.AddValue(NodeBlueprint.ConnectionClass.GiveGameObject, u.data.head?.gameObject);
                        env.AddValue(NodeBlueprint.ConnectionClass.GiveGameObject, u.data.rightArm?.gameObject);
                        env.AddValue(NodeBlueprint.ConnectionClass.GiveGameObject, u.data.leftArm?.gameObject);
                        env.AddValue(NodeBlueprint.ConnectionClass.GiveGameObject, u.data.rightHand?.gameObject);
                        env.AddValue(NodeBlueprint.ConnectionClass.GiveGameObject, u.data.leftHand?.gameObject);
                        env.AddValue(NodeBlueprint.ConnectionClass.GiveGameObject, u.data.legRight?.gameObject);
                        env.AddValue(NodeBlueprint.ConnectionClass.GiveGameObject, u.data.legLeft?.gameObject);
                        env.AddValue(NodeBlueprint.ConnectionClass.GiveGameObject, u.data.footRight?.gameObject);
                        env.AddValue(NodeBlueprint.ConnectionClass.GiveGameObject, u.data.footLeft?.gameObject);
                        break;
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