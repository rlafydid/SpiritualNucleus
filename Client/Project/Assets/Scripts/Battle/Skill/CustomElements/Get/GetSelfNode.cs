using System.Collections;
using System.Collections.Generic;
using GraphProcessor;
using UnityEngine;

[System.Serializable, NodeMenuItem("技能/获取/自身")]
public class GetSelfNode : UniversalNode
{
    public override string name { get => "获取自身数据"; }

    [Output(name = "id")]
    public long ownerId;
    
    [Output(name = "Position")]
    public Vector3 point;

    [Output(name = "Rotation")] 
    public Quaternion rotation;
    
    [Output(name = "Scale")] 
    public Vector3 scale;
    protected override void Process()
    {
        base.Process();
        this.ownerId = this.skilUnit.OwnerID;
        var actor = Facade.Battle.GetActor(this.ownerId);
        if (actor != null)
        {
            point = actor.Position;
            rotation = actor.Entity.LocalRotation;
            scale = actor.Entity.LocalScale;
        }
    }
}