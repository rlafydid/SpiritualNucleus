using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LKEngine
{
    public class Entity : Component
    {
        public GameObject GameObject;
        public Transform Transform;

        public bool IsLoaded { get; set; }

        Vector3 _position;
        public Vector3 Position
        {
            get
            {
                if (IsLoaded)
                {
                    _position = Transform.position;
                }
                return _position;
            }
            set
            {
                _position = value;
                if (IsLoaded)
                    Transform.position = value;
            }
        }

        Vector3 _localScale = Vector3.one;
        public Vector3 LocalScale { get => _localScale;
            set
            {
                _localScale = value;
                if(IsLoaded)
                    Transform.localScale = value;
            }
        }

        Quaternion _localRotation = Quaternion.identity;
        public Quaternion LocalRotation { get => _localRotation;
            set
            {
                _localRotation = value;
                if(IsLoaded)
                {
                    Transform.localRotation = _localRotation;
                }
            }
        }

        public string ResName;

        public Vector3 Forward { get => Transform.forward.normalized; }
        public Vector3 Up { get => Transform.up.normalized; }
        public Vector3 Right { get => Transform.right.normalized; }

        public Action Finished { get; set; }
        public long Id { get; set; }

        static long entityId;

        public Entity(params Type[] subTypes) : base(subTypes)
        {
            Id = ++entityId;
        }

        public Entity(ComponentGroup group) : base(group)
        {
            Id = ++entityId;
        }

        public void Load()
        {
            GameObject = Facade.Asset.Instantiate<GameObject>(ResName);
            Transform = GameObject.transform;
            IsLoaded = true;
            ResetTransform();
            this.Start();
            Finished?.Invoke();
        }

        void ResetTransform()
        {
            LocalScale = _localScale;
            Position = _position;
            LocalRotation = _localRotation;
        }

        public void LookAt(Vector3 point)
        {
            if(IsLoaded)
                Transform.LookAt(point);
        }

        protected override void OnDestroy()
        {
            GameObject.Destroy(GameObject);
        }
    }

}
