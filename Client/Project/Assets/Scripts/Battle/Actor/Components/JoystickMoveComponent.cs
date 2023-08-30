using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    class JoystickMoveComponent : ActorComponent
    {
        float _moveSpeed = 5;

        public float MoveSpeed { get => _moveSpeed; set => _moveSpeed = value; }

        float value;

        protected override void OnUpdate()
        {
            // float x = Input.GetAxis("Horizontal");
            // float y = Input.GetAxis("Vertical");
            // Vector3 offset = new Vector3(x, 0, y);
            // if (offset == Vector3.zero)
            //     return;

            //offset = Entity.LocalRotation * offset;

            //MoveToDir(Vector3.Lerp(Entity.Forward, offset.normalized, Time.deltaTime));

            if (_direction != Vector3.zero)
            {
                SetMoveDelta(_direction);
            }
        }
        
        public void SetMoveDelta(Vector3 val)
        {
            MoveToDir(val.normalized);

            value = Mathf.Max(Mathf.Abs(val.x), Mathf.Abs(val.z));
            SetValue("v1", val.z);
            SetValue("v2", val.x);
        }

        private Vector3 _direction;
        public void SetMoveDir(Vector3 dir)
        {
            _direction = dir;
            SetMoveDelta(_direction);
        }
        
        public void MoveToDir(Vector3 dir)
        {
            dir.x *= 0.7f;
            Vector3 moveToPos = ownerActor.Position + dir * _moveSpeed * Time.deltaTime;

            Vector3 lookAt = moveToPos;
            lookAt.y = ownerActor.Position.y;
            // Entity.LookAt(lookAt);
            Vector3 groundPos = moveToPos.ToGroundPos();
            moveToPos.y = groundPos.y;
            // Debug.Log($"ownerActor.Position {ownerActor.Position} moveToPos {moveToPos}");
            ownerActor.Position = moveToPos;
        }

        protected override void OnLateUpdate()
        {
            base.OnLateUpdate();
           

        }

        public void SetValue(string name, float value)
        {
            ownerActor.Entity.GetComponent<AnimationController>().SetAnimatorValue(name, value);
        }

        public bool IsMoving()
        {
            return value > 0;
        }

        public bool IsNotMoving()
        {
            return value == 0;
        }
    }
}