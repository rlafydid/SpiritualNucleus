using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Act
{
    [ActDisplayName("暂停动画", "暂停动画"), Serializable]
    public class ActAnimationPause : ActBaseClip
    {
        private SimpleAnimation.State _state;
        private float _speed = 1;
        public override bool OnTrigger()
        {
            _speed = 0;
            var animation = owner.GetMonoComponentInChildren<SimpleAnimation>();
            _state = animation.getPlayingState;
            if (_state == null)
                _state = animation.GetState("Default");
            if (_state != null)
            {
                _speed = _state.speed;
                _state.speed = 0;
            }
            
            var animator = owner.GetMonoComponentInChildren<Animator>();
            if(_speed == 0)
                _speed = animator.speed;
            animator.speed = 0;

            return base.OnTrigger();
        }

        public override void OnDisable()
        {
            base.OnDisable();
            if(_state != null)
                _state.speed = _speed;
            var animator = owner.GetMonoComponentInChildren<Animator>();
            if (animator != null)
                animator.speed = _speed;
        }
    }
}
