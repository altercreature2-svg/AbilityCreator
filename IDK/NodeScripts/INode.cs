using IDK.Node_Related_Scripts.NodeRunning;
using Landfall.TABS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IDK.NodeScripts
{
    public interface INode
    {
        IEnumerator Execute(NodeEnv env);
        IEnumerator Cache(NodeEnv env);
    }
}