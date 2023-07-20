using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Logger
{
    public static void Log(object str)
    {
        Debug.Log(str);
    }

    public static void Error(string str)
    {
        Debug.LogError(str);
    }
}
