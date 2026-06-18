using AC.Help;
using Landfall.TABS;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

namespace AC.Node_Related_Scripts.NodeRunning
{
    public class ComponentCacheSystem : MonoBehaviour
    {
        public const int MAX_GO_CACHE = 64;
        public const int MAX_COMPONENT_CACHE = 16;
        public struct ComponentCacheKey
        {
            public enum KeyType
            {
                GetComponent,
                GetComponents,
                GetComponentInChildren,
                GetComponentsInChildren,
            }
            public KeyType keyType;
            public Type type;
            public ComponentCacheKey(KeyType keyType, Type type)
            {
                this.keyType = keyType;
                this.type = type;
            }
        }
        public struct GOCacheEntry
        {
            public GameObject go;
            public FixedDictionaryPool<ComponentCacheKey, object> componentCacheEntries;
            public GOCacheEntry(GameObject go)
            {
                this.go = go;
                this.componentCacheEntries = new FixedDictionaryPool<ComponentCacheKey, object>(MAX_COMPONENT_CACHE);
            }
        }
        public ComponentCacheSystem()
        {
            cacheEntries = new FixedDictionaryPool<GameObject, GOCacheEntry>(MAX_COMPONENT_CACHE);
        }
        public FixedDictionaryPool<GameObject, GOCacheEntry> cacheEntries;
        public object GetCachedComponent(System.Type type, GameObject go)
        {
            object result = null;
            ComponentCacheKey componentCacheKey = new ComponentCacheKey(ComponentCacheKey.KeyType.GetComponent, type);
            if (cacheEntries.TryGetValue(go, out GOCacheEntry entry))
            {
                if (entry.componentCacheEntries.TryGetValue(componentCacheKey, out result))
                {
                    return result;
                }
                object component = go.GetComponent(type);
                entry.componentCacheEntries.Insert(componentCacheKey, component);
                return component;
            }
            else
            {

                GOCacheEntry cacheEntry = new GOCacheEntry(go);
                object component = go.GetComponent(type);
                cacheEntry.componentCacheEntries.Insert(componentCacheKey, component);
                cacheEntries.Insert(go, cacheEntry);
                return component;

            }
        }
        
        public T GetCachedComponent<T>(GameObject go) where T : Component
        {
            object result = null;
            ComponentCacheKey componentCacheKey = new ComponentCacheKey(ComponentCacheKey.KeyType.GetComponent,typeof(T));
            if (cacheEntries.TryGetValue(go, out GOCacheEntry entry))
            {
                if (entry.componentCacheEntries.TryGetValue(componentCacheKey, out result))
                {
                    return (T)result;
                }
                T component = go.GetComponent<T>();
                entry.componentCacheEntries.Insert(componentCacheKey, component);
                return component;
            }
            else
            {

                GOCacheEntry cacheEntry = new GOCacheEntry(go);
                T component = go.GetComponent<T>();
                cacheEntry.componentCacheEntries.Insert(componentCacheKey, component);
                cacheEntries.Insert(go, cacheEntry);
                return component;
                
            }
        }
        public object GetCachedComponents(System.Type type, GameObject go)
        {
            object result = null;
            ComponentCacheKey componentCacheKey = new ComponentCacheKey(ComponentCacheKey.KeyType.GetComponents, type);
            if (cacheEntries.TryGetValue(go, out GOCacheEntry entry))
            {
                if (entry.componentCacheEntries.TryGetValue(componentCacheKey, out result))
                {
                    return result;
                }
                object component = go.GetComponents(type);
                entry.componentCacheEntries.Insert(componentCacheKey, component);
                return component;
            }
            else
            {

                GOCacheEntry cacheEntry = new GOCacheEntry(go);
                object component = go.GetComponents(type);
                cacheEntry.componentCacheEntries.Insert(componentCacheKey, component);
                cacheEntries.Insert(go, cacheEntry);
                return component;

            }
        }

