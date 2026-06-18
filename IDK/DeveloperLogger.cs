using System;
using UnityEngine;

public class DeveloperLogger
{
    public static bool devoleperMode = true;
    public static void Log(object message, bool forceTrue = false)
    {
        if ((!devoleperMode) | forceTrue)
            return;
        Console.WriteLine("[Message : Abiltiy Creator] " + message);
    }
    public static void Log(object message, ConsoleColor consoleColor, bool forceTrue = false)
    {
        if ((!devoleperMode) | !forceTrue)
            return;
        Console.ForegroundColor = consoleColor;
        Console.WriteLine("[Message : Abiltiy Creator] " + message);
        Console.ResetColor();
    }
    public static void LogError(object message, bool forceTrue = false)
    {
        if ((!devoleperMode) | !forceTrue)
            return;
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("[Error : Abiltiy Creator] " + message);
        Console.ResetColor();
    }
}

