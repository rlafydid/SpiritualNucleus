using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Act
{
    public class ActEditorUtility
    {
        static Dictionary<string, List<ActAsset>> actMapping = new Dictionary<string, List<ActAsset>>();

        public static float GetParticleSystemDuration(string assetName)
        {
            var guids = AssetDatabase.FindAssets($"t:prefab {assetName}");

            if (guids.Length == 0)
            {
                Debug.LogError($"没找到特效{assetName}");
                return 0.5f;
            }

            foreach (var scriptGUID in guids)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(scriptGUID);
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

                if (prefab != null && prefab.name == assetName)
                {
                    return 1;
                }
            }

            Debug.LogError($"没找到特效{assetName}");
            return 0.5f;
        }

        public static float GetParticleDuration(GameObject prefab)
        {
            return 1;
        }

        public static List<ActAsset> GetActAssetsByModel(string model)
        {
            if (string.IsNullOrEmpty(model))
                return null;

            if (actMapping.Count == 0)
            {
                var guids = UnityEditor.AssetDatabase.FindAssets($"t:ActAsset");

                foreach (var scriptGUID in guids)
                {
                    var assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(scriptGUID);
                    var actAsset = UnityEditor.AssetDatabase.LoadAssetAtPath<Act.ActAsset>(assetPath);

                    if (actAsset != null && !string.IsNullOrEmpty(actAsset.Model))
                    {
                        if (!actMapping.TryGetValue(actAsset.Model, out var actList))
                        {
                            actList = new List<ActAsset>();
                            actMapping.Add(actAsset.Model, actList);
                        }
                        actList.Add(actAsset);
                    }
                }
            }

            if (actMapping.TryGetValue(model, out var list))
            {
                return new List<ActAsset>(list);
            }
            else
                return null;
        }
    }
}

