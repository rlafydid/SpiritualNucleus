using System;
using System.Collections;
using System.Collections.Generic;
using FSM;
using UnityEngine;

namespace Battle
{
    public interface IState
    {
        void SetData(IStateData data);
        void Enter();
        void Update();
        void Exit();
    }

    public interface IStateData
    {
        
    }

    public struct StateEmptyData : IStateData
    {
    }

    public class TransitionParams
    {
        public int lastState;
        public object param;
    }

    public class BaseState<T> : BaseState where T : IStateData
    {
        protected T Data { get; private set; }

        public override void SetData(IStateData data)
        {
            if(data != null)
                Data = (T)data;
        }
    }   
    
    public class BaseState : IState
    {
        public SceneActorController owner;
        public FiniteStateMachine fsm;

        protected List<Transition> transitions = new List<Transition>();

        public int state;

        protected bool IsUpdate { get; set; } = true;

        public virtual void SetData(IStateData data)
        {
        }

        public void Enter()
        {
            IsUpdate = true;
            OnEnter();
        }
        public void Update()
        {
            if(IsUpdate)
                OnUpdate();
        }
        public void Exit()
        {
            OnExit();
        }
        
        protected virtual void OnEnter()
        {
        }
        protected virtual void OnUpdate() { }
        protected virtual void OnExit() { }

        public void AddTransition(Transition transition)
        {
            transition.fsm = fsm;
            transitions.Add(transition);
        }

        public bool TryTransitionState(out int state)
        {
            foreach(var trasition in transitions)
            {
                if(trasition.CanTransition())
                {
                    state = trasition.toState;
                    return true;
                }
            }
            state = 0;
            return false;
        }
        
        /// <summary>
        /// 是否可以转换到指定状态
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public bool CanTransitionToState(int state)
        {
            foreach (var trasition in transitions)
            {
                if (state == trasition.toState && trasition.CanTransition())
                {
                    return true;
                }
            }

            return false;
        }
        
        /// <summary>
        /// 跳转到连接的状态
        /// </summary>
        /// <returns></returns>
        public bool ToLinkedState()
        {
            Transition transition = null;
            foreach (var item in transitions)
            {
                if (item.CanTransition() && (transition == null || item.priority > transition.priority))
                {
                    transition = item;
                }
            }

            if (transition != null)
            {
                fsm.ChangeState(transition.toState);
                return true;
            }
            else
            {
                fsm.ToDefaultState();
            }

            return false;
        }

        protected void ChangeState(Enum state, IStateData param = null)
        {
            fsm.ChangeState(state, param);
        }

        protected void TriggerEvent(Enum state, IStateData param = null)
        {
            fsm.TriggerEvent(state, param);
        }
        
        /// <summary>
        /// 退出当前状态
        /// </summary>
        protected void ExitState()
        {
            ToLinkedState();
        }

        protected void ToDefaultState()
        {
            fsm.ToDefaultState();
        }
    }
}
