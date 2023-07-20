using System;
using System.Collections;
using System.Collections.Generic;
using Battle;
using UnityEngine;

public class ActCustomAgent
{
    public static void Register()
    {
        Act.Facade.Externals.LoadAssetAsync = LoadAssetAsync;
        Act.Facade.Externals.ReleaseAsset = ReleaseAsset;

        Act.Facade.Externals.InstantiateAsync = InstantiateAsync;
        Act.Facade.Externals.ReleaseInstance = ReleaseInstance;

        Act.Facade.Externals.PlayAudio = PlayAudio;
        Act.Facade.Externals.GetAudioTime = GetAudioTime;

        Act.Facade.Externals.GetResourceName = GetResourceName;

        Act.Facade.Externals.Delay = Delay;
    }

    public static async void LoadAssetAsync(string assetName, Action<UnityEngine.Object> callback)
    {
        callback?.Invoke(Facade.Asset.Load<UnityEngine.Object>(assetName));
        //var res = Facade.Asset.Load<UnityEngine.Object>(assetName);
    }

    public static async void ReleaseAsset(UnityEngine.Object asset)
    {
    }

    public static void InstantiateAsync(string assetName, Action<UnityEngine.Object> loaded, bool isPrefab)
    {
        loaded?.Invoke(Facade.Asset.Instantiate<UnityEngine.Object>(assetName));
    }

    public static void ReleaseInstance(UnityEngine.Object obj)
    {
        GameObject.Destroy(obj);
    }

    public static void PlayAudio(string configID, string curSheet, string name)
    {
        if (name.IndexOf(".wav") < 0)
            name += ".wav";
        AudioManager.Instance.PlayAudio(name);
    }

    static float GetAudioTime(string cueSheet, string name)
    {
        if (name.IndexOf(".wav") < 0)
            name += ".wav";
        var clip = Facade.Asset.Load<AudioClip>(name);
        return clip != null ? clip.length : 0;
    }

    static string GetResourceName(string resName, int type)
    {
        string path = resName;
        if (resName.IndexOf("PREFAB") < 0)
        {
            path = "PREFAB_" + resName;
        }
        return path;
    }

    static void Delay(float time, Action action)
    {
        //await System.Threading.Tasks.Task.Delay((int)(time * 1000));
        TimerMod.Delay(time, action);
    }
}


