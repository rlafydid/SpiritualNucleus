using System.Collections;
using System.Collections.Generic;
using GraphProcessor;
using UnityEngine;

[System.Serializable, NodeMenuItem("转换/Actor转换为Transform值")]
public class ActorConvertToTransformValueNode : UniversalNode
{
    public override string name { get => "Actor转换为Transform值"; }

    [Input(name = "ActorId")]
    public long actorId;

    [Output(name = "Position")]
    public Vector3 position;

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
            position = actor.Position;
            rotation = actor.Entity.LocalRotation;
            scale = actor.Entity.LocalScale;
        }
    }
}
