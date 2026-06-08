using Landfall.TABS;
using Landfall.TABS.AI.Components;
using Photon.Bolt.Utils;
using System.Collections;
using System.Collections.Generic;
using TFBGames;
using UnityEngine;

namespace IDK.NodeScripts
{
    public class SpawnParticleNode : IBehaviorNode
    {
        public override ValuePool GetValuePool(LegacySavedNode savedNode, Unit unit, List<NodeComponent.LegacyConnection> connections, string[] fields)
        {
            return savedNode.GetValuePool(unit);
        }
        public override IEnumerator RunNode(LegacySavedNode savedNode, Unit unit, List<NodeComponent.LegacyConnection> connections, string[] fields, NodeRunner nodeRunner)
        {
            ValuePool valuePool = savedNode.GetValuePool(unit);
            valuePool.ClearValues();
            GameObject[] gameObjs = connections.GetNode(NodeBlueprint.ConnectionClass.ReciveGameObject).GetValuePoolSmart(unit).GetValues<GameObject>(); 
            for (int o = 0; o < gameObjs.Length; o++)
            {
                var particle = Object.Instantiate(AbilityCreator.particles[fields[0]]);
                particle.transform.position = gameObjs[o].transform.position;
                if (fields[1].QuickParse() != 0)
                    particle.transform.localScale *= fields[1].QuickParse();
                if (fields[2] == "Don't")
                    particle.GetComponent<ParticleSystem>().loop = false;
                else
                    particle.GetComponent<ParticleSystem>().loop = true;
                particle.GetComponent<ParticleSystem>().playbackSpeed *= fields[3].QuickParse();
                if (fields[5] == "Follow")
                    particle.transform.parent = gameObjs[o].transform;
                particle.GetComponent<ParticleSystem>().Play();
                particle.AddComponent<DestroySelfWhenObjectDestroyed>().obj = gameObjs[o];
                particle.AddComponent < RemoveAfterSeconds>().seconds = fields[4].QuickParse();
                Debug.Log("Summoned particle: " + particle.name);
                
                valuePool.AddValue(particle);
            }
            yield return savedNode.TriggerConnection(nodeRunner);

        }
    }
}