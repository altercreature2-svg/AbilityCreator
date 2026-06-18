using AC;
using AC.Node_Related_Scripts.SavingStuff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDK.AbilityHandling.Saving
{
    public class SaveableHelper
    {
        public static T Load<T>(string json) where T : ISaveable, new()
        {
            SaveableObject saveableObject = Serialize.LoadJson<SaveableObject>(json);
            T saveable = new T();
            saveable.Load(saveableObject);
            return saveable;
        }
        public static string Save<T>(T obj) where T : ISaveable, new()
        {
            string json = Serialize.SaveJson(obj.Save());
            return json;
        }
    }
}
