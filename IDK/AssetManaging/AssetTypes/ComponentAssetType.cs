using Sirenix.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AC.AssetManaging.AssetTypes
{
    public class ComponentAssetType : AssetType<Type>
    {

        public override Dictionary<string, Type> GetAllAssets()
        {
            Dictionary<string, Type> assets = new Dictionary<string, Type>();
            List<Type> allTypes = new List<Type>();
            Assembly[] assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < assemblies.Length; i++)
            {
                try
                {
                    allTypes.AddRange(assemblies[i].GetTypes());
                }
                catch (ReflectionTypeLoadException ex)
                {
                    allTypes.AddRange(ex.Types); // some may be null, filter below
                }
            }
            for (int i = 0; i < allTypes.Count; i++)
            {
                if (typeof(Component).IsAssignableFrom(allTypes[i]))
                {
                    string name = allTypes[i].GetCompilableNiceFullName();
                    while (assets.ContainsKey(name))
                    {
                        name += "+";
                    }
                    assets.Add(name, allTypes[i]);
                }
            }
            return assets;
        }
        public override string GetName()
        {
            return "component";
        }
    }
}
