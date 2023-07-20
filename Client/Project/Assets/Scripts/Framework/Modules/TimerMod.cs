using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerMod : BaseModule
{
    static TimerMod inst;
    public class TimerData
    {
        public float Length;
        public float Timer;
        public Action Callback;
        public bool isUnscale;
    }

    public List<TimerData> timers = new List<TimerData>();

    public static void Delay(float length, Action callback, bool isUnscale = false)
    {
        inst.InternalDelay(length, callback, isUnscale);
    }

    public static void Remove(Action callback)
    {
        inst.InternalRemove(callback);
    }

    public override void Start()
    {
        base.Start();
        inst = this;
    }

    void InternalDelay(float length, Action callback, bool isUnscale = false)
    {
        int index = FindIndex(callback);

        TimerData data = index >= 0 ? timers[index] : new TimerData();
        data.Callback = callback;
        data.Length = length;
        data.Timer = 0;
        data.isUnscale = isUnscale;
        timers.Add(data);
    }

    int FindIndex(Action callback)
    {
        return timers.FindIndex(d => d.Callback == callback);
    }

    void InternalRemove(Action callback)
    {
        int index = FindIndex(callback);
        if(index >= 0)
        {
            timers.RemoveAt(index);
        }
    }

    public override void Update()
    {
        int length = timers.Count;
        for(int i = length - 1; i >= 0; i--)
        {
            TimerData data = timers[i];
            data.Timer += data.isUnscale ? Time.unscaledDeltaTime : Time.deltaTime;
            if(data.Timer >= data.Length)
            {
                timers.RemoveAt(i);
                data.Callback?.Invoke();
            }
        }

    }
}
