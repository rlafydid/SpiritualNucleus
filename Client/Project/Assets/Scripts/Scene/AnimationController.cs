using System.Collections;
using System.Collections.Generic;
using LKEngine;
using UnityEngine;

public class AnimationController : LKEngine.Component
{
    public SimpleAnimation animator;
    string curState;
    SimpleAnimation.State state;
    protected override void OnStart()
    {
        animator = ((Entity)Owner).GameObject.GetComponentInChildren<SimpleAnimation>(true);
    }
    public void CrossFade(string name, float fadeLength = 0, bool returnToDefaultState = false)
    {
        curState = name;
        if(animator.IsPlaying(name))
        {
            animator.Stop(name);
        }

        animator.CrossFade(name, fadeLength, returnToDefaultState);
        state = animator.GetState(curState);
    }

    protected override void OnUpdate()
    {
        if (string.IsNullOrEmpty(curState))
            return;

        //bool isPlayDefault = false;
        //if(state.time >= state.length)
        //{
        //    isPlayDefault = true;
        //}
        //if (isPlayDefault)
        //{
        //    PlayDefault();
        //    isPlayDefault = false;
        //}

    }

    public void SetAnimatorValue(string name, float value)
    {
        animator.animator.SetFloat(name, value);
    }

    public void PlayDefault()
    {
        curState = null;
        animator.Stop();
        animator.animator.Play("Move");
        animator.animator.SetFloat("v1", 0);
        animator.animator.SetFloat("v2", 0);
    }
}
