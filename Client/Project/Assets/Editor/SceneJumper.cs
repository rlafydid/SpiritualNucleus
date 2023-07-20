using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class SceneJumper
{
    [MenuItem("Tools/场景跳转/主场景")]
    public static void OpenMainScene()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/Main.unity");
    }
}
