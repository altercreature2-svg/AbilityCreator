using AC;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace IDK.Node_Related_Scripts
{
    public class FileManager
    {
        public static void WriteAbility(string name, string content)
        {
            string directory = GetOrCreateDirectory(name);
            File.WriteAllText(name + FilePaths.AbilityExtension, content);
        }
        public static string ReadAbility(string name)
        {
            string dir = GetAbilityDirectory(name);
            string file = GetAbilityFile(name);
            if (!Directory.Exists(dir))
                return default;
            if (!File.Exists(file))
                return default;
            return File.ReadAllText(file);
        }
        private static string GetOrCreateDirectory(string name)
        {
            string path = Path.Combine(FilePaths.AbilitesPath, "/" + name);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }
        private static string GetAbilityDirectory(string name)
        {
            return Path.Combine(FilePaths.AbilitesPath, "/" + name);
        }
        private static string GetAbilityFile(string name)
        {
            return Path.Combine(GetAbilityDirectory(name), "/" + name + FilePaths.AbilityExtension);
        }
        public static bool SearchForSpirte(string name, out Sprite sprite)
        {
            string abilityDir = GetAbilityDirectory(name);
            string abilityFile = GetAbilityFile(name);
            string[] allFiles = Directory.GetFiles(abilityDir);
            foreach (var file in allFiles)
            {
                if (file == abilityFile)
                    continue;
                switch (Path.GetExtension(file) )
                {
                    case ".png":
                        sprite = LoadSprite(file);
                        return true;
                    case ".bmp":
                        sprite = LoadSprite(file);
                        return true;
                    default:
                        break;
                }
            }
            sprite = null;
            return false;
        } 
        private static Sprite LoadSprite(string path)
        {
            Texture2D texture = new Texture2D(0, 0);
            texture.LoadImage(File.ReadAllBytes(path));
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            return sprite;
        } 
    }
}
