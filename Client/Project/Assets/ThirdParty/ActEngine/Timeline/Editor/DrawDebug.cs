using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DrawDebug
{
    static bool isClose = false;
    public static void Draw(Rect rect, string content)
    {
        Draw(rect, content, Color.white);
    }

    public static void Draw(Rect rect, string content, Color col, bool isDrawRect)
    {
        Draw(rect, content, col, 0.3f, isDrawRect);
    }

    public static void Draw(Rect rect, string content, Color col, float a = 0.3f, bool isDrawRect = false)
    {
        if (isClose)
            return;

        string str = isDrawRect ? content + " r: " + rect.ToString() : content;
        col.a = a;
        EditorGUI.DrawRect(rect, col);
        EditorGUI.LabelField(rect, str);
    }
}
