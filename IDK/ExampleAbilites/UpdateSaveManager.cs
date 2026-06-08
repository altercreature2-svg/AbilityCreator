using System.Collections;
using System.IO;
using UnityEngine;

namespace IDK.ExampleAbilites
{
    public class UpdateSaveManager : MonoBehaviour
    {
        public static void Handle()
        {
            if (!File.Exists(AbilityCreator.path + "/SAVE"))
            {
                FileStream fileStream = File.Create(AbilityCreator.path + "/SAVE");
                fileStream.Close();
                File.WriteAllText(AbilityCreator.path + "/SAVE", "2.6.0");
                ExampleManager.WriteAll();
                return;
            }
            else
            {
                if (File.ReadAllText(AbilityCreator.path + "/SAVE").Contains("2.6.0"))
                {
                    return;
                }
                else
                {
                    ExampleManager.WriteAll();
                    File.WriteAllText(AbilityCreator.path + "/SAVE", "2.6.0");
                }
            }
        }
    }
}