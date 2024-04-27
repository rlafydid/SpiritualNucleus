using System;
using System.Collections;
using System.Collections.Generic;
using LKEngine;
using Unity.Burst.Intrinsics;
using UnityEngine;

namespace Battle
{
    class JoystickMoveComponent : ActorComponent
    {
        float _moveSpeed = 7;

        public float MoveSpeed { 
            get => _moveSpeed; 
            set => _moveSpeed = value;
        }

        public Vector3 GetMoveDirection
        {
            get
            {
                return _direction;
            }
        }

        private Vector3 _lastJoytickDirection;
        
        private bool _isMoving = false;
        public bool IsMoving { get => _isMoving; }

        private event Action<bool> _moveStateAction;
        
        protected override void OnUpdate()
        {
            SetMoveDelta(_lastJoytickDirection);
        }

        public void StartRunFaster()
        {
            GetActor.PlayAnim("RunFaster");
            MoveSpeed = 20;
            GetActor.DontToDefaultAnimation();
        }

        public void StopRunFaster()
        {
            GetActor.Entity.GetComponent<AnimationController>().PlayDefault();
            MoveSpeed = 5;
            GetActor.TurnOnToDefaultAnimation();
        }

        public void RegisterMoveStateChanged(Action<bool> moveStateAction)
        {
            _moveStateAction += moveStateAction;
        }
        
        public void UnregisterMoveStateChanged(Action<bool> moveStateAction)
        {
            _moveStateAction -= moveStateAction;
        }
        
        public void SetMoveDelta(Vector3 val)
        {
            if (val.magnitude > 0.1f)
            {
                StartMove();
            }
            else
            {
                StopMove();
            }
            MoveToDir(val.normalized);
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
            Vector3 groundPos = moveToPos.ToGroundPos();
            moveToPos.y = groundPos.y;
            // Debug.Log($"ownerActor.Position {ownerActor.Position} moveToPos {moveToPos}");
            // ownerActor.Entity.LookAt(ownerActor.Position + camToOwner);
            ownerActor.Entity.LookAt(moveToPos);
            ownerActor.Position = moveToPos;
        }

        public void SetValue(string name, float value)
        {
            ownerActor.Entity.GetComponent<AnimationController>().SetAnimatorValue(name, value);
        }

        public void StartMove()
        {
            Active = true;
            _isMoving = true;
            SetValue("v1", 1);
            _moveStateAction?.Invoke(true);
        }
        
        public void StopMove()
        {
            StopRunFaster();
            Active = false;
            _isMoving = false;
            SetValue("v1", 0);
            _moveStateAction?.Invoke(false);
        }
    }
}