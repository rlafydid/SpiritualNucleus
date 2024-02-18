using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGizmos
{
    void Draw();
}

public abstract class BaseGizmos : IGizmos
{
    public Color color = Color.gray;
    public float duration = 5f;
    public abstract void Draw();
}

public class SphereGizmos : BaseGizmos
{
    public Vector3 center;
    public float radius = 0.5f;

    public SphereGizmos(Vector3 center, float radius = 0.5f)
    {
        this.center = center;
        this.radius = radius;
    }
    
    public override void Draw()
    {
        Gizmos.color = color;
        Gizmos.DrawSphere(center, radius);
    }
}

public class GizmosUtils : MonoBehaviour
{

    public static GizmosUtils Instance
    {
        get
        {
            var util = GameObject.FindObjectOfType<GizmosUtils>();
            if (util == null)
            {
                var go = new GameObject("GizmosUtils");
                util = go.AddComponent<GizmosUtils>();
            }

            return util;
        }
    }

    List<BaseGizmos> _gizmosList = new();

    public void Draw(BaseGizmos gizmos)
    {
        _gizmosList.Add(gizmos);
    }

    private void Update()
    {
        for (int i = _gizmosList.Count - 1; i >= 0; i--)
        {
            var item = _gizmosList[i];
            item.duration -= Time.deltaTime;
            if(item.duration <= 0)
                _gizmosList.RemoveAt(i);
        }
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            return;
        foreach (var item in _gizmosList)
        {
            item.Draw();
        }
    }
}
