using System.Collections;
using System.Collections.Generic;
using GraphProcessor;
using UnityEngine;

public class DisplacementNode : UniversalNodeWithOut
{
    [SerializeField, Input("欧拉角")]
    Vector3 eulerAngles;

    [SerializeField, Input("距离")]
    float distance;

    [SerializeField, Input("速度")]
    float speed;

    protected override void Process()
    {
        base.Process();
    }
}
