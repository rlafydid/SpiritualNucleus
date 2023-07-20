using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Act
{
    public partial class ActEditor
    {
        public ActAsset owner { protected get; set; }

        public object target { protected get; set; }

        public SerializedObject serializedObject { protected get; set; }

        protected SerializedProperty serializedProperty;

        bool _isActive = false;
        public bool IsActive { get => _isActive; }

        FieldInfo[] fields = { };

        GameObject model;
        protected GameObject getModel
        {
            get
            {
                if (string.IsNullOrEmpty(owner.Model))
                    return null;

                if (model == null || model.name != owner.Model)
                    model = ActUtility.GetAsset<GameObject>(owner.Model);
                return model;
            }
        }

        public virtual void OnEnable()
        {
            Refresh();
        }

        public virtual void OnDisable()
        {

        }

        public void Refresh()
        {
            bool isClipType = typeof(ActBaseClip).IsAssignableFrom(target.GetType());

            if (isClipType)
            {
                if (owner.Clips == null || owner.Clips.Count == 0)
                    return;
            }
            else if (owner.Events == null || owner.Events.Count == 0)
                return;

            SerializedProperty arrayProperty = serializedObject.FindProperty(isClipType ? "Clips" : "Events");
            if (isClipType)
            {
                serializedProperty = arrayProperty.GetArrayElementAtIndex(owner.Clips.FindIndex(d => d == target as ActBaseClip));
            }
            else
            {
                serializedProperty = arrayProperty.GetArrayElementAtIndex(owner.Events.FindIndex(d => d == target as ActBaseEvent));
            }
            fields = target.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).Where(d => d.GetCustomAttribute<NonSerializedAttribute>() == null).ToArray();
        }

        public void SetActive(bool isActive)
        {
            _isActive = isActive;
        }

        public virtual void OnInspectorGUI()
        {
            if (serializedProperty == null)
            {
                Refresh();
                return;
            }

            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            DrawClass(target, serializedProperty);

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

        protected void DrawClass(object instance, SerializedProperty parentProperty, Type specifiedDrawType = null)
        {
            if (instance == null)
                return;

            var type = instance.GetType();

            RecursionDrawClass(type, instance, parentProperty, specifiedDrawType);
        }

        protected void RecursionDrawClass(Type type, object instance, SerializedProperty parentProperty, Type specifiedDrawType)
        {
            if (type == null || instance == null || type == typeof(ScriptableObject))
                return;

            RecursionDrawClass(type.BaseType, instance, parentProperty, specifiedDrawType);

            if (specifiedDrawType != null && specifiedDrawType != type)
                return;

            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly).Where(d => d.GetCustomAttribute<NonSerializedAttribute>() == null).ToArray();
            foreach (var field in fields)
            {
                if (field.FieldType.IsClass && field.FieldType != typeof(string))
                {
                    DrawClass(field.GetValue(instance), parentProperty.FindPropertyRelative(field.Name));
                }
                else
                    DrawProperty(field, parentProperty, instance);
            }
        }

        Dictionary<FieldInfo, UnityEngine.Object> fieldDependentMapping = new Dictionary<FieldInfo, UnityEngine.Object>();
        void DrawProperty(FieldInfo field, SerializedProperty parentProperty, object instance)
        {
            if (parentProperty == null || field == null)
                return;

            SerializedProperty property = parentProperty.FindPropertyRelative(field.Name);
            if (property == null)
                return;

            ActDisplayNameAttribute displayAttr = field.GetCustomAttribute<ActDisplayNameAttribute>();
            string name = displayAttr != null ? displayAttr.DisplayName : field.Name;

            try
            {
                EditorGUILayout.BeginHorizontal();
                if (!string.IsNullOrEmpty(name))
                {

                    GUIStyle style = new GUIStyle();
                    style.normal = new GUIStyleState();
                    string tips = null;
                    if (field.GetCustomAttribute<SynchroDataAttribute>() != null)
                    {
                        style.normal.textColor = new Color(102, 220, 255, 255) / 255;
                        tips = "要同步到技能蓝图的字段";
                    }
                    else
                        style.normal.textColor = Color.white;

                    GUILayout.Label(new GUIContent(name, null, tips), style, GUILayout.Width(110));

                    if (!(displayAttr != null && displayAttr.ShowNameOnly))
                        EditorGUILayout.PropertyField(property, new GUIContent(""));

                }

                var dependentAttr = field.GetCustomAttribute<DependentObjectAttribute>();
                if (dependentAttr != null && field.FieldType == typeof(string))
                {
                    string resName = (string)field.GetValue(instance);
                    UnityEngine.Object prefab = null;
                    if (!string.IsNullOrEmpty(resName) && (!fieldDependentMapping.TryGetValue(field, out prefab) || prefab.name != resName))
                    {
                        prefab = Act.ActUtility.GetAsset<GameObject>(resName);
                        if (prefab != null)
                            fieldDependentMapping.Add(field, prefab);
                    }
                    EditorGUI.BeginChangeCheck();
                    prefab = EditorGUILayout.ObjectField(prefab, dependentAttr.DependentObjectType, false);
                    if (EditorGUI.EndChangeCheck())
                    {
                        fieldDependentMapping[field] = prefab;
                        field.SetValue(instance, prefab != null ? prefab.name : null);
                    }
                }
                EditorGUILayout.EndHorizontal();

            }
            catch (Exception e)
            {
                Debug.LogError($"字段 {field.Name} 出错: {e.ToString()}");
            }
        }
    }



    public partial class ActEditor
    {
        static Dictionary<Type, Type> editorMapping = new Dictionary<Type, Type>();

        /// <summary>
        /// 创建对应的Editor
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static ActEditor CreateEditor(ActAsset owner, object target)
        {
            Type editorType = GetEditorType(target.GetType());
            ActEditor editor = editorType != null ? Activator.CreateInstance(editorType) as ActEditor : new ActEditor();
            editor.owner = owner;
            editor.target = target;
            editor.serializedObject = new SerializedObject(owner);

            editor._isActive = true;
            editor.OnEnable();
            return editor;
        }

        static Type GetEditorType(Type scriptType)
        {
            CheckEditors(scriptType);
            if (editorMapping.TryGetValue(scriptType, out var value))
            {
                return value;
            }
            return null;
        }

        static void CheckEditors(Type scriptType)
        {
            if (!editorMapping.TryGetValue(scriptType, out var value))
            {
                InitActEditors();
            }
        }

        static void InitActEditors()
        {
            editorMapping.Clear();
            foreach (var type in typeof(ActEditor).Assembly.GetTypes())
            {
                var attr = type.GetCustomAttribute<ActCustomEditorAttribute>(false);
                if (attr != null)
                {
                    if (editorMapping.ContainsKey(attr.Type))
                        Debug.LogError($"{type.Name}上在ActCustomEditor指定的类型{attr.Type.Name}重复, 请处理");
                    else
                        editorMapping.Add(attr.Type, type);
                }
            }
        }
    }
}

