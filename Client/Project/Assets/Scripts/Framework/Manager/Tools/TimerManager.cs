using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerManager : MonoSingleton<TimerManager>
{
    float waitTime;
    bool isWait = false;
    Action waitAction;

    public void WaitTime(float waitTime, Action action)
    {
        this.waitTime = waitTime;
        waitAction = action;
        isWait = true;
    }

    void Update()
    {
        WaitUpdate();
    }

    void WaitUpdate()
    {
        if (!isWait) return;

        this.waitTime -= Time.deltaTime;

        if (this.waitTime <= 0)
        {
            isWait = false;
            waitAction?.Invoke();
        }
    }
}
