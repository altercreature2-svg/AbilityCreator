using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDK
{
    public abstract class IRegisterable
    {
        public IRegisterable()
        {
            Registery.Register(this);   
        }
    }
}
