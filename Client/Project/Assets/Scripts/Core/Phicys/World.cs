using System;
using System.Collections;
using System.Collections.Generic;
using LKEngine;
using UnityEngine;

namespace Facade
{
    public class World
    {
        public static Action<Collision.BaseCollider> Add;
        public static Action<Collision.BaseCollider> Remove;
    }
}

namespace Collision
{
    public class BaseCollider
    {
        public Entity Entity { get; set; }
        public OBB obb;
        public HashSet<BaseCollider> triggerd = new HashSet<BaseCollider>();
        public Action<Entity> onTriggerEnter;
        public Action<Entity> onTriggerExit;

        public BaseCollider(Entity entity)
        {
            Entity = entity;
            obb = new OBB(entity);
        }
    }

    public class World : BaseModule
    {
        public static World Inst = new World();

        public List<BaseCollider> colliders = new List<BaseCollider>();

        public override void Init()
        {
            base.Init();
            Facade.World.Add = Add;
            Facade.World.Remove = Remove;
        }

        public void Add(BaseCollider collider)
        {
            colliders.Add(collider);
        }

        public void Remove(BaseCollider collider)
        {
            colliders.Remove(collider);
        }

        public override void Update()
        {
            base.Update();

            for (int i = 0; i < colliders.Count; i++)
            {
                var collider = colliders[i];
                foreach (var other in colliders)
                {
                    if (collider == other)
                        continue;

                    if (collider.obb.Intersects(other.obb))
                    {
                        if (!collider.triggerd.Contains(other))
                        {
                            collider.triggerd.Add(other);
                            collider.onTriggerEnter?.Invoke(other.Entity);
                        }
                    }
                    else
                    {
                        if (collider.triggerd.Contains(other))
                        {
                            collider.triggerd.Remove(other);
                            collider.onTriggerExit?.Invoke(other.Entity);
                        }

                    }
                }
            }
        }
    }

}