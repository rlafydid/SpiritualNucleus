using System;
using System.Collections;
using System.Collections.Generic;
using LKEngine;
using UnityEngine;

public enum EMoveState
{
    None,
    Joystick,
    Lerp,
    Trace,
    Point,
    Path
}

abstract class BaseMoveState
{
    public float MoveSpeed { get; set; }
    public EMoveState State { get; set; }
    protected Entity Entity;

    public bool IsFinish { get; set; }

    protected MoveComponent owner;

    public void Init(MoveComponent owner, Entity entity)
    {
        Entity = entity;
        this.owner = owner;
    }

    public virtual void Enter(object[] param)
    {
        IsFinish = false;
    }

    public abstract void Update(float deltaTime);

    public virtual void Exit()
    {
    }

    protected void MoveToDir(Vector3 dir)
    {
        owner.MoveToDir(dir);
    }

    protected void SetAnimatorValue(float value)
    {
        owner.SetAnimatorValue(value);
    }

    protected void SetAnimatorVelocity(float value)
    {
        owner.SetAnimatorValue(value);
    }
}

class LerpMoveState : BaseMoveState
{
    float timeLength;
    Vector3 targetPos;
    Vector3 beginPos;

    float timer;
    public override void Enter(object[] param)
    {
        base.Enter(param);
        targetPos = (Vector3)param[0];
        timeLength = (float)param[1];
        beginPos = Entity.Position;
        timer = 0;
    }
    public override void Update(float deltaTime)
    {
        timer += deltaTime;

        float t = timer / timeLength;

        Entity.Position = Vector3.Lerp(beginPos, targetPos, t);

        if(t >= 1)
        {
            this.IsFinish = true;
        }
    }
}

class MoveToPointState : BaseMoveState
{
    Vector3 targetPos;

    public override void Enter(object[] param)
    {
        targetPos = (Vector3)param[0];
    }

    public override void Update(float deltaTime)
    {
        Vector3 targetTempPos = targetPos;

        Vector3 dir = targetTempPos - Entity.Position;
        if (dir.magnitude < 0.001f)
        {
            this.IsFinish = true;
            return;
        }

        MoveToDir(dir.normalized);
    }
}

class MoveTraceState : BaseMoveState
{
    Entity target;

    public override void Enter(object[] param)
    {
        target = (Entity)param[0];
    }

    public override void Update(float deltaTime)
    {
        Vector3 targetTempPos = target.Position;

        Vector3 dir = targetTempPos - Entity.Position;
        if (dir.magnitude < 0.001f)
        {
            this.IsFinish = true;
            return;
        }
        Entity.LookAt(Entity.Position + dir);
        MoveToDir(dir.normalized);
    }
}


public class MoveComponent : LKEngine.Component
{
    protected Entity Entity;

    Dictionary<EMoveState, BaseMoveState> states = new Dictionary<EMoveState, BaseMoveState>();

    EMoveState defaultState;
    BaseMoveState curState;

    bool _isMove = true;
    public bool IsMove { get => _isMove; set => _isMove = value; }

    float _moveSpeed;
    public float MoveSpeed { get => _moveSpeed; set => _moveSpeed = value; }

    Action callback;
    public Action Finish { get => callback; set => callback = value; }

    protected override void OnStart()
    {
        Entity = Owner as Entity;
        AddState<LerpMoveState>(EMoveState.Lerp);
        AddState<MoveToPointState>(EMoveState.Point);
        AddState<MoveTraceState>(EMoveState.Trace);
    }

    void AddState<T>(EMoveState stateType) where T : BaseMoveState, new()
    {
        T state = new T();
        state.Init(this, Entity);
        state.State = stateType;
        states.Add(stateType, state);
    }

    public void SetDefaultState(EMoveState state)
    {
        defaultState = state;
        ChangeState(defaultState);
    }

    public void ToDefaultState()
    {
        ChangeState(defaultState);
    }

    public void ChangeState(EMoveState state, params object[] param)
    {
        if (curState != null && curState.State == state)
            return;

        if(states.TryGetValue(state, out BaseMoveState toState))
        {
            curState?.Exit();
            toState.MoveSpeed = _moveSpeed;
            toState.Enter(param);
            curState = toState;
        }
        _isMove = true;
    }

    public void MoveTo(Vector3 targetPos)
    {
        ChangeState(EMoveState.Point, targetPos);
    }

    public void MoveLerpTo(Vector3 targetPos, float time)
    {
        ChangeState(EMoveState.Lerp, targetPos, time);
    }

    public void TraceTarget(Entity target)
    {
        ChangeState(EMoveState.Trace, target);
    }

    // Update is called once per frame
    protected override void OnUpdate()
    {
        if (!_isMove || curState == null)
            return;

        if(curState.IsFinish)
        {
            callback?.Invoke();
            callback = null;
            return;
        }
        curState.Update(Time.deltaTime);
    }

    public void MoveToDir(Vector3 dir)
    {
        Vector3 moveToPos = Entity.Position + dir * _moveSpeed * Time.deltaTime;
        moveToPos = moveToPos.ToGroundPos();

        Vector3 lookAt = moveToPos;
        lookAt.y = Entity.Position.y;
        //Entity.LookAt(lookAt);
        Entity.Position = moveToPos;
    }

    public void SetAnimatorValue(float value)
    {
        Entity.GetComponent<AnimationController>().animator.animator.SetFloat("Move", value);
    }

    public void SetValue(string name, float value)
    {
        Entity.GetComponent<AnimationController>().animator.animator.SetFloat(name, value);
    }

    public void Stop()
    {
        _isMove = false;
        curState = null;
    }
}

public static class RaycastTools
{
    static Ray ray = new Ray();
    public static Vector3 ToGroundPos(this Vector3 p)
    {
        Vector3 origin = p + Vector3.up * 100;
        Vector3 direction = Vector3.down;
        int layer = LayerMask.GetMask("Ground");

        var hits = Physics.RaycastAll(origin, direction, 1000, layer);

        float maxY = int.MinValue;
        foreach(var item in hits)
        {
            if(item.point.y > maxY)
            {
                maxY = item.point.y;
                p.y = maxY;
            }
        }
        return p;
    }

}
