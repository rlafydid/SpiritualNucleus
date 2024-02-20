using System;
using System.Collections;
using System.Collections.Generic;
using Battle;
using UnityEngine;

namespace FSM
{
    public class Transition
    {
        public int fromState = -1;
        public int toState;

        public int priority = 0;

        //表示在达成条件时自动转换
        public bool willAutoTransit = true;
        
        List<Condition> conditions = new List<Condition>();

        public FiniteStateMachine fsm;

        public Transition(Enum fromState, Enum toState)
        {
            this.fromState =  fromState.ToInt();
            this.toState = toState.ToInt();
        }

        public Transition(int fromState, int toState){
            this.fromState = fromState;
            this.toState = toState;
        }

        public Transition(Enum toState, bool willAutoTransit = false)
        {
            this.toState = toState.ToInt();
            this.willAutoTransit = willAutoTransit;
        }

        public Transition AddCondition(Condition condition)
        {
            condition.Init(this);
            conditions.Add(condition);
            return this;
        }

        public bool CanTransition()
        {
            foreach (var item in conditions)
            {
                if (!item.Pass())
                {
                    return false;
                }
            }
            return true;
        }
    }

    public abstract class Condition
    {
        protected SceneActorController owner { get => fsm.GetActor; }
        protected FiniteStateMachine fsm { get => transition.fsm; }

        protected Transition transition;

        public abstract bool Pass();

        public void Init(Transition transition)
        {
            this.transition = transition;
        }
    }
}
