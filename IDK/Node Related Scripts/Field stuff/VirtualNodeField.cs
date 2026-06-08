using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDK.Node_Related_Scripts.Field_stuff
{
    
    public class VirtualNodeField : IRegisterable
    {
        public VirtualNodeField(int id, string value, ExtraAttribute[] extraAttributes = null)
        {
            this.fieldID = id;
            this.value = value;
            this.extras = extraAttributes != null ? extraAttributes : new ExtraAttribute[0];
        } 
        public int fieldID;
        public string value;
        public ExtraAttribute[] extras;
    }
}
