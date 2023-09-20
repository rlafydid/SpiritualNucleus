using System.Collections;
using System.Collections.Generic;
using LKEngine;
using UnityEngine;

namespace Battle
{
    class JoystickMoveComponent : ActorComponent
    {
        float _moveSpeed = 7;

        public float MoveSpeed { get => _moveSpeed; set => _moveSpeed = value; }

        float value;

        public Vector3 GetMoveDirection
        {
            get
            {
                return _direction;
            }
        }

        private Vector3 _lastJoytickDirection;

        protected override void OnUpdate()
        {
            // float x = Input.GetAxis("Horizontal");
            // float y = Input.GetAxis("Vertical");
            // Vector3 offset = new Vector3(x, 0, y);
            // if (offset == Vector3.zero)
            //     return;

            //offset = Entity.LocalRotation * offset;

            //MoveToDir(Vector3.Lerp(Entity.Forward, offset.normalized, Time.deltaTime));

            if (_lastJoytickDirection != Vector3.zero)
            {
                SetMoveDelta(_lastJoytickDirection);
            }
            else
            {
                StopRunFaster();
            }
        }

        public bool FastMoving { get; private set; }

        public void StartRunFaster()
        {
            GetActor.PlayAnim("RunFaster");
            MoveSpeed = 20;
            GetActor.DontToDefaultAnimation();
            FastMoving = true;
        }

        public void StopRunFaster()
        {
            GetActor.Entity.GetComponent<AnimationController>().PlayDefault();
            MoveSpeed = 5;
            GetActor.TurnOnToDefaultAnimation();
            FastMoving = false;
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
            _lastJoytickDirection = dir;
            SetMoveDelta(dir);
        }
        
        public void MoveToDir(Vector3 dir)
        {
            Vector3 camToOwner = ownerActor.Position - SceneManager.Instance.Camera.Camera.transform.position;
            camToOwner.Normalize();
            camToOwner.y = 0;

            Vector3 forward = ownerActor.Entity.Forward;
            forward.y = 0;
            
            float angle = Vector3.SignedAngle(Vector3.forward, camToOwner, Vector3.up);
            
            dir = Quaternion.AngleAxis(angle, Vector3.up) * dir;
            
            _direction = dir;

            // dir.x *= 0.7f;

            Vector3 moveToPos = ownerActor.Position + dir * _moveSpeed * Time.deltaTime;
            
            Vector3 lookAt = moveToPos;
            lookAt.y = ownerActor.Position.y;
            // Entity.LookAt(lookAt);
            Vector3 groundPos = moveToPos.ToGroundPos();
            moveToPos.y = groundPos.y;
            // Debug.Log($"ownerActor.Position {ownerActor.Position} moveToPos {moveToPos}");
            ownerActor.Position = moveToPos;
            ownerActor.Entity.LookAt(ownerActor.Position + camToOwner);
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
            Debug.Log($"英雄 move value {value}");
            return value > 0;
        }

        public bool IsNotMoving()
        {
            return value == 0;
        }
    }
}