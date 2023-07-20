using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ObjectTools
{
    public static void AddTriggerEvent(GameObject itemObj, Action action, EventTriggerType triggerType = EventTriggerType.PointerClick)
    {
        CheckCameraEvent();

        var box = itemObj.GetComponent<BoxCollider>();
        if (box == null)
        {
            box = itemObj.AddComponent<BoxCollider>();
        }

        EventTrigger trigger = itemObj.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = itemObj.AddComponent<EventTrigger>();

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = triggerType;
        entry.callback.AddListener(e => action.Invoke());

        trigger.triggers.Clear();
        trigger.triggers.Add(entry);
    }

    static void CheckCameraEvent()
    {
        Camera cam = Camera.main;
        GameObject cameraObj = cam.gameObject;

        if (cameraObj.GetComponent<PhysicsRaycaster>() == null)
        {
            cameraObj.AddComponent<PhysicsRaycaster>();
        }

        if (cameraObj.GetComponent<EventSystem>() == null)
        {
            cameraObj.AddComponent<EventSystem>();
        }

        if (cameraObj.GetComponent<StandaloneInputModule>() == null)
        {
            cameraObj.AddComponent<StandaloneInputModule>();
        }
    }

    public static void SetObjectAlpha(GameObject obj, float alpha)
    {
        Renderer[] rendererArr = obj.GetComponentsInChildren<Renderer>();
        foreach (Renderer item in rendererArr)
        {
            Color color = item.material.GetColor("_Color");
            color.a = alpha;
            item.material.SetColor("_Color", color);
        }
    }

    static Dictionary<Renderer, Color> objectColorDic = new Dictionary<Renderer, Color>();
    public static void SetObjectBlendColor(GameObject obj, Color blendColor)
    {
        Renderer[] rendererArr = obj.GetComponentsInChildren<Renderer>();
        foreach (Renderer item in rendererArr)
        {
            Color color = Color.white;
            if (objectColorDic.ContainsKey(item))
                color = objectColorDic[item];
            else
            {
                color = item.material.GetColor("_Color");
                objectColorDic[item] = color;
            }
            color *= blendColor;
            item.material.SetColor("_Color", color);
        }
    }
}
