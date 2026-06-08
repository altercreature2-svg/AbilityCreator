using Landfall.TABS;
using Landfall.TABS.AI.Components;
using Photon.Bolt.Utils;
using System.Collections;
using System.Collections.Generic;
using TFBGames;
using UnityEngine;

namespace IDK.NodeScripts
{
    public class SpawnExplosionNode : IBehaviorNode
    {
        public override ValuePool GetValuePool(LegacySavedNode savedNode, Unit unit, List<NodeComponent.LegacyConnection> connections, string[] fields)
        {
            return savedNode.GetValuePool(unit);
        }
        public override IEnumerator RunNode(LegacySavedNode savedNode, Unit unit, List<NodeComponent.LegacyConnection> connections, string[] fields, NodeRunner nodeRunner)
        {
            GameObject[] gameObjs = connections.GetNode(NodeBlueprint.ConnectionClass.ReciveGameObject).GetValuePoolSmart(unit).GetValues<GameObject>();
            ValuePool valuePool = savedNode.GetValuePool(unit);
            valuePool.ClearValues();
            for (int o = 0; o < gameObjs.Length; o++)
            {
                var explosion = Object.Instantiate(AbilityCreator.explosions[fields[0]]);
                explosion.transform.position = gameObjs[o].transform.position;
                Debug.Log("Summoned explosion: " + explosion.name);

                explosion.transform.forward = gameObjs[o].transform.forward;
                if (gameObjs[o].transform.root.GetComponent<Unit>())
                {
                    if (explosion.GetComponentInChildren<TeamHolder>())
                    {
                        explosion.GetComponentInChildren<TeamHolder>().team = gameObjs[o].transform.root.GetComponent<Unit>().Team;
                    }
                    explosion.SetActive(true);
                    if (explosion.GetComponentInChildren<TeamHolder>())
                    {
                        explosion.GetComponentInChildren<TeamHolder>().team = gameObjs[o].transform.root.GetComponent<Unit>().Team;
                    }
                    for (int l = 0; l < explosion.GetComponentsInChildren<Explosion>().Length; l++)
                    {
                        explosion.GetComponentsInChildren<Explosion>()[l].damage = fields[1].QuickParse();
                        if (fields[2] == "My team")
                        {
                            typeof(Explosion).GetField("team", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(explosion.GetComponentsInChildren<Explosion>()[l], gameObjs[o].transform.root.GetComponent<Unit>().Team);
                            typeof(Explosion).GetField("ownUnit", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(explosion.GetComponentsInChildren<Explosion>()[l], gameObjs[o].transform.root.GetComponent<Unit>());
                        }
                        else
                        {
                            typeof(Explosion).GetField("team", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(explosion.GetComponentsInChildren<Explosion>()[l], TeamUtlity.GetOtherTeam(gameObjs[o].transform.root.GetComponent<Unit>().Team));
                            typeof(Explosion).GetField("ownUnit", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(explosion.GetComponentsInChildren<Explosion>()[l], gameObjs[o].transform.root.GetComponent<Unit>());
                        }
                    }
                }
                
                valuePool.AddValue(explosion);               
            }
            yield return savedNode.TriggerConnection(nodeRunner);

        }
    }
}