using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using LKEngine;
using UnityEngine;

public class Main : MonoBehaviour
{
    List<BaseModule> modules;
    bool isInited = false;
    private void Awake()
    {
        if (!isInited)
        {
            Init();
            isInited = true;
        }
        Debug.Log($"Awake");
    }

    private void Init()
    {
        DontDestroyOnLoad(this);
        AddModule();
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log($" Start");
        // Tables table = new Tables(Loader); 
        // var item = table.TbItem.Get(10001);
        // Debug.Log(item.Desc);
        // Debug.Log(table.skill.GetOrDefault(10000).Desc);

        ActCustomAgent.Register();

        foreach (var module in modules)
        {
            module.Start();
        }
        SceneManager.Instance.LoadScene(SceneType.Battle);

    }

    // Update is called once per frame
    void Update()
    {
        UnityEngine.Profiling.Profiler.BeginSample("Main.Update");
        foreach (var module in modules)
        {
            module.Update();
        }
        SceneManager.Instance.Update();
        UnityEngine.Profiling.Profiler.EndSample();
    }

    private void LateUpdate()
    {
        foreach (var module in modules)
        {
            module.LateUpdate();
        }
    }

    private void OnGUI()
    {
        Rect rect = new Rect(20, 20, 50, 50);
        if (GUI.Button(rect, "播放"))
        {
            var obj = GameObject.Find("monster1(Clone)");
            obj.GetComponentInChildren<SimpleAnimation>().CrossFade("Hurt", 0.3f);
        }
    }

    public void AddModule()
    {
        Debug.Log($" AddModule");

        modules = new List<BaseModule>();
        modules.Add(new ResourceManager());
        modules.Add(new Battle.BattleModule());
        modules.Add(new TimerMod());
        modules.Add(new SceneManager());
        modules.Add(new Collision.World());

        foreach (var module in modules)
        {
            module.Init();
        }
    }

}
