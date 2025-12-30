using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IDK
{
    public static class SavedNodeSceneStorer 
    {
        public static List<SavedNodeScene> savedNodeScenes = new List<SavedNodeScene>();
        public static List<NodeScene> nodeScenes = new List<NodeScene>();
        public static List<SavedNode> m_savedNodes = new List<SavedNode>();
        public static List<SavedNode> SavedNodes
        {
            get
            {
                if (m_savedNodes == null)
                    m_savedNodes = new List<SavedNode>();
                return m_savedNodes;
            }
        }
        public static List<GameObject> gameobjs = new List<GameObject>();
        public static List<object> objs = new List<object>();
        public static NodeManager manger;
       
    }
}