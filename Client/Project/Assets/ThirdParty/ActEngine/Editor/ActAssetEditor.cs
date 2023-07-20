using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Reflection;
using System.IO;

namespace Act
{
    [CustomEditor(typeof(ActAsset))]
    public class ActAssetEditor : Editor
    {
        SerializedProperty lifeTime;

        ActAsset actData;

        List<Type> typeList;
        List<string> nameList;

        List<ActEditor> editors;

        Dictionary<Type, ActDisplayNameAttribute> nameMapping = new Dictionary<Type, ActDisplayNameAttribute>(); //节点对应的名称

        int selectNodeInstanceIndex; //选择已添加的节点实例

        int selectNodeIndex; //选择要添加的节点

        float speed = 1;

        private void OnEnable()
        {
            typeList = new List<Type>();
            nameList = new List<string>();
            editors = new List<ActEditor>();

            lifeTime = serializedObject.FindProperty("LifeTime");

            actData = target as ActAsset;

            Type baseType = typeof(ActBaseClip);
            Type eventBaseType = typeof(ActBaseEvent);

            foreach (var type in baseType.Assembly.GetTypes())
            {
                if (baseType.IsAssignableFrom(type) && !type.IsAbstract)
                {
                    var attr = type.GetCustomAttribute<ActDisplayNameAttribute>();
                    if (attr != null)
                    {
                        typeList.Add(type);
                        nameList.Add(attr.DisplayName);
                        nameMapping.Add(type, attr);
                    }
                }
            }

            for (int i = 0; i < actData.Clips.Count; i++)
            {
                editors.Add(ActEditor.CreateEditor(actData, actData.Clips[i]));
            }

            for (int i = 0; i < actData.Events.Count; i++)
            {
                actData.Events[i].CheckGUID();
            }

            EditorUtility.SetDirty(actData);
        }

        public override void OnInspectorGUI()
        {
            var data = target as ActAsset;

            if (Application.isPlaying)
            {
                using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    GUILayout.Label("预览");
                    EditorGUILayout.Separator();
                    using (new GUILayout.HorizontalScope())
                    {
                        if (GUILayout.Button("播放当前Act", GUILayout.Width(100)))
                        {
                            string path = AssetDatabase.GetAssetPath(data);
                            //Facade.Preview.PlayAct(path, speed);
                            Facade.Preview.PlayAct(data.name + ".asset", speed);
                            Facade.Internals.Timeline.Reset?.Invoke();
                        }
                        if (GUILayout.Button("停止当前Act", GUILayout.Width(100)))
                        {
                            Facade.Preview.StopAct?.Invoke();
                        }
                    }
                    speed = EditorGUILayout.FloatField("速度", speed);
                }
            }

            if (GUILayout.Button("打开时间轴", GUILayout.Width(100)))
            {
                ActUtility.OpenTimeline(actData);
            }

            EditorGUI.BeginChangeCheck();

            EditorGUI.BeginChangeCheck();
            actData.LifeTime = EditorGUILayout.FloatField("生命时间", actData.LifeTime);
            if (EditorGUI.EndChangeCheck())
            {
                actData.CheckLifeTime();
            }

            actData.Model = EditorGUILayout.TextField("绑定角色:", actData.Model);

            for (int i = 0; i < actData.Clips.Count; i++)
            {
                EditorGUILayout.Separator();
                using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    DrawClipEditor(i);
                }
            }

            EditorGUILayout.Separator();

            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField("添加新表现");

                selectNodeIndex = EditorGUILayout.Popup("请选择: ", selectNodeIndex, nameList.ToArray());

                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("添加", GUILayout.Width(80)))
                    {
                        AddNode(typeList[selectNodeIndex]);
                    }

                    if (GUILayout.Button("清空", GUILayout.Width(80)))
                    {
                        ClearNode();
                    }
                }
            }

            serializedObject.Update();

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

        void DrawClipEditor(int index)
        {
            ActBaseClip clip = actData.Clips[index];
            nameMapping.TryGetValue(clip.GetType(), out var attr);
            //using (new EditorGUILayout.HorizontalScope())
            //{
            //    GUILayout.Label(attr.Title, GUILayout.ExpandWidth(true));
            //}

            ActEditor nodeEditor = editors[index];

            using (new EditorGUILayout.HorizontalScope())
            {
                bool isActive = EditorGUILayout.BeginFoldoutHeaderGroup(nodeEditor.IsActive, $"{index}:{attr.DisplayName}", null, (pos) =>
                {
                    selectNodeInstanceIndex = index;
                    var menu = new GenericMenu();
                    menu.AddItem(new GUIContent("删除"), false, RemoveNode);
                    menu.AddItem(new GUIContent("Edit Script"), false, OpenScript);

                    menu.DropDown(pos);
                });
                if (nodeEditor.IsActive != isActive)
                {
                    nodeEditor.SetActive(isActive);
                }
                EditorGUILayout.EndFoldoutHeaderGroup();
            }

            if (nodeEditor.IsActive)
                nodeEditor.OnInspectorGUI();
        }

        void OpenScript()
        {
            var script = FindScriptFromClassName(actData.Clips[selectNodeInstanceIndex].GetType().Name);

            if (script != null)
                AssetDatabase.OpenAsset(script.GetInstanceID(), 0, 0);
        }

        static MonoScript FindScriptFromClassName(string className)
        {
            var scriptGUIDs = AssetDatabase.FindAssets($"t:script {className}");

            if (scriptGUIDs.Length == 0)
                return null;

            foreach (var scriptGUID in scriptGUIDs)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(scriptGUID);
                var script = AssetDatabase.LoadAssetAtPath<MonoScript>(assetPath);

                if (script != null && String.Equals(className, Path.GetFileNameWithoutExtension(assetPath), StringComparison.OrdinalIgnoreCase))
                    return script;
            }

            return null;
        }

        void AddNode(Type nodeType)
        {
            var newNode = Activator.CreateInstance(nodeType) as ActBaseClip;
            newNode.Asset = actData;
            actData.Clips.Add(newNode);

            editors.Add(ActEditor.CreateEditor(actData, newNode));
        }

        void RemoveNode()
        {
            RemoveNode(selectNodeInstanceIndex);
        }

        void RemoveNode(int index)
        {
            actData.Clips.RemoveAt(index);

            editors.RemoveAt(index);
        }

        void ClearNode()
        {
            actData.Clips.Clear();
            editors.Clear();
        }

    }
}

