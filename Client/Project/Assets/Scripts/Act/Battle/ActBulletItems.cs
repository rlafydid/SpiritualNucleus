using Battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Act.Simulator
{
    class BulletItem
    {
        public GameObject gameObject;
        public Transform transform;

        public ActorController target;

        public Vector3 launchPos;
        public Quaternion rotation;

        public Vector3 targetOffset;

        public float timer;

        public Vector3 direction;
        public float lifeTime = 10;
        public float speed = 1;

        protected Vector3 targetPos { get => target.Position + targetOffset; }

        public bool isDestroyed { get => gameObject == null; }

        protected void SetLocation(BLocation location)
        {
            transform.rotation = location.rotation;
            transform.position = location.position;
        }

        public void Setup(GameObject entity)
        {
            gameObject = entity;
            transform = entity.transform;
            transform.localScale = Vector3.one;
            transform.position = launchPos;
            transform.rotation = rotation;
            gameObject.SetActive(true);
        }

        public virtual void Start() { }
        public virtual void Update() { }

        public void Destroy()
        {
            GameObject.Destroy(gameObject);
            gameObject = null;
            transform = null;
        }
    }

    class TraceBulletItem : BulletItem
    {
        public bool turnToTargetNow;

        public override void Update()
        {
            var trans = gameObject.transform;
            BCalculateItem item = new BCalculateItem()
            {
                self = new BLocation(trans.position, trans.rotation),
                deltaTime = Time.deltaTime,
                speed = speed,
                target = targetPos
            };
            BLocation result = BulletMath.TickForTrace(item, turnToTargetNow);
            SetLocation(result);

            CheckCollision();
        }

        void CheckCollision()
        {
            var pos = transform.position;
            var tPos = targetPos;
            pos.y = 0;
            tPos.y = 0;
            if (Vector3.Distance(pos, tPos) <= Time.deltaTime * speed)
            {
                GameObject.Destroy(gameObject);
                gameObject = null;
                target.Hit();
                return;
            }
        }
    }

    class VectorBulletItem : BulletItem
    {
        public override void Update()
        {
            base.Update();
            BCalculateItem item = new BCalculateItem()
            {
                self = new BLocation(transform.position, transform.rotation),
                deltaTime = Time.deltaTime,
                speed = speed,
            };
            transform.position = BulletMath.TickForLine(item);

            CheckCollision();
        }

        HashSet<ActorController> targetsHash = new HashSet<ActorController>();
        void CheckCollision()
        {
            var targets = BattleFacade.GetTargets();
            foreach (var item in targets)
            {
                if (targetsHash.Contains(item))
                    continue;

                var pos = transform.position;
                var tPos = item.Position;
                pos.y = 0;
                tPos.y = 0;
                if (Vector3.Distance(pos, tPos) < 0.5f)
                {
                    item.Hit();
                    targetsHash.Add(item);
                }
            }
        }
    }

    class MissileBulletItem : BulletItem
    {
        public float gravity;
        public float upAcceleration;

        public override void Update()
        {
            base.Update();
            BCalculateItem item = new BCalculateItem()
            {
                self = new BLocation(transform.position, transform.rotation),
                deltaTime = Time.deltaTime,
                speed = speed,
            };
            BLocation result = BulletMath.TickForMissile(item, direction, upAcceleration, ref gravity);
            SetLocation(result);

            CheckCollision();
        }

        void CheckCollision()
        {
            var pos = transform.position;
            var tPos = targetPos;
            pos.y = 0;
            tPos.y = 0;
            if (Vector3.Distance(pos, tPos) <= Time.deltaTime * speed)
            {
                GameObject.Destroy(gameObject);
                gameObject = null;
                target.Hit();
                return;
            }
        }
    }

    class ChainBulletItem : BulletItem
    {
        public GameObject shooterPoint;
        public GameObject targetPoint;

        public override void Start()
        {
            base.Start();
            target.Hit();
        }

        public override void Update()
        {
            base.Update();
            //var nextChainBehaviour = gameObject.GetComponentInChildren<ChainBehaviour>();
            //if (nextChainBehaviour == null)
            //{
            //    nextChainBehaviour = gameObject.AddComponent<ChainBehaviour>();
            //}

            //nextChainBehaviour.SetStartPos(shooterPoint);
            //nextChainBehaviour.SetEndPos(targetPoint);
        }


    }
}
