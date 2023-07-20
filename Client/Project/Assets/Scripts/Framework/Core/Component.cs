using System;
using System.Collections;
using System.Collections.Generic;

namespace LKEngine
{
    public class Component
    {
        public Component Owner { get; set; }

        bool _active = true;
        public bool Active { get => _active; set => _active = value; }

        public List<Component> children = new List<Component>();

        public Component(params Type[] subTypes)
        {
            InitGroup(subTypes);
        }

        public Component(ComponentGroup group)
        {
            InitGroup(group.GetTypes.ToArray());
        }

        public void InitGroup(Type[] subTypes)
        {
            foreach(var i in subTypes)
            {
                Component component = (Component)Activator.CreateInstance(i);
                component.Owner = this;
                children.Add(component);
                component.Awake();
            }
        }

        protected virtual void OnAwake() { }
        protected virtual void OnStart() { }
        protected virtual void OnUpdate() { }
        protected virtual void OnLateUpdate() { }
        protected virtual void OnDestroy() { }

        public void Awake()
        {
            OnAwake();
            foreach (var item in children)
            {
                item.Awake();
            }
        }

        public void Start()
        {
            OnStart();
            foreach (var item in children)
            {
                item.Start();
            }
        }

        public void Update()
        {
            if (!Active)
                return;
            OnUpdate();
            foreach(var item in children)
            {
                if(item.Active)
                    item.Update();
            }
        }

        public void LateUpdate()
        {
            if (!Active)
                return;
            OnLateUpdate();
            foreach (var item in children)
            {
                if (item.Active)
                    item.LateUpdate();
            }
        }

        public void Destroy()
        {
            OnDestroy();
           foreach(var item in children)
            {
                item.Destroy();
            }
        }

        public T GetComponent<T>() where T : Component
        {
            foreach(var item in children)
            {
                if (item is T)
                    return (T)item;
            }
            return null;
        }

        public T AddComponent<T>() where T : Component,new()
        {
            T t = new T();
            t.Owner = this;
            children.Add(t);
            t.Awake();
            return t;
        }
    }
}

