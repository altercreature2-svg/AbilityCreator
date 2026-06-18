using AC.Node_Related_Scripts.NodeRunning;
using AC.Node_Related_Scripts.NodeRunning.Instructions.Courtines;
using Landfall.TABS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace AC.NodeScripts
{
    public class ToggleProps : IBehaviorNode
    {
        int min, max;
        public IEnumerator<CoroutineReturn> Execute(NodeEnv env)
        {
            var unitsEnum = env.GetValues(NodeBlueprint.ConnectionClass.ReciveUnit);
            foreach (var item in unitsEnum)
            {
                if (!(item.value is Unit u))
                    continue;
                PropItem[] props = env.cacheSystem.GetCachedComponentsInChildren<PropItem>(u.gameObject);
                for (int i = Mathf.Min(0, min); i < Mathf.Max(props.Length, max); i++)
                {
                    bool isVisible = (bool)((CharacterItem)props[i]).GetField("m_isVisible");
                    props[i].SetVisibility(!isVisible);
                }
            }
            yield return new CoroutineReturn(CoroutineReturn.CourtineType.ContinueBranch);
        }
        public IEnumerator<CoroutineReturn> Cache(NodeEnv env)
        {
            min = env.GetField(0).QuickParseInt();
            max = env.GetField(1).QuickParseInt();
            return null;
        }
    }
}