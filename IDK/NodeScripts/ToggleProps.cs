using Landfall.TABS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace IDK.NodeScripts
{
    public class ToggleProps : IBehaviorNode
    {
        public override IEnumerator RunNode(SavedNode savedNode, Unit unit, List<Node.Connection> connections, string[] fields, NodeRunner nodeRunner)
        {
            Unit[] units = connections.GetNode(NodeBlueprint.ConnectionType.ReciveUnit).GetValuePoolSmart(unit).GetValues<Unit>();
            foreach (var unitIndex in units)
            {
                for (int p = (int)fields[0].QuickParse(); p < (int)fields[1].QuickParse(); p++)
                {

                    try
                    {

                        if (unitIndex.GetComponentsInChildren<PropItem>(true)[p].GetComponent<Weapon>())
                        {
                            continue;
                        }
                        if (unitIndex.GetComponentsInChildren<PropItem>(true)[p].GetComponentInChildren<Renderer>().enabled == true)
                        {

                            GameObject[] gameobjs = (GameObject[])typeof(CharacterItem).GetField("m_gameObjects", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(unitIndex.GetComponentsInChildren<PropItem>(true)[p]);
                            for (int q = 0; q < gameobjs.Length; q++)
                            {
                                for (int s = 0; s < gameobjs[q].GetComponentsInChildren<Renderer>(true).Length; s++)
                                {
                                    gameobjs[q].GetComponentsInChildren<Renderer>(true)[s].enabled = false;
                                }

                            }


                        }
                        else if (unitIndex.GetComponentsInChildren<PropItem>(true)[p].GetComponentInChildren<Renderer>().enabled == false)
                        {


                            GameObject[] gameobjs = (GameObject[])typeof(CharacterItem).GetField("m_gameObjects", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(unitIndex.GetComponentsInChildren<PropItem>(true)[p]);
                            for (int q = 0; q < gameobjs.Length; q++)
                            {
                                for (int s = 0; s < gameobjs[q].GetComponentsInChildren<Renderer>(true).Length; s++)
                                {
                                    gameobjs[q].GetComponentsInChildren<Renderer>(true)[s].enabled = true;
                                }

                            }
                        }


                    }
                    catch (IndexOutOfRangeException)
                    {


                        break;
                    }

                }
            }
            yield return savedNode.TriggerConnection(nodeRunner);

        }
    }
}