using Backtrace.Unity.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDK.AssetManaging.AssetTypes
{
    public abstract class AssetType<T> : AssetBaseType
    {
        public virtual string GetName() => typeof(T).Name;
        public abstract Dictionary<string, T> GetAllAssets();
        public Dictionary<string, object> UntypedData()
        {
            Dictionary<string, T> data = GetAllAssets();
            Dictionary<string, object> untypedData = new Dictionary<string, object>();
            foreach (KeyValuePair<string, T> pair in data)
            {
                untypedData.Add(pair.Key, pair.Value);
            }
            return untypedData;
        }
    }
}