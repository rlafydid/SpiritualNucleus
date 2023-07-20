using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Act
{
    public class ActEntity : Component
    {
        public virtual Vector3 Position { get; set; }
        public virtual Quaternion Rotation { get; set; }

        public virtual Vector3 LocalPosition { get; set; }
        public virtual Quaternion LocalRotation { get; set; }

        public virtual bool IsLoaded { get; set; }

        /// <summary>
        /// 是否是有效的
        /// </summary>
        public bool IsValid
        {
            get
            {
                return this.IsLoaded && !this.IsDestroyed;
            }
        }

        public virtual bool IsDestroyed { get; set; }

        public string ResName { get; set; }

        public bool IsActive = true;

        public Action<ActEntity> LoadComplete;



        public virtual void Load()
        {
            IsLoaded = false;
            Facade.Externals.InstantiateAsync(ResName, Loaded, true);
        }

        protected void Loaded(UnityEngine.Object asset)
        {
            IsLoaded = true;
            GameObject obj = asset as GameObject;
            OnLoaded(obj);
            if (IsDestroyed)
            {
                Destroy();
                return;
            }
            obj.SetActive(IsActive);
            LoadComplete?.Invoke(this);
        }

        protected virtual void OnLoaded(GameObject obj)
        {

        }

        public virtual void SetActive(bool isActive)
        {
            IsActive = isActive;
        }

        public virtual T GetMonoComponent<T>() where T : UnityEngine.Component
        {
            return null;
        }

        public virtual T GetMonoComponentInChildren<T>() where T : UnityEngine.Component
        {
            return null;
        }

        public virtual GameObject GetAttachedPoint(string pointName)
        {
            return null;
        }

        public virtual GameObject GetBone(string boneName)
        {
            return null;
        }

        public virtual void Destroy()
        {
            IsDestroyed = true;
        }
    }

    public class Component
    {
        public Component Owner;
        public List<Component> compList = new List<Component>();

        protected virtual void Start() { }

        public T AddComponent<T>() where T : Component, new()
        {
            T t = new T();
            t.Owner = this;
            t.Start();
            compList.Add(t);
            return t;
        }

        public T GetComponent<T>() where T : Component
        {
            Component comp = compList.Find(d => d.GetType() == typeof(T));
            return comp != null ? (T)comp : null;
        }
    }

    public class GameObjectEntity : ActEntity
    {
        public override Vector3 Position
        {
            get
            {
                return transform.position;
            }
            set
            {
                transform.position = value;
            }
        }
        public override Quaternion Rotation { get => transform.rotation; set => transform.rotation = value; }

        public override Vector3 LocalPosition { get => transform.localPosition; set => transform.localPosition = value; }
        public override Quaternion LocalRotation { get => transform.localRotation; set => transform.localRotation = value; }


        public GameObject GameObject { get => gameObject; }

        GameObject gameObject;
        Transform transform;

        public GameObjectEntity()
        {
        }

        public GameObjectEntity(GameObject obj)
        {
            Loaded(obj);
        }

        protected override void OnLoaded(GameObject obj)
        {
            base.OnLoaded(obj);
            this.gameObject = obj;
            this.transform = obj.transform;
        }

        public override T GetMonoComponent<T>()
        {
            return gameObject.GetComponent<T>();
        }

        public override T GetMonoComponentInChildren<T>()
        {
            return gameObject.GetComponentInChildren<T>();
        }

        public override GameObject GetAttachedPoint(string pointName)
        {
            //ReferenceCollector rc = gameObject.GetComponentInChildren<ReferenceCollector>();
            //if (rc != null)
            //{
            //    GameObject go = rc.Get<GameObject>(pointName);
            //    if (go != null)
            //    {
            //        return go;
            //    }
            //}
            GameObject bone = GetBone(pointName);
            if (bone != null)
                return bone;

            return GameObject;
        }

        public override void SetActive(bool isActive)
        {
            base.SetActive(isActive);
            if (!IsLoaded)
                return;
            gameObject.SetActive(isActive);
        }

        public override GameObject GetBone(string boneName)
        {
            return DeepFind(this.gameObject.transform);

            GameObject DeepFind(Transform trans)
            {
                for (int i = 0; i < trans.childCount; i++)
                {
                    Transform t = trans.GetChild(i);
                    if (t.name == boneName)
                        return t.gameObject;
                    GameObject bone = DeepFind(t);
                    if (bone != null)
                        return bone;
                }
                return null;
            }
        }

        public override void Destroy()
        {
            base.Destroy();
            if (!IsLoaded)
                return;
            //Facade.Externals.ReleaseInstance(gameObject);
            GameObject.Destroy(gameObject);
        }
    }

}


