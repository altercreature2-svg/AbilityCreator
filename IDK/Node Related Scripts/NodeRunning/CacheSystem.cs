using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace IDK.Node_Related_Scripts.NodeRunning
{
    public class CacheSystem : MonoBehaviour
    {
        public struct CachedMethod : IEquatable<CachedMethod>
        {
            public CachedMethod(string methodName, object[] args)
            {
                this.methodName = methodName;
                this.args = args;
            }
            public bool Equals(CachedMethod obj)
            {
                return obj.methodName == methodName && obj.args == args;
            }
            public string methodName;
            public object[] args;
        }
        public class MethodCache<T> : GenericMethodCache
        {
            public T result => (T)untypedResult;
        }
        public class GenericMethodCache
        {
            public CachedMethod method;
            public object untypedResult;
        }
        public List<GenericMethodCache> cacheList = new List<GenericMethodCache>();
        public void Cache<T>(CachedMethod cachedMethod, T value)
        {
            cacheList.Add(new MethodCache<T>
            {
                method = cachedMethod,
                untypedResult = value,
            });
        }
        public T GetCache<T>(CachedMethod cachedMethod)
        {
            for (int i = 0; i < cacheList.Count; i++)
            {
                if (cacheList[i].method.Equals(cachedMethod))
                {
                    return (T)cacheList[i].untypedResult;
                }
            }
            return default(T);
        }
        public object GetCache(CachedMethod cachedMethod)
        {
            for (int i = 0; i < cacheList.Count; i++)
            {
                if (cacheList[i].method.Equals(cachedMethod))
                {
                    return cacheList[i].untypedResult;
                }
            }
            return null;
        }
    }
}
