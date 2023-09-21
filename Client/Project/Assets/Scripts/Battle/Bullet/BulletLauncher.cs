using System;
using System.Collections;
using System.Collections.Generic;
using LKEngine;
using UnityEngine;

namespace Battle
{
    public class BulletLauncher
    {
        public BulletData bulletData;

        public virtual BulletItem CreateItem() { return new BulletItem(); }

        public Action<BulletItem> CreateFinished;

        public virtual void Start() { }
    }

    public class VectorBulletLauncher : BulletLauncher
    {
        public VectorBulletData vectorBulletData;

        public VectorBulletLauncher(VectorBulletData data)
        {
            this.vectorBulletData = data;
        }

        public override void Start()
        {
            int count = vectorBulletData.dispersionCount;
            float angle = vectorBulletData.angle;

            float minAngle = 0;
            if (count % 2 == 0)
                minAngle = angle * count * 0.5f - angle * 0.5f;
            else
                minAngle = angle * (count - 1) * 0.5f;

            minAngle = -minAngle;

            var shooter = Facade.Battle.GetActor(vectorBulletData.shooter);
            for (int i = 0; i < vectorBulletData.dispersionCount; i++, minAngle += angle)
            {
                Entity entity = SceneManager.Instance.AddEntity(vectorBulletData.res, typeof(ColliderComponent));
                entity.Load();
                entity.Position = shooter.Position + vectorBulletData.offset;

                Vector3 direction = Quaternion.Euler(0, minAngle, 0) * (shooter.Entity.Forward);

                VectorBulletItem item = new VectorBulletItem(vectorBulletData)
                {
                    direction = direction,
                    Entity = entity
                };
                CreateFinished?.Invoke(item);
            }
        }
    }
}