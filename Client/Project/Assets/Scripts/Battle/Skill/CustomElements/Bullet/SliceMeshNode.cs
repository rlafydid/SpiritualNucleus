using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using EzySlice;
using GraphProcessor;
using LKEngine;
using UnityEngine;

[NodeMenuItem("技能/功能/切割Mesh")]
public class SliceMeshNode : UniversalNodeWithOut
{
    public override string name => "切割Mesh";

    [Input("子弹Id")]
    long bulletId;
    [Input("目标Id")]
    long targetId;

    protected override void Process()
    {
        base.Process();
        Entity bullet = SceneManager.Instance.GetEntity(bulletId);
        Entity target = SceneManager.Instance.GetEntity(targetId);
        var hull = target.GameObject.GetComponentInChildren<MeshFilter>().gameObject.Slice(bullet.Position, bullet.Up);
        var up = hull.CreateUpperHull();
        var down = hull.CreateLowerHull();
        up.transform.DOMove(up.transform.position + Vector3.up * 4, 1);
        down.transform.DOMove(down.transform.position + Vector3.down * 4, 1);
    }
}
