

using Landfall.TABS;
using Landfall.TABS.UnitEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IDK.NodeScripts
{
    public class DestroyNode : IBehaviorNode
    {
        public override IEnumerator RunNode(SavedNode savedNode, Unit unit, List<Node.Connection> connections, string[] fields, NodeRunner nodeRunner)
        {

            Object[] gameObjects = connections.GetNode(NodeBlueprint.ConnectionType.ReciveGameObject).GetValuePoolSmart(unit).GetValues<Object>();
            Debug.Log($"{gameObjects.Length} objects to destroy!");
            foreach (var gameObj in gameObjects)
            {
                if (gameObj is GameObject @object)
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
                
                Object.Destroy(gameObj);
                Debug.Log("Destroyed:" + gameObj.name + "!");
            }
            yield return savedNode.TriggerConnection(nodeRunner);

        }
    }
}