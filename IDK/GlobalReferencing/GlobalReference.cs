using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDK.GlobalReferencing
{
    public struct GlobalReference
    {
        public GlobalReference(int index)
        {
            this.index = index;
        }
        public int index;
        public object GetValue()
        {
            return GlobalReferenceManager.Instance.globalReferences[index];
        }
    }
}
