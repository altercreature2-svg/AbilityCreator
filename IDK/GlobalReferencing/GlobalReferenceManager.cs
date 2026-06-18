using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDK.GlobalReferencing
{
    public class GlobalReferenceManager
    {
        private static GlobalReferenceManager m_instance; 
        public static GlobalReferenceManager Instance { get { if (m_instance == null) m_instance = new GlobalReferenceManager(); return m_instance; } }
        public List<object> globalReferences = new List<object>();
        public GlobalReference GetReference(object obj)
        {
            GlobalReference globalReference = new GlobalReference(globalReferences.Count);
            globalReferences.Add(obj);
            return globalReference;
        }
    }
}
