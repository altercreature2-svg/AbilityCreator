using AC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDK.Node_Related_Scripts
{
    public class NodeIDHelper
    {
        private static int _globalCounter = 0;
        private static Dictionary<NodeComponent, int> ids = new Dictionary<NodeComponent, int>();
        public static int GetID(NodeComponent node)
        {
            if (ids.TryGetValue(node, out var id)) return id;
            _globalCounter++;
            ids?.Add(node, _globalCounter);
            return _globalCounter;
        }
        public static int DefaultUnassignedID
        {
            get
            {
                return -1;
            }
        }
    }
}
