using IDK.Node_Related_Scripts;
using System.Collections;
using System.IO;
using UnityEngine;

namespace AC.ExampleAbilites
{
    public class UpdateSaveManager : MonoBehaviour
    {
        public const string Version = "3.0.0";
        public static void Handle()
        {
            if (!File.Exists(FilePaths.AbilityCreatorPath + "/SAVE"))
            {
                FileStream fileStream = File.Create(FilePaths.AbilityCreatorPath + "/SAVE");
                fileStream.Close();
                File.WriteAllText(FilePaths.AbilityCreatorPath + "/SAVE", Version);
                ExampleManager.WriteAll();
                return;
            }
            else
            {
                if (File.ReadAllText(FilePaths.AbilityCreatorPath + "/SAVE").Contains(Version))
                {
                    return;
                }
                else
                {
                    ExampleManager.WriteAll();
                    File.WriteAllText(FilePaths.AbilityCreatorPath + "/SAVE", Version);
                }
            }
        }
    }
}