using System;
using System.Collections;
using System.Collections.Generic;
using GraphProcessor;
using UnityEngine;

[Serializable]
public class ForeachListNode<T> : UniversalNodeWithOut
{
    [Input("列表")]
    public List<T> list;
    
    [Output("元素")]
    public T element;

    public override string name { get => $"遍历{typeof(T).Name}列表"; }

    protected override void Process()
    {
        base.Process();
        element = default;
        foreach(var item in list)
        {
            element = item;
            Execute(nameof(this.executes));
        }
        list.Clear();
    }
}
