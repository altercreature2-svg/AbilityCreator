using AC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDK.Help
{
    public static class GlobalCaching
    {
        private static Dictionary<string, VirtualNodeScene> nodeScenes;
        public static void CacheScene(VirtualNodeScene scene)
        {
            nodeScenes.Add(scene.abilityName, scene);
        }
        public static VirtualNodeScene GetVirtualNodeScene(string sceneName)
        {
            if (nodeScenes.ContainsKey(sceneName)) return nodeScenes[sceneName];
            return null; // node scene not cached!
        }
        public static VirtualNodeScene[] GetAllVirtualNodeScene()
        {
            return nodeScenes.Values.ToArray();
            
        }
        public static void SetVirtualNodeScene(VirtualNodeScene virtualNodeScene)
        {
            if (nodeScenes.ContainsKey(virtualNodeScene.abilityName)) { nodeScenes[virtualNodeScene.abilityName] = virtualNodeScene; return; }
            nodeScenes.Add(virtualNodeScene.abilityName, virtualNodeScene);
        }
    }
}
