using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Act.Simulator
{
    enum EBulletType
    {
        Trace,
        Vector,
        Missile,
        Chain
    }
    class ActBulletSimulator : ActSimulator
    {
        List<BulletItem> bullets = new List<BulletItem>();

        Dictionary<string, GameObject> cachePrefab = new Dictionary<string, GameObject>();

        public void TriggerBulletEvent(ActBulletEvent bulletEvent)
        {
            if (bulletEvent.BulletData == null)
                return;

            string resName = bulletEvent.BulletData.bulletID;
            if (string.IsNullOrEmpty(resName))
                resName = "fx_com_bullet01";

            if (!cachePrefab.TryGetValue(resName, out GameObject prefab))
            {
                prefab = Act.ActUtility.GetAsset<GameObject>(resName);
                cachePrefab.Add(resName, prefab);
            }
            if (prefab != null)
            {
                ActEntity entity = Act.ActEntityFactory.Instantiate(prefab as GameObject);
                LaunchBullet((entity as GameObjectEntity).GameObject, bulletEvent);
            }
        }

        void LaunchBullet(UnityEngine.Object obj, ActBulletEvent bulletEvent)
        {
            GameObject bullet = obj as GameObject;
            bullet.SetActive(false);
            bullet.transform.localRotation = Quaternion.Euler(bulletEvent.BulletData.angleOffset);
            switch (bulletEvent.BulletData)
            {
                case ActTraceBulletData data:
                    LaunchTrace(bullet, data);
                    break;
                case ActVectorBulletData data:
                    LaunchVectorBullet(bullet, data);
                    break;
                case ActMissileBulletData data:
                    LaunchMissileBullet(bullet, data);
                    break;
                case ActChainBulletData data:
                    LaunchChainBullet(bullet, data);
                    break;
            }
        }

        public override void Update()
        {
            for (int i = bullets.Count - 1; i >= 0; i--)
            {
                var item = bullets[i];
                item.timer += Time.deltaTime;

                if (item.timer > item.lifeTime)
                    item.Destroy();

                if (item.isDestroyed)
                {
                    bullets.RemoveAt(i);
                    continue;
                }

                item.Update();
            }
        }


        async void LaunchTrace(GameObject bullet, ActTraceBulletData data)
        {
            var shooter = GetMainCharactor();
            var targets = GetTargets();

            int continuousShootingCount = data.continuousShootingCount;
            int dispersionNum = data.dispersionNum;

            dispersionNum = Mathf.Min(dispersionNum, targets.Count);

            int timeInterval = (int)(data.continuousTimeInterval * 1000);

            for (int i = 0; i < continuousShootingCount; i++)
            {
                if (i > 0)
                    await Task.Delay(timeInterval);

                for (int y = 0; y < dispersionNum; y++)
                {
                    Vector3 fromPos = shooter.Position + shooter.Rotation * data.launchOffset;
                    Quaternion rotation = shooter.Rotation;

                    TraceBulletItem item = new TraceBulletItem();
                    item.launchPos = fromPos;
                    item.rotation = rotation;
                    item.speed = data.speed;
                    item.target = targets[y];
                    item.targetOffset = Vector3.up * 0.5f;

                    var instance = GameObject.Instantiate(bullet);
                    item.Setup(instance);

                    item.direction = instance.transform.forward;

                    AddBullet(item);
                }
            }
        }

        async void LaunchVectorBullet(GameObject bullet, ActVectorBulletData data)
        {
            var shooter = GetMainCharactor();
            var targets = GetTargets();

            float HAngleOffsetCfg = 0;

            int dispersionNum = data.dispersionNum;

            float dispersionAngle = data.BPdispersionAngle;
            float halfAngle = 0;
            if (data.BPdispersionSymmetry)//散射对称
            {
                halfAngle = Mathf.Max(dispersionAngle * (dispersionNum - 1) * .5f, 0);
            }

            int continuousShootingCount = data.continuousShootingCount;
            int timeInterval = (int)(data.continuousTimeInterval * 1000);

            List<List<VectorBulletItem>> itemGroups = new List<List<VectorBulletItem>>();
            for (int i = 0; i < continuousShootingCount; i++)
            {
                var list = new List<VectorBulletItem>();
                itemGroups.Add(list);
                for (int y = 0; y < dispersionNum; y++)
                {
                    Vector3 fromPos = shooter.Position + shooter.Rotation * data.launchOffset;
                    Quaternion rotation = shooter.Rotation;

                    VectorBulletItem item = new VectorBulletItem();
                    item.speed = data.speed;
                    item.launchPos = fromPos;

                    var forwardOrTargerPos = shooter.Entity.transform.forward;
                    item.direction = new Vector3(forwardOrTargerPos.x, 0, forwardOrTargerPos.z);// 子弹方向无视射击者Y轴方向

                    var angleOffset = -halfAngle + y * dispersionAngle;

                    item.direction = Quaternion.AngleAxis(HAngleOffsetCfg, Vector3.right) *
            Quaternion.AngleAxis(angleOffset, Vector3.up) * item.direction;

                    item.rotation = Quaternion.LookRotation(item.direction) * Quaternion.Euler(data.angleOffset);

                    list.Add(item);
                }
            }

            for (int i = 0; i < itemGroups.Count; i++)
            {
                if (i > 0)
                    await Task.Delay(timeInterval);

                for (int y = 0; y < itemGroups[i].Count; y++)
                {
                    VectorBulletItem item = itemGroups[i][y];

                    var instance = GameObject.Instantiate(bullet);
                    item.Setup(instance);

                    AddBullet(item);
                }
            }
        }

        void AddBullet(BulletItem item)
        {
            item.Start();
            bullets.Add(item);
        }

        async void LaunchMissileBullet(GameObject bullet, ActMissileBulletData data)
        {
            var shooter = GetMainCharactor();
            var targets = GetTargets();
            var target = targets[0];
            Vector3 targetPos = target.Position;

            Vector3 shooterPos = shooter.Position + shooter.Rotation * data.launchOffset;

            //计算抛物线参数
            float forwardStartSpeed;
            float upStartSpeed;
            float upAcceleration;

            Vector2 fromPos = new Vector2(shooterPos.x, shooterPos.z);
            Vector2 toPos = new Vector2(targetPos.x, targetPos.z);
            float distance = Vector2.Distance(fromPos, toPos);

            float height = data.height;
            float gAcceleration = data.gAcceleration;

            if (data.type == 1)
            {
                upAcceleration = data.gAcceleration;
                upStartSpeed = Mathf.Sqrt(2 * gAcceleration * (height - shooterPos.y));
                float time = Mathf.Sqrt(2 * (height - shooterPos.y) / gAcceleration) + Mathf.Sqrt(2 * height / gAcceleration);
                forwardStartSpeed = distance / time;
            }
            else //if (type == 2)
            {
                upAcceleration = gAcceleration;
                forwardStartSpeed = data.forwardSpeed;
                float time = distance / forwardStartSpeed;
                upStartSpeed = gAcceleration * time;
            }

            int dispersionNum = 1;

            int continuousShootingCount = data.continuousShootingCount;

            int timeInterval = (int)(data.continuousTimeInterval * 1000);

            for (int i = 0; i < continuousShootingCount; i++)
            {
                if (i > 0)
                    await Task.Delay(timeInterval);

                for (int y = 0; y < dispersionNum; y++)
                {
                    Vector3 bulletFromPos = shooter.Position + shooter.Rotation * data.launchOffset;
                    Quaternion rotation = shooter.Rotation;

                    MissileBulletItem item = new MissileBulletItem();
                    item.launchPos = bulletFromPos;
                    item.rotation = shooter.Rotation;
                    item.speed = forwardStartSpeed;
                    item.direction = (targetPos - bulletFromPos).normalized;
                    item.gravity = upStartSpeed;
                    item.upAcceleration = upAcceleration;
                    item.target = target;
                    item.rotation = Quaternion.LookRotation(item.direction);

                    var instance = GameObject.Instantiate(bullet);
                    item.Setup(instance);

                    AddBullet(item);
                }

            }
        }

        async void LaunchChainBullet(GameObject bullet, ActChainBulletData data)
        {
            var shooter = GetMainCharactor();
            var targets = GetTargets();

            int continuousShootingCount = data.continuousShootingCount;
            int dispersionNum = 1;

            dispersionNum = Mathf.Min(dispersionNum, targets.Count);

            int timeInterval = 0;

            for (int i = 0; i < continuousShootingCount; i++)
            {
                for (int y = 0; y < dispersionNum; y++)
                {
                    Vector3 fromPos = shooter.Position + shooter.Rotation * data.launchOffset;
                    Quaternion rotation = shooter.Rotation;

                    ChainBulletItem item = new ChainBulletItem();
                    item.launchPos = fromPos;
                    item.rotation = shooter.Rotation;

                    item.target = targets[y];
                    //item.targetOffset = targets[y].position + Vector3.up * 0.5f;
                    //item.targetPoint = targets[y].entity.GetComponentInChildren<ReferenceCollector>().Get<GameObject>(data.endAttachedPoint);
                    //item.shooterPoint = shooter.entity.GetComponentInChildren<ReferenceCollector>().Get<GameObject>(data.startAttachedPoint);
                    //item.lifeTime = data.maxLifetime * 0.001f;

                    //var instance = GameObject.Instantiate(bullet);
                    //item.Setup(instance);

                    AddBullet(item);
                }
            }
        }

        public override void Destroy()
        {
            base.Destroy();
            foreach (var item in bullets)
            {
                item.Destroy();
            }
            bullets.Clear();
        }

    }
}


