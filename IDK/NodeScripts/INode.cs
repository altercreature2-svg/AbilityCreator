using AC.Node_Related_Scripts.NodeRunning;
using AC.Node_Related_Scripts.NodeRunning.Instructions.Courtines;
using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AC.NodeScripts
{
    public interface INode
    {
        IEnumerator<CoroutineReturn> Execute(NodeEnv env);
        IEnumerator<CoroutineReturn> Cache(NodeEnv env);
    }
}