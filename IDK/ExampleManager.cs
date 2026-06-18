using AC.Node_Related_Scripts.SavingStuff;
using IDK.AbilityHandling;
using IDK.AbilityHandling.Saving;
using IDK.Node_Related_Scripts;
using System.IO;
using System.Reflection;

namespace AC
{
    public static class ExampleManager
    {
        public static void WriteAll() // DO IT YOU SUCK FUCK
        {}
        public static void WriteExample(string json)
        {
            FileManager.WriteAbility(SaveableHelper.Load<VirtualNodeScene>(json).abilityName, json); // idgaf any more
        }
        public static void TurnIntoExample(string json, string name)
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(json);
            string base64 = System.Convert.ToBase64String(bytes);
            FileManager.WriteAbility(name, base64);
        }
        public static void TurnAllIntoExample()
        {
            for (int i = 0; i < AbilityManager.Instance.Abilities.Count; i++)
            {
                TurnIntoExample(SaveableHelper.Save(AbilityManager.Instance.Abilities[i].nodeScene), AbilityManager.Instance.Abilities[i].abilityName);
            }
        }
    }
}