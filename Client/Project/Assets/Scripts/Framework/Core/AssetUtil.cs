using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class AssetTableContainer
{
    public AssetTable[] tables;
}

[Serializable]
public class AssetTable
{
    public AssetTable(string id, string path, string guid)
    {
        ID = id;
        Path = path;
        GUID = guid;
    }
    public string ID;
    public string Path;
    public string GUID;
}

public static class AssetConfig
{
    public const string assetConfigPath = "Assets/Res/Dataset/AssetTable.txt";
}

public class AssetUtil
{

    private static Dictionary<string, string> assetTableCache;

    //public static async UniTask InitializeAssetTable()
    //{
    //    var assetAsync = await Component.LoadAsync<TextAsset>(AssetConfig.assetConfigPath);
    //    if (assetAsync.Asset == null)
    //    {
    //        Debug.LogError("InitAssetTable Error Not find AssetConfig:" + AssetConfig.assetConfigPath);
    //        return;
    //    }

    //    string json = (assetAsync.Asset as TextAsset).text;
    //    AssetTableContainer container = JsonUtility.FromJson<AssetTableContainer>(json);
    //    if (container == null)
    //    {
    //        Debug.LogError("InitAssetTable Error:/r/n" + json);
    //        return;
    //    }

    //    assetTableCache = container.tables.ToDictionary((p) => p.ID, (o) => o.Path);
    //    container.tables = null;
    //    container = null;
    //    Component.Unload(assetAsync);
    //    Debug.Log("InitializeAssetTable Over.  Asset Count:" + assetTableCache.Count);
    //}

#if UNITY_EDITOR
    public static void InitializeAssetTableInEditor()
    {
        try
        {
            string json = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>(AssetConfig.assetConfigPath).text;
            AssetTableContainer container = JsonUtility.FromJson<AssetTableContainer>(json);
            assetTableCache = new Dictionary<string, string>(container.tables.Length);
            foreach (var item in container.tables)
            {
                if(!assetTableCache.ContainsKey(item.ID))
                    assetTableCache.Add(item.ID, item.Path);
                else
                {
                    //Debug.Log($"id重复 {item.ID}");
                }
            }
            container.tables = null;
            container = null;
        }
        catch (System.Exception e)
        {
            Debug.LogError("编辑器下未发现资源映射表, 请及时生成" + e.ToString());
        }
    }
# endif
    public static string GetPath(string ID)
    {
        if (assetTableCache == null)
        {
            throw new System.Exception("资源表为空");
        }

        if (!assetTableCache.TryGetValue(ID, out var path))
        {
            throw new System.Exception("资源表中未找到ID:" + ID);
        }

        return path;
    }


    private static Dictionary<string, string> assetTable = new Dictionary<string, string>();

    public static bool HasPath(string ID)
    {
        return assetTableCache != null ? assetTableCache.ContainsKey(ID) : false;
    }
}