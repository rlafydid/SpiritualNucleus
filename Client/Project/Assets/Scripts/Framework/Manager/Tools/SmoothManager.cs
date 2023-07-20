using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothManager : MonoSingleton<SmoothManager>
{
    Vector3 posVoleciy;
    Vector3 rotateVoleciy;
    Vector3 curPos;
    Vector3 curRotate;
    float time;
    const float maxTime = 1.5f;

    Vector3 defaultPos;
    Vector3 defaultRotate;
    Vector3 targetPos;
    Vector3 targetRotate;

    private GameObject targetObj;
    private Transform targetTrans;
    private Action endSmoothAction;

    bool isStartSmoothMove = false;
    bool isStartSmoothRotate = false;

    public void StartSmoothMove(GameObject obj, Vector3 pos, float time = maxTime, Action endAction = null)
    {
        targetObj = obj;
        targetTrans = obj.transform;

        endSmoothAction = endAction;

        defaultPos = targetTrans.position;
        targetPos = pos;
        curPos = defaultPos;

        posVoleciy = Vector3.zero;

        isStartSmoothMove = true;

        StartSmooth(time);
    }

    public void StartSmoothRotate(GameObject obj, Vector3 rotate, float time = maxTime, Action endAction = null)
    {
        targetObj = obj;
        targetTrans = obj.transform;

        endSmoothAction = endAction;

        defaultRotate = targetTrans.localRotation.eulerAngles;
        targetRotate = rotate;
        curRotate = defaultRotate;

        rotateVoleciy = Vector3.zero;

        isStartSmoothRotate = true;

        StartSmooth(time);
    }

    public void ToDefaultMove(GameObject obj, float time = maxTime, Action endAction = null)
    {
        StartSmoothMove(obj, defaultPos, time, endAction);
    }

    public void ToDefaultRotate(GameObject obj, float time = maxTime, Action endAction = null)
    {
        StartSmoothRotate(obj, defaultRotate, time, endAction);
    }

    void StartSmooth(float time)
    {
        if (time == -1)
            time = maxTime;

        this.time = time;
    }

    void EndSmooth()
    {
        endSmoothAction?.Invoke();
    }

    void Update()
    {
        SmoothUpdate();
    }

    void SmoothUpdate()
    {
        if (!isStartSmoothMove && !isStartSmoothRotate) return;

        if(isStartSmoothMove)
        {
            curPos = Vector3.SmoothDamp(curPos, targetPos, ref posVoleciy, time);
            targetTrans.position = curPos;
        }

        if(isStartSmoothRotate)
        {
            curRotate = Vector3.SmoothDamp(curRotate, targetRotate, ref rotateVoleciy, time);
            targetTrans.localRotation = Quaternion.Euler(curRotate);
        }

        if (time == 0)
        {
            isStartSmoothMove = false;
            isStartSmoothRotate = false;
            EndSmooth();
            return;
        }

        time -= Time.deltaTime;

        if (time < 0)
        {
            time = 0;
        }
    }
}
