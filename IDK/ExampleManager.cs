using System.IO;
using System.Reflection;

namespace IDK
{
    public static class ExampleManager
    {
        public static void WriteAll()
        {
            string[] files = Assembly.GetExecutingAssembly().GetManifestResourceNames();
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].EndsWith("abilityEncoded"))
                {

                    using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(files[i]))
                    {
                        StreamReader streamWriter = new StreamReader(stream);
                        string encodedJson = streamWriter.ReadToEnd();
                        string json = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(encodedJson));
                        LegacySavedNodeScene savedNodeScene = AbilityCreator.DeserializeAbility(json);
                        if (Directory.Exists(AbilityCreator.abilitespath + "/" + AbilityCreator.CleanNodeName(savedNodeScene.sceneName)))
                            continue;
                        Directory.CreateDirectory(AbilityCreator.abilitespath + "/" + AbilityCreator.CleanNodeName(savedNodeScene.sceneName));
                        File.WriteAllText(AbilityCreator.GetPath(savedNodeScene), json);
                    }
                }
            }
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].EndsWith("png"))
                {
                    using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(files[i]))
                    {
                        string path = AbilityCreator.abilitespath + "/size=8Examplesize=13#00AAFFCat Lives/icon.png";
                        if (File.Exists(path))
                            continue;
                        using (FileStream fileStream = new FileStream(path, FileMode.CreateNew, FileAccess.Write))
                        {
                            stream.CopyTo(fileStream);
                        }
                    }
                }
            }

        }
        public static void WriteExample(string json)
        {

            NodeManager.WriteAbility(AbilityCreator.DeserializeAbility(json), json);
        }
        public static void TurnIntoExample(string json, string name)
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(json);
            string base64 = System.Convert.ToBase64String(bytes);
            File.WriteAllText(AbilityCreator.abilitespath + "/" + AbilityCreator.CleanNodeName(name) + ".abilityEncoded", base64);
        }
        public static void TurnAllIntoExample()
        {
            for (int i = 0; i < AbilityCreator.nodeScenes.Count; i++)
            {
                TurnIntoExample(AbilityCreator.nodeScenes[i].Jsonify(), AbilityCreator.nodeScenes[i].sceneName);
            }
        }
    }
}