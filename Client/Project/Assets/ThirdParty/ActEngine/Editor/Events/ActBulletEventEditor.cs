using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Act
{
    [ActCustomEditor(typeof(ActBulletEvent))]
    public class ActBulletEventEditor : ActBaseEventEditor
    {
        List<string> bulletNames = new List<string>();
        List<Type> bulletTypes = new List<Type>();
        public override void OnEnable()
        {
            base.OnEnable();
            bulletTypes = new List<Type>();
            bulletNames = new List<string>();

            Debug.Log("Enable ActBulletEvent");

            var baseType = typeof(ActBaseBulletData);
            foreach (var item in typeof(ActBulletEvent).Assembly.GetTypes())
            {
                if (item.GetType() != baseType && item.IsSubclassOf(baseType))
                {
                    var attr = item.GetCustomAttribute<ActDisplayNameAttribute>(false);
                    bulletTypes.Add(item);
                    bulletNames.Add(attr != null ? attr.DisplayName : item.Name);
                }
            }

        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            DrawClass(target, serializedProperty, typeof(ActBaseEvent));
            var curBulletData = (target as ActBulletEvent).BulletData;
            int lastSelectIndex = curBulletData != null ? bulletTypes.FindIndex(d => d == curBulletData.GetType()) : -1;
            int selectIndex = EditorGUILayout.Popup("弹道类型", lastSelectIndex, bulletNames.ToArray());
            if (lastSelectIndex != selectIndex)
            {
                (target as ActBulletEvent).BulletData = Activator.CreateInstance(bulletTypes[selectIndex]) as ActBaseBulletData;
            }
            DrawClass(target, serializedProperty, typeof(ActBulletEvent));

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }

            if (GUILayout.Button("一键复制"))
            {
                ActUtility.Copy((target as ActBulletEvent).BulletData);
            }
        }
    }
}
