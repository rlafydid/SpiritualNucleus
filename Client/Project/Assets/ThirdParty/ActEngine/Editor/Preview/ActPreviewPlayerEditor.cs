using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Act
{
    [CustomEditor(typeof(ActPreviewPlayer))]
    public class ActPreviewPlayerEditor : Editor
    {
        SerializedProperty heroList;
        SerializedProperty monsterList;
        SerializedProperty sceneList;

        SerializedProperty previewAsset;
        SerializedProperty hitAct;

        private void OnEnable()
        {
            previewAsset = serializedObject.FindProperty("previewAsset");
            hitAct = serializedObject.FindProperty("hitAct");

            var player = target as ActPreviewPlayer;

            if (player.previewAsset != null)
                EditorUtility.SetDirty(player.previewAsset);
        }

        bool isActivePreviewDatas = false;
        public override void OnInspectorGUI()
        {
            var player = target as ActPreviewPlayer;

            EditorGUILayout.LabelField("预览");

            EditorGUILayout.Space(10);

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(previewAsset, new GUIContent("预览数据源:"));
            if (EditorGUI.EndChangeCheck())
            {
                if (player.previewAsset != null)
                    EditorUtility.SetDirty(player.previewAsset);
                serializedObject.ApplyModifiedProperties();
            }


            if (player.previewAsset != null)
            {
                EditorGUI.BeginChangeCheck();

                player.previewHeroIndex = EditorGUILayout.Popup("选择预览英雄: ", GetSafeIndex(player.previewAsset.heroList, player.previewHeroIndex), player.previewAsset.heroList.Select(d => d.Name).ToArray());

                player.previewMonsterIndex = EditorGUILayout.Popup("选择预览怪物: ", GetSafeIndex(player.previewAsset.monsterList, player.previewMonsterIndex), player.previewAsset.monsterList.Select(d => d.Name).ToArray());



                var selectMainIndex = player.mainCharactorIsHero ? 0 : 1;
                selectMainIndex = EditorGUILayout.Popup("主角: ", selectMainIndex, new string[] { "英雄", "怪物" });
                player.mainCharactorIsHero = selectMainIndex == 0;
                if (EditorGUI.EndChangeCheck() && Application.isPlaying)
                {
                    player.Reload();
                }

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(hitAct, new GUIContent("靶子受击Act:"));
                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();
                }

                EditorGUI.BeginChangeCheck();
                player.previewSceneIndex = EditorGUILayout.Popup("选择预览场景: ", GetSafeIndex(player.previewAsset.sceneList, player.previewSceneIndex), player.previewAsset.sceneList.Select(d => d.Name).ToArray());
                if (EditorGUI.EndChangeCheck())
                {
                    player.ChangeScene();
                }

                using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    EditorGUILayout.LabelField("当前选择的场景数据:               (自动同步到预览数据配置)");

                    if (player.previewAsset.sceneList.Count > 0)
                    {
                        PreviewSceneData data = player.previewAsset.sceneList[GetSafeIndex(player.previewAsset.sceneList, player.previewSceneIndex)];
                        data.HeroPosition = EditorGUILayout.Vector3Field("英雄出生位置:", data.HeroPosition);
                        data.MonsterPosition = EditorGUILayout.Vector3Field("怪物出生位置:", data.MonsterPosition);
                    }
                }

                //EditorGUILayout.Separator();

                //isActivePreviewDatas = EditorGUILayout.BeginFoldoutHeaderGroup(isActivePreviewDatas, $"预览数据配置", null);
                //EditorGUILayout.EndFoldoutHeaderGroup();

                //if (isActivePreviewDatas)
                //{
                //    heroList = serializedObject.FindProperty("previewAsset").FindProperty("heroList");
                //    monsterList = serializedObject.FindProperty("monsterList");
                //    sceneList = serializedObject.FindProperty("sceneList");

                //    using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                //    {
                //        EditorGUILayout.PropertyField(heroList, new GUIContent("英雄列表"), true);
                //        EditorGUILayout.PropertyField(monsterList, new GUIContent("怪物列表"), true);
                //        EditorGUILayout.PropertyField(sceneList, new GUIContent("场景列表"), true);
                //    }
                //}
            }
            else
            {
                EditorGUILayout.LabelField("请指定预览数据文件");
            }

            serializedObject.Update();
            serializedObject.ApplyModifiedProperties();
        }

        void DrawPopup()
        {

        }

        int GetSafeIndex(IList list, int index)
        {
            if (list.Count == 0)
                return 0;
            return Mathf.Min(list.Count - 1, index);
        }
    }


}

