using UnityEngine;

namespace IDK
{
    public class DontDestroyOnLoad
    {
        public Object gameObject;
        public DontDestroyOnLoad(Object target)
        {
            GameObject.DontDestroyOnLoad(target);
            gameObject = target;
        }
    }
}