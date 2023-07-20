using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Act
{
    public static class ActUtility
    {
        public static int EventFPS = 30; /*事件帧率*/

        static Dictionary<Type, ActDisplayNameAttribute> displayNameMapping = new Dictionary<Type, ActDisplayNameAttribute>();

        static Assembly actTimelineEditorAssembly;

        public static Dictionary<Type, ActDisplayNameAttribute> GetClipDisplayNameMapping
        {
            get
            {
                CheckDisplayNameMapping();
                IEnumerable<KeyValuePair<Type, ActDisplayNameAttribute>> map = displayNameMapping.Where(item => typeof(ActBaseClip).IsAssignableFrom(item.Key));
                return map.OrderBy(item => item.Value.Order).ToDictionary(item => item.Key, item => item.Value);
            }
        }

        public static Dictionary<Type, ActDisplayNameAttribute> GetEventDisplayNameMapping
        {
            get
            {
                CheckDisplayNameMapping();
                IEnumerable<KeyValuePair<Type, ActDisplayNameAttribute>> map = displayNameMapping.Where(item => typeof(ActBaseEvent).IsAssignableFrom(item.Key));
                return map.OrderBy(item => item.Value.Order).ToDictionary(item => item.Key, item => item.Value);
            }
        }

        public static Assembly GetActTimelineEditorAssembly()
        {
            if (actTimelineEditorAssembly == null)
            {
                /*这是Act时间轴程序集名. 为了能够使用UnityEditor内部类, 暂时使用这个名, 自定义的名无法拿到UnityEditor内部类*/
                string actTimelineAssemblyName = "UnityEditor.UWP.Extensions";
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (assembly.GetName().Name.IndexOf(actTimelineAssemblyName) >= 0)
                    {
                        actTimelineEditorAssembly = assembly;
                        break;
                    }
                }
            }
            return actTimelineEditorAssembly;
        }

        public static string GetDisplayName(Type clipType)
        {
            CheckDisplayNameMapping();
            if (displayNameMapping.TryGetValue(clipType, out var item))
            {
                return item.DisplayName;
            }
            return "";
        }

        public static void OpenTimeline(ActAsset actData)
        {
            if (actData == null)
                return;

            Assembly assembly = ActUtility.GetActTimelineEditorAssembly();
            MethodInfo methodInfo = assembly.GetType("UnityEditor.ActTimeline.TimelineWindow").GetMethod("ShowWindow", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            methodInfo.Invoke(null, new object[] { actData });
        }

        static void CheckDisplayNameMapping()
        {
            if (displayNameMapping.Count > 0)
                return;

            Type baseType = typeof(ActBaseClip);

            foreach (var type in baseType.Assembly.GetTypes())
            {
                if (!type.IsAbstract && type.IsClass)
                {
                    var attr = type.GetCustomAttribute<ActDisplayNameAttribute>();
                    if (attr != null)
                    {
                        displayNameMapping.Add(type, attr);
                    }
                }
            }
        }

        static ActBaseBulletData copiedData;
        public static void Copy(ActBaseBulletData bulletData)
        {
            copiedData = bulletData;
        }

        public static void PasteInto(object value)
        {
            if (copiedData == null)
            {
                Debug.Log("没有可复制的数据");
                return;
            }

            var fields = copiedData.GetType().GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            foreach (var field in fields)
            {
                if (field.GetCustomAttribute<SynchroDataAttribute>() != null)
                {
                    var targetField = value.GetType().GetField(field.Name);
                    if (targetField != null)
                    {
                        targetField.SetValue(value, field.GetValue(copiedData));
                    }
                }
            }
            copiedData = null;
        }


        public static Act.ActAsset GetActAsset(string assetName)
        {
#if UNITY_EDITOR
            var guids = UnityEditor.AssetDatabase.FindAssets($"t:ActAsset {assetName}");

            if (guids.Length == 0)
            {
                Debug.LogError($"没找到Act资源 {assetName}");
                return null;
            }

            foreach (var scriptGUID in guids)
            {
                var assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(scriptGUID);
                var actAsset = UnityEditor.AssetDatabase.LoadAssetAtPath<Act.ActAsset>(assetPath);

                if (actAsset != null && actAsset.name == assetName)
                {
                    return actAsset;
                }
            }

            Debug.LogError($"没找到Act资源 {assetName}");
#endif
            return null;
        }

        public static T GetAsset<T>(string assetName, string filter = "object") where T : UnityEngine.Object
        {
            if (string.IsNullOrEmpty(assetName))
            {
                return null;
            }
#if UNITY_EDITOR
            if (typeof(T) == typeof(GameObject))
            {
                filter = "prefab";
            }

            var guids = UnityEditor.AssetDatabase.FindAssets($"t:{filter} {assetName}");

            if (guids.Length == 0)
            {
                Debug.LogError($"没找到资源 {assetName}");
                return null;
            }

            foreach (var scriptGUID in guids)
            {
                var assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(scriptGUID);
                var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetPath);

                if (asset != null && asset.name == assetName)
                {
                    return (T)asset;
                }
            }

            Debug.LogError($"没找到资源 {assetName}");
#endif
            return null;
        }


    }
}



