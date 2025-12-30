using System.Collections;
using UnityEngine;

namespace IDK
{
    public class FixNodeSystem : MonoBehaviour
    {
        public static bool IsValid(SavedNodeScene nodeScene)
        {
            for (int i = 0; i < nodeScene.blueprints2.Count; i++)
            {
                if (!Main.nodeDatabase.ContainsKey(nodeScene.blueprints2[i]))
                {
                    return false;
                }
                string[] fields = nodeScene.fields2[i].Split(separator: new string[] { "###" }, System.StringSplitOptions.RemoveEmptyEntries);
                if (Main.nodeDatabase[nodeScene.blueprints2[i]].fields.Count != fields.Length)
                {
                    return false;
                } 
            }
            return true;
        }
    }
}