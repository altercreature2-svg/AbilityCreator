using AC.Node_Related_Scripts.NodeRunning;
using AC.Node_Related_Scripts.NodeRunning.Instructions.Courtines;
using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using TFBGames;
using UnityEngine;

namespace AC.NodeScripts
{
    public class PlaySoundNode : IBehaviorNode
    {
        float volume;
        float pitch;
        public IEnumerator<CoroutineReturn> Execute(NodeEnv env)
        {
            var gameObjects = env.GetValues(NodeBlueprint.ConnectionClass.ReciveGameObject);
            foreach (var item in gameObjects)
            {
                if (!(item.value is GameObject go))
                    continue;
                AudioPathData path = null;
                bool validated = AudioPathData.ValidateAndAssignPathData(env.GetField(0).Replace("&", "/"), ref path);
                
                if (validated)
                {
                    AudioPlayer audioPlayer = ServiceLocator.GetService<SoundPlayer>().PlaySoundEffectNonAlloc(path, 
                        volume, 
                        go.transform.position, 
                        SoundEffectVariations.MaterialType.Default, 
                        go.transform, 
                        pitch);
                }
            }
            yield return new CoroutineReturn(CoroutineReturn.CourtineType.ContinueBranch);
        }
        public IEnumerator<CoroutineReturn> Cache(NodeEnv env)
        {
            volume = env.GetField(1).QuickParse();
            if (volume > 10)
                volume /= 100;  // cause of kylegaz
            pitch = env.GetField(2).QuickParse();
            return null;
        }
    }
}