using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TestMenu : MonoBehaviour
{
    static class Test
    {
        [MenuItem("GameObject/测试选择")]
        static void Select()
        {
            Selection.Add(GameObject.Find("TestSelect"));
        }
    }
}