        public T[] GetCachedComponents<T>(GameObject go) where T : Component
        {
            object result = null;
            ComponentCacheKey componentCacheKey = new ComponentCacheKey(ComponentCacheKey.KeyType.GetComponents, typeof(T));
            if (cacheEntries.TryGetValue(go, out GOCacheEntry entry))
            {
                if (entry.componentCacheEntries.TryGetValue(componentCacheKey, out result))
                {
                    return (T[])result;
                }
                T[] component = go.GetComponents<T>();
                entry.componentCacheEntries.Insert(componentCacheKey, component);
                return component;
            }
            else
            {

                GOCacheEntry cacheEntry = new GOCacheEntry(go);
                T[] component = go.GetComponents<T>();
                cacheEntry.componentCacheEntries.Insert(componentCacheKey, component);
                cacheEntries.Insert(go, cacheEntry);
                return component;

            }
        }
        public object GetCachedComponentInChildren(Type type, GameObject go)
        {
            object result = null;
            ComponentCacheKey componentCacheKey = new ComponentCacheKey(ComponentCacheKey.KeyType.GetComponentInChildren, type);
            if (cacheEntries.TryGetValue(go, out GOCacheEntry entry))
            {
                if (entry.componentCacheEntries.TryGetValue(componentCacheKey, out result))
                {
                    return result;
                }
                object component = go.GetComponentInChildren(type);
                entry.componentCacheEntries.Insert(componentCacheKey, component);
                return component;
            }
            else
            {

                GOCacheEntry cacheEntry = new GOCacheEntry(go);
                object component = go.GetComponentInChildren(type);
                cacheEntry.componentCacheEntries.Insert(componentCacheKey, component);
                cacheEntries.Insert(go, cacheEntry);
                return component;

            }
        }
        public T GetCachedComponentInChildren<T>(GameObject go) where T : Component
        {
            object result = null;
            ComponentCacheKey componentCacheKey = new ComponentCacheKey(ComponentCacheKey.KeyType.GetComponentInChildren, typeof(T));
            if (cacheEntries.TryGetValue(go, out GOCacheEntry entry))
            {
                if (entry.componentCacheEntries.TryGetValue(componentCacheKey, out result))
                {
                    return (T)result;
                }
                T component = go.GetComponentInChildren<T>();
                entry.componentCacheEntries.Insert(componentCacheKey, component);
                return component;
            }
            else
            {

                GOCacheEntry cacheEntry = new GOCacheEntry(go);
                T component = go.GetComponentInChildren<T>();
                cacheEntry.componentCacheEntries.Insert(componentCacheKey, component);
                cacheEntries.Insert(go, cacheEntry);
                return component;

            }
        }
        public object GetCachedComponentsInChildren(System.Type type ,GameObject go)
        {
            object result = null;
            ComponentCacheKey componentCacheKey = new ComponentCacheKey(ComponentCacheKey.KeyType.GetComponentsInChildren, type);
            if (cacheEntries.TryGetValue(go, out GOCacheEntry entry))
            {
                if (entry.componentCacheEntries.TryGetValue(componentCacheKey, out result))
                {
                    return result;
                }
                object components = go.GetComponentsInChildren(type);
                entry.componentCacheEntries.Insert(componentCacheKey, components);
                return components;
            }
            else
            {
                GOCacheEntry cacheEntry = new GOCacheEntry(go);
                object components = go.GetComponentsInChildren(type);
                cacheEntry.componentCacheEntries.Insert(componentCacheKey, components);
                cacheEntries.Insert(go, cacheEntry);
                return components;
            }
        }
        public T[] GetCachedComponentsInChildren<T>(GameObject go) where T : Component
        {
            object result = null;
            ComponentCacheKey componentCacheKey = new ComponentCacheKey(ComponentCacheKey.KeyType.GetComponentsInChildren, typeof(T));
            if (cacheEntries.TryGetValue(go, out GOCacheEntry entry))
            {
                if (entry.componentCacheEntries.TryGetValue(componentCacheKey, out result))
                {
                    return (T[])result;
                }
                T[] components = go.GetComponentsInChildren<T>();
                entry.componentCacheEntries.Insert(componentCacheKey, components);
                return components;
            }
            else
            {
                GOCacheEntry cacheEntry = new GOCacheEntry(go);
                T[] components = go.GetComponentsInChildren<T>();
                cacheEntry.componentCacheEntries.Insert(componentCacheKey, components);
                cacheEntries.Insert(go, cacheEntry);
                return components;
            }
        }

    }
}
