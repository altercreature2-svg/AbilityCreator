using System.Collections;
using System.IO;
using UnityEngine;

namespace IDK.ExampleAbilites
{
    public class UpdateSaveManager : MonoBehaviour
    {
        public static void Handle()
        {
            if (!File.Exists(Main.path + "/SAVE"))
            {
                FileStream fileStream = File.Create(Main.path + "/SAVE");
                fileStream.Close();
                File.WriteAllText(Main.path + "/SAVE", "2.6.0");
                ExampleManager.WriteAll();
                return;
            }
            else
            {
                if (File.ReadAllText(Main.path + "/SAVE").Contains("2.6.0"))
                {
                    return;
                }
                else
                {
                    ExampleManager.WriteAll();
                    File.WriteAllText(Main.path + "/SAVE", "2.6.0");
                }
            }
        }
    }
}