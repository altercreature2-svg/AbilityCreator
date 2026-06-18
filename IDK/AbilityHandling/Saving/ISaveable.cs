using AC.NodeScripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AC.Node_Related_Scripts.SavingStuff
{
    [Serializable]
    public class SaveableField
    {
        public string fieldName;
        public string fieldValue;
    }
    [Serializable]
    public class SaveableObject
    {
        public string typeIdentfier;
        public SaveableField[] fields;
        public string GetSavedField(string fieldName)
        {
            return fields.FirstOrDefault(n => n.fieldName == fieldName).fieldValue;
        }
        public string[] GetSavedFields(string fieldName)
        {
            return fields.Where(n => n.fieldName.Contains(fieldName)).Select(n => n.fieldValue).ToArray();
        }
    }
    public interface ISaveable
    {
        SaveableObject Save();
        void Load(SaveableObject saveableObject);
    }
}
