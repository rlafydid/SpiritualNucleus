using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AssetReferenceTools : EditorWindow
{
    private List<Object> dependencies = new List<Object>();
    [MenuItem("Assets/查找资源引用", false, 33)]
    static void FindSelectAssetReference()
    {
        Object obj = Selection.activeObject;
        string[] deps = AssetDatabase.GetDependencies(AssetDatabase.GetAssetPath(obj));
        Init(deps);
    }

    private void SetDeps(string[] deps)
    {
        dependencies.Clear();
        foreach (var dep in deps)
        {
            if (dep.LastIndexOf(".cs") == -1)
            {
                dependencies.Add(AssetDatabase.LoadAssetAtPath(dep, typeof(Object)));
            }
        }
    }

    private static void Init(string[] deps)
    {
        AssetReferenceTools window = (AssetReferenceTools)EditorWindow.GetWindow(typeof(AssetReferenceTools));
        window.Show();
        window.SetDeps(deps);
    }



    void OnGUI()
    {
        EditorGUILayout.LabelField("资源引用数量:" + (dependencies.Count - 1));
        foreach (var depObj in dependencies)
        {
            EditorGUILayout.ObjectField(depObj.name, depObj, typeof(Object), false);
        }
    }




}