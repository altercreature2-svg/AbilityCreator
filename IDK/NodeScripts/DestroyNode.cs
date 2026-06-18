

using AC.Node_Related_Scripts.NodeRunning;
using AC.Node_Related_Scripts.NodeRunning.Instructions.Courtines;
using Landfall.TABS;
using Landfall.TABS.UnitEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AC.NodeScripts
{
    public class DestroyNode : IBehaviorNode
    {
        public IEnumerator<CoroutineReturn> Execute(NodeEnv env)
        {
            var gameObjects = env.GetValues(NodeBlueprint.ConnectionClass.ReciveGameObject);
            foreach (var item in gameObjects)
            {
                if (!(item.value is UnityEngine.Object obj))
                    continue;
                if (obj is GameObject @object)
                {
                    CharacterItem characterItem = @object.GetComponent<CharacterItem>();
                    Debug.Log("prop to destroy:" + characterItem);
                    if (characterItem && !@object.GetComponent<Unit>())
                    {
                        characterItem.Remove();
                        Debug.Log("Removed prop!");
                        continue;
                    }
                }
                UnityEngine.Object.Destroy(obj);
                Debug.Log("Destroyed:" + obj.name + "!");
            }
            yield return new CoroutineReturn(CoroutineReturn.CourtineType.ContinueBranch);
        }
        public IEnumerator<CoroutineReturn> Cache(NodeEnv env)
        {
            return null;
        }
        
    }
}