using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Act
{
    [CustomEditor(typeof(ActPreviewAsset))]
    public class ActPreviewAssetEditor : Editor
    {
        SerializedProperty heroList;
        SerializedProperty monsterList;
        SerializedProperty sceneList;

        private void OnEnable()
        {
            heroList = serializedObject.FindProperty("heroList");
            monsterList = serializedObject.FindProperty("monsterList");
            sceneList = serializedObject.FindProperty("sceneList");
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("预览数据配置");

            EditorGUI.BeginChangeCheck();
            serializedObject.Update();

            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.PropertyField(heroList, new GUIContent("英雄列表"), true);
                EditorGUILayout.PropertyField(monsterList, new GUIContent("怪物列表"), true);
                EditorGUILayout.PropertyField(sceneList, new GUIContent("场景列表"), true);
            }

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }

}

