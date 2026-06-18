using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AC
{
    public abstract class IRegisterable
    {
        public IRegisterable()
        {
            Registery.Register(this);   
        }
    }
}
