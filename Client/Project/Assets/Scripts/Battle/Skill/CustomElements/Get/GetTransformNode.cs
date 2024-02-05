using System.Collections;
using System.Collections.Generic;
using GraphProcessor;
using UnityEngine;

[System.Serializable, NodeMenuItem("技能/获取/获取Transform参数")]
public class GetTransformNode : UniversalNode
{
    public override string name { get => "Actor转换为Transform值"; }

    [Input(name = "ActorId")]
    public long actorId;

    [Output(name = "Position")]
    public Vector3 point;

    [Output(name = "Rotation")] 
    public Quaternion rotation;
    
    [Output(name = "Scale")] 
    public Vector3 scale;
    protected override void Process()
    {
        base.Process();
        var actor = Facade.Battle.GetActor(actorId);
        if (actor != null)
        {
            point = actor.Position;
            rotation = actor.Entity.LocalRotation;
            scale = actor.Entity.LocalScale;
        }
    }
}