using UnityEngine;

namespace AC
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