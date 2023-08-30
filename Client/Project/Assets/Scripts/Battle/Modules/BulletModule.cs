using System;
using System.Collections;
using System.Collections.Generic;
using Battle;
using Collistion;
using LKEngine;
using Collision;
using UnityEngine;
using GraphProcessor;

namespace Facade
{
    public class Bullet
    {
        public static Action<BulletData> LaunchBullet;
        public static Action<BulletItem> DestroyBullet;
    }
}

namespace Battle
{
    public class BulletData
    {
        public string res;
        public float speed;
        public long shooter;
        public Vector3 offset;
        public List<TriggerNode> hitTriggerNode;
        public SkillProcess process;
    }

    public class VectorBulletData : BulletData
    {
        public int dispersionCount;
        public float angle;
        public float distance;
    }

    public class BulletItem
    {
        public Entity Entity { get; set; }
        public bool IsDestroyed { get; private set; } = false;

        public virtual void Start()
        {

        }

        public virtual void Update()
        {

        }

        protected void Destroy()
        {
            IsDestroyed = true;
            SceneManager.Instance.RemoveEntity(Entity);
            Entity = null;
        }
    }

    public class VectorBulletItem : BulletItem
    {
        public Vector3 direction;
        VectorBulletData data;
        BaseCollider collider;
        float distance;

        public VectorBulletItem(VectorBulletData data)
        {
            this.data = data;
        }

        public override void Start()
        {
            base.Start();
            this.Entity.GetComponent<ColliderComponent>().onTriggerEnter = OnTrigerEnter;
        }

        void OnTrigerEnter(Entity entity)
        {
            // Debug.Log($"与 {entity.GameObject.name} 发生碰撞");
            foreach(var node in data.hitTriggerNode)
            {
                (node as BulletHitNode).SetData(this.Entity.Id, entity.Id);
                data.process.ExecuteNode(node);
            }
            
        }

        public override void Update()
        {
            base.Update();
            Vector3 offset = direction * Time.deltaTime * data.speed;
            Vector3 pos = Entity.Position + offset;
            Entity.LookAt(pos);
            Entity.Position = pos;

            distance += offset.magnitude;
            if(distance > data.distance)
            {
                Destroy();
            }
        }
    }
    

    public class BulletModule : BaseBattleModule
    {
        public List<BulletItem> bullets = new List<BulletItem>();

        public override void Init()
        {
            base.Init();
            Facade.Bullet.LaunchBullet = LaunchBullet;
            Facade.Bullet.DestroyBullet = DestroyBullet;
        }

        public override void Update()
        {
            base.Update();
            for (int i = bullets.Count - 1; i >= 0; i--)
            {
                if(bullets[i].IsDestroyed)
                    bullets.RemoveAt(i);
                else
                    bullets[i].Update();
            }
        }

        void LaunchBullet(BulletData data)
        {
            BulletLauncher launcher = CreateBulletLauncher(data);
            launcher.CreateFinished = CreateFinished;
            launcher.Start();
        }

        void CreateFinished(BulletItem item)
        {
            item.Start();
            bullets.Add(item);
        }

        void DestroyBullet(BulletItem item)
        {

        }

        BulletLauncher CreateBulletLauncher(BulletData data)
        {
            switch (data)
            {
                case VectorBulletData vectorData:
                    return new VectorBulletLauncher(vectorData);
            }
            return null;
        }
    }

    
}

