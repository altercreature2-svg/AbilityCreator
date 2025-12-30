using DM;
using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace IDK.NodeScripts
{
    public class SpawnUnitNode : IBehaviorNode
    {
        public override ValuePool GetValuePool(SavedNode savedNode, Unit unit, List<Node.Connection> connections, string[] fields)
        {
            return savedNode.GetValuePool(unit);
        }
        public override IEnumerator RunNode(SavedNode savedNode, Unit unit, List<Node.Connection> connections, string[] fields, NodeRunner nodeRunner)
        {
            Debug.Log("Spawning unit...");
            GameObject[] gameObjs = connections.GetNode(NodeBlueprint.ConnectionType.ReciveGameObject).GetValuePoolSmart(unit).GetValues<GameObject>();
            Debug.Log("Length of Gameobjects :" + gameObjs.Length);
            ValuePool valuePool = savedNode.GetValuePool(unit);
            valuePool.ClearValues();
            foreach (var gameObj in gameObjs)
            {
                GameObject[] spawnedUnit;
                UnitBlueprint unitBlueprint = Main.units[fields[0]];
                if (fields[1] == "My Team")
                {
                    spawnedUnit = Main.units[fields[0]].Spawn(gameObj.transform.position, Quaternion.identity, unit.Team);
                }
                else
                {
                    spawnedUnit = Main.units[fields[0]].Spawn(gameObj.transform.position, Quaternion.identity,unit.Team.Reverse()) ;
                }
                Debug.Log("Spawned unit! unit:" + spawnedUnit[0].transform.root.GetComponent<Unit>().unitBlueprint.Entity.Name);

                if (unitBlueprint.FilePath != null && unitBlueprint.FilePath != "")
                {
                    
                    SerializedUnitBlueprint serializedUnitBlueprint = JsonUtility.FromJson<SerializedUnitBlueprint>(File.ReadAllText(unitBlueprint.FilePath));
                    for (int i = 0; i < serializedUnitBlueprint.m_combatMoves.Length; i++)
                    {
                        Debug.Log("Move: " + serializedUnitBlueprint.m_combatMoves[i].m_ID);
                        try
                        {
                            GameObject move = ContentDatabase.Instance().LandfallContentDatabase.GetCombatMove(serializedUnitBlueprint.m_combatMoves[i]);
                            Object.Instantiate(move, spawnedUnit[0].transform.root);
                        }catch { }
                    }
                } 
                spawnedUnit[0].transform.root.position = gameObj.transform.position;
                Unit unit1 = spawnedUnit[0].transform.root.GetComponent<Unit>();
                Debug.Log("objectsToSpawnAsChildren:" + unit1.unitBlueprint.objectsToSpawnAsChildren.Length);
                for (int i = 0; i < unit1.spawnedObjects.Length; i++)
                {
                    Debug.Log("spawned object:" + unit1.spawnedObjects[i]);
                }
                
                valuePool.AddValue(unit1);
            }
            yield return savedNode.TriggerConnection(nodeRunner);
        }
    }
}