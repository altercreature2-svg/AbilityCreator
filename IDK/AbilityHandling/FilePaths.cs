using AC;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TFBGames;
using static Mono.Security.X509.X520;

namespace IDK.Node_Related_Scripts
{
    public class FilePaths
    {
        public const string AbilityExtension = ".abilityx";
        public const string LegacyAbilityExtension = ".ability";
        public static string AbilityCreatorPath
        {
            get
            {
                return Path.Combine(GamePaths.DataPath, "/Abilty Creator");
            }
        }
        public static string AbilitesPath
        {
            get
            {
                return Path.Combine(AbilityCreatorPath, "/Abilites");
            }
        }
        public static string[] AbilityDirs
        {
            get
            {
                return Directory.GetDirectories(AbilitesPath);
            }
        }
        public static string[] AbilityFiles
        {
            get
            {
                return AbilityDirs.Select(n => Directory.GetFiles(n).FirstOrDefault(b => Path.GetExtension(n) == AbilityExtension)).ToArray();
            }
        }
        public static string[] AbilityNames
        {
            get
            {
                return AbilityFiles.Select(n => Path.GetFileNameWithoutExtension(n)).ToArray();
            }
        }
        public static string MakeFilePathSafe(string path)
        {
            return path.Replace("<", "").Replace(">", "").Replace("/", "").Replace("\\", "").Replace(":", "").Replace("*", "").Replace("?", "").Replace("\"", "").Replace("|", "");
        }
    }
}
