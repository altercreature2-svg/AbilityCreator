using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AC.AssetManaging.AssetTypes
{
    public interface AssetBaseType
    {
        Dictionary<string, object> UntypedData();
        string GetName();
        
    }
}
