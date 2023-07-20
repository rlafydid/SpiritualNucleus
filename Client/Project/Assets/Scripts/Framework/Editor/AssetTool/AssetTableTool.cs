using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ORII.Asset
{

    public class AssetTableTool
    {
        /// <summary>
        /// 生成资源ID的资源是基于哪个目录
        /// </summary>
        public const string AssetTableDirectory = "Assets/Res";

        //[UnityEditor.Callbacks.DidReloadScripts()]
        [MenuItem("Tools/配置相关/生成资源表")]
        public static void GenerateAssetTable()
        {
            string[] allAssets = AssetDatabase.FindAssets("", new string[] { AssetTableDirectory });
            List<AssetTable> tables = new List<AssetTable>();
            foreach (var asset in allAssets)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(asset);

                FileInfo file = new FileInfo(Application.dataPath + assetPath.Substring("Assets".Length));
                if (file.Attributes.HasFlag(FileAttributes.Directory))
                    continue;

                string key = file.Name;

                AssetTable assetTable = new AssetTable(key, assetPath, asset);
                tables.Add(assetTable);
            }
            WriteAssetConfig(tables.ToArray());
            AssetDatabase.Refresh();
            Debug.Log("生成资源表完成 资源数量:" + tables.Count);
        }


        public static string GetAssetIDByGUID(string guid)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            AssetImporter importer = AssetImporter.GetAtPath(assetPath);
            return importer == null ? string.Empty : importer.userData;
        }

        public static string GetAssetIDByObject(Object asset)
        {
            if (asset == null)
                return null;

            string path = AssetDatabase.GetAssetPath(asset);
            AssetImporter importer = AssetImporter.GetAtPath(path);
            return importer == null ? string.Empty : importer.userData;
        }


        public static void WriteAssetConfig(AssetTable[] array, bool prettyPrint = true)
        {
            if (array.Length == 0)
            {
                return;
            }
            AssetTableContainer container = new AssetTableContainer();
            container.tables = array;
            string json = JsonUtility.ToJson(container, prettyPrint);
            FileInfo file = new FileInfo(AssetConfig.assetConfigPath);

            if(!Directory.Exists(file.DirectoryName))
            {
                Directory.CreateDirectory(file.DirectoryName);
            }
            //if (!file.Exists)
            //    file.Create();

            using (StreamWriter sw = new StreamWriter(AssetConfig.assetConfigPath, false))
            {
                sw.Write(json);
                sw.Flush();
                sw.Close();
            }
        }
    }
}