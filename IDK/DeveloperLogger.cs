using System;
using UnityEngine;

public class DeveloperLogger
{
    public static bool devoleperMode = true;
    public static void Log(object message, bool forceTrue = false)
    {
        if (!devoleperMode)
            return;
        else if (!forceTrue)
            return;
        Console.WriteLine("[Message : Abiltiy Creator] " + message);
    }
}

