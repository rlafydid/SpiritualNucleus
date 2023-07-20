using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraManager : MonoSingleton<CameraManager>
{
    public Camera cam;
    public GameObject cameraObj;
    public Transform cameraTrans;


    protected override void Constructor()
    {
        cam = Camera.main;
        cameraObj = cam.gameObject;
        cameraTrans = cam.transform;

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

    public void StartSmoothMove(Vector3 targetPos, float time = -1, Action action = null)
    {
        SmoothManager.Instance.StartSmoothMove(cameraObj, targetPos, time, action);
    }

    public void StartSmoothRotate(Vector3 targetRotate, float time = -1, Action action = null)
    {
        SmoothManager.Instance.StartSmoothRotate(cameraObj, targetRotate, time, action);
    }

    public void ToDefaultView(float time = -1, Action action = null)
    {
        SmoothManager.Instance.ToDefaultMove(cameraObj, time, action);
        SmoothManager.Instance.ToDefaultRotate(cameraObj, time, action);
    }

}
