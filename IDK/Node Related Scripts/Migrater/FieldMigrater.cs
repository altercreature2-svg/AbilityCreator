using AC.Node_Related_Scripts.Field_stuff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AC.Node_Related_Scripts.Migrater
{
    public class FieldMigrater
    {
        // we can use the virtual type for saving aswell because it can be serialized 
        public static VirtualNodeField GetNewField(string fieldValue, int n)
        {
            VirtualNodeField virtualNodeField = new VirtualNodeField(n, fieldValue);
            return virtualNodeField;
        }
    }
}
