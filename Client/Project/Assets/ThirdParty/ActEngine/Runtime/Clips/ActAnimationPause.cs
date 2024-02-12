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
            _state = owner.GetMonoComponentInChildren<SimpleAnimation>().getPlayingState;
            _speed = _state.speed;
            return base.OnTrigger();
        }

        public override void OnDisable()
        {
            base.OnDisable();
            _state.speed = _speed;
        }
    }
}
