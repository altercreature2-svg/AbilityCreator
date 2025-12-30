using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using TFBGames;
using UnityEngine;

namespace IDK.NodeScripts
{
    public class PlaySoundNode : IBehaviorNode
    {
        public override IEnumerator RunNode(SavedNode savedNode, Unit unit, List<Node.Connection> connections, string[] fields, NodeRunner nodeRunner)
        {
            Debug.Log("Spawning unit...");
            GameObject[] gameObjs = connections.GetNode(NodeBlueprint.ConnectionType.ReciveGameObject).GetValuePoolSmart(unit).GetValues<GameObject>();
            Debug.Log("Length of Gameobjects :" + gameObjs.Length);
            foreach (var gameObj in gameObjs)
            {

                AudioPathData path = null;
                AudioPathData.ValidateAndAssignPathData(fields[0].Replace("&", "/"), ref path);
                AudioPlayer audioPlayer = ServiceLocator.GetService<SoundPlayer>().PlaySoundEffectNonAlloc(path,fields[1].QuickParse(), gameObj.transform.position, SoundEffectVariations.MaterialType.Default, gameObj.transform, fields[2].QuickParse());
                Debug.Log("Playing sound..." + audioPlayer);
                
                
            }
            yield return savedNode.TriggerConnection(nodeRunner);

        }
    }
}