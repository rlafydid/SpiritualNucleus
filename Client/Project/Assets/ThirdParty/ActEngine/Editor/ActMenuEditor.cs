using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Act
{

    public class ActMenuEditor
    {
        //[MenuItem("Assets/创建Act")]
        public static void Create()
        {
            ActAsset data = ScriptableObject.CreateInstance<ActAsset>();
            ProjectWindowUtil.CreateAsset(data, "Act.asset");
        }

        [MenuItem("Tools/场景跳转/ACT预览场景")]
        public static void OpenActScene()
        {
            EditorSceneManager.OpenScene("Assets/ArtRes/ActPreview/ActPreview.unity");
        }

        [MenuItem("Tools/Act编辑器/选中Act文件夹 _F6")]
        public static void SelectActDirectory()
        {
            Object obj = UnityEditor.AssetDatabase.LoadAssetAtPath<Object>("Assets/Res/Behavior/Act");
            Selection.activeObject = obj;
        }
    }
}

