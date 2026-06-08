using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDK
{
    // to avoid the trash garbage collector
    public static class Registery
    {
        public static List<IRegisterable> registerables = new List<IRegisterable>();
        public static void Register(IRegisterable registerable)
        {
            registerables.Add(registerable);
        }
    }
}
