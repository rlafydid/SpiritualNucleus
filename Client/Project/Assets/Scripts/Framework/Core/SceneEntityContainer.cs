using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LKEngine
{
    public class SceneEntityContainer : Component
    {
        public List<Entity> entities = new List<Entity>();

        public Entity GetEntity(long id)
        {
            return entities.Find(d => d.Id == id);
        }

        public Entity AddEntity(Entity entity)
        {
            entities.Add(entity);
            return entity;
        }

        public void RemoveEntity(Entity entity)
        {
            entities.Remove(entity);
        }

        protected override void OnUpdate()
        {
            foreach(var item in entities)
            {
                item.Update();
            }
        }
    }
}
