using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AC.Node_Related_Scripts.SavingStuff
{
    public class Serialize
    {
        public static string SaveJson<T>(T obj)
        {
            var settings = new Newtonsoft.Json.JsonSerializerSettings()
            {
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore,
            };

            string json = Newtonsoft.Json.JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented, settings);
            return json;
        }
        public static T LoadJson<T>(string json)
        {
            var settings = new Newtonsoft.Json.JsonSerializerSettings()
            {
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore,
            };

            return (T)Newtonsoft.Json.JsonConvert.DeserializeObject(json, settings);
            
        }
    }
}
