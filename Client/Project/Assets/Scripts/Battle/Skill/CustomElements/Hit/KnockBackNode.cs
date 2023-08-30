using System;
using System.Collections;
using System.Collections.Generic;
using GraphProcessor;
using UnityEngine;

[NodeMenuItem("击中效果/击退"), Serializable]
public class KnockBackNode : UniversalNode
{
    [Input("冲击力"), SerializeField]
    float f;
    [Input("减速度"), SerializeField]
    float a;
    [Input("速度"), SerializeField]
    public float speed = 1;

    [SerializeField, Input("动画状态")]
    public EKnockflyAnimState animState = EKnockflyAnimState.Hurt;

    [Input("目标集合")]
    public List<long> targets;

    [Input("目标")]
    public long target;

    public override string name => "击退";

    protected override void Process()
    {
        base.Process();

        Debug.Log($"=击退 count {targets.Count} target {target}");
        
        //AudioManager.Instance.PlayAudio("baixiaofei01_attack_liuxingyu_hit.wav");
        for (int i = 0; i < targets.Count; i++)
        {
            Hurt(targets[i]);
        }
        Hurt(target);
    }

    void Hurt(long targetId)
    {
        if (targetId <= 0)
            return;

        var actor = Facade.Battle.GetActor(targetId);
        Battle.KnockBackData data = new Battle.KnockBackData()
        {
            direction = (actor.Position - owner.Position).normalized,
            f = f,
            a = a,
            speed = speed,
            state = animState,
            value = owner.GetComponent<Battle.AttributesComponent>().attack
        };
        actor.Hurt(data);
    }
}
