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

        List<Condition> conditions = new List<Condition>();
        List<Func<bool>> conditionFuncs = new List<Func<bool>>();

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

        public Transition(Enum toState)
        {
            this.toState = toState.ToInt();
        }

        public Transition AddCondition(Condition condition)
        {
            condition.Init(this);
            conditions.Add(condition);
            return this;
        }

        public Transition AddCondition(params Func<bool>[] conds)
        {
            conditionFuncs.AddRange(conds);
            return this;
        }

        public bool CanTransition()
        {
            foreach (var item in conditionFuncs)
            {
                if (!item.Invoke())
                {
                    return false;
                }
            }

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
