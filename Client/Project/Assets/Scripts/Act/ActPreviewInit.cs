using Act;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
# if UNITY_EDITOR
public class ActPreviewInit : MonoBehaviour
{
    bool isInited = false;
    List<BaseModule> modules;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);

        AddModule();
        StartAsync();
    }

    public void AddModule()
    {
        modules = new List<BaseModule>();
        modules.Add(new Battle.BattleModule());
        modules.Add(new Battle.SkillMudule());
        modules.Add(new TimerMod());
        modules.Add(new ResourceManager());

        foreach (var module in modules)
        {
            module.Init();
        }
    }

    async void StartAsync()
    {
        if (isInited)
            return;
        foreach (var module in modules)
        {
            module.Start();
        }
        StartPreviewPlayer();
    }

    private void Update()
    {
        if (!isInited)
            return;
        foreach (var module in modules)
        {
            module.Update();
        }
    }

    void StartPreviewPlayer()
    {
        GameObject.Find("UICamera").gameObject.SetActive(false);

        ActCustomAgent.Register();

        GameObject.FindObjectOfType<ActPreviewPlayer>().StartPreview(InitBattle);
    }

    void InitBattle()
    {
        isInited = true;
        gameObject.AddComponent<Act.Simulator.ActBattleSimulator>();

        
    }
}
#endif
