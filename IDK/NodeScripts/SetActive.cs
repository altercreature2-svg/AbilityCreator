using AC.Node_Related_Scripts.NodeRunning;
using AC.Node_Related_Scripts.NodeRunning.Instructions.Courtines;
using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AC.NodeScripts
{
    public class SetActive : IBehaviorNode
    {
        int mode;
        public IEnumerator<CoroutineReturn> Execute(NodeEnv env)
        {
            var gameObjects = env.GetValues(NodeBlueprint.ConnectionClass.ReciveGameObject);
            foreach (var item in gameObjects)
            {
                if (!(item.value is UnityEngine.GameObject obj))
                    continue;
                switch (mode)
                {
                    case 0:
                        obj.gameObject.SetActive(!obj.activeSelf);
                        break;
                    case 1:
                        obj.gameObject.SetActive(true);
                        break;
                    case 2:
                        obj.gameObject.SetActive(false);
                        break;
                    default:
                        obj.gameObject.SetActive(!obj.activeSelf);
                        break;
                }
            }
            yield return new CoroutineReturn(CoroutineReturn.CourtineType.ContinueBranch);
        }
        public IEnumerator<CoroutineReturn> Cache(NodeEnv env)
        {
            mode = env.GetField(0) == "Toggle" ? 0 : mode;
            mode = env.GetField(0) == "True" ? 1 : mode;
            mode = env.GetField(0) == "False" ? 2 : mode;
            return null;
        }
    }
}