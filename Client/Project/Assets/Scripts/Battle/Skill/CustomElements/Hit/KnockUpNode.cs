using System.Collections;
using System.Collections.Generic;
using GraphProcessor;
using UnityEngine;

public enum EKnockflyAnimState
{
    Hurt,
    KnockFly,
    KnockFly2,
    KnockBack
}

[System.Serializable, NodeMenuItem("技能/击中效果/击飞")]
public class KnockUpNode : UniversalNode
{
    [SerializeField, Input("角度")]
    public int angle;
    [SerializeField, Input("力度")]
    public float v0 = 2;
    [SerializeField, Input("速度")]
    public float speed = 1;

    [SerializeField, Input("动画状态")]
    public EKnockflyAnimState animState = EKnockflyAnimState.Hurt;

    [Input("目标集合")]
    public List<long> targets;

    [Input("目标")]
    public long target;

    public override string name => "击飞";

    protected override void Process()
    {
        base.Process();

        //AudioManager.Instance.PlayAudio("baixiaofei01_attack_liuxingyu_hit.wav");
        Debug.Log($"子弹命中目标");
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
        if (actor == null)
            return;

        Battle.KnockFlyData data = new Battle.KnockFlyData()
        {
            angle = angle,
            direction = (actor.Position - owner.Position).normalized,
            f = v0,
            speed = speed,
            state = animState,
            value = owner.GetComponent<Battle.AttributesComponent>().attack
        };
        actor.Hurt(data);
    }
}
