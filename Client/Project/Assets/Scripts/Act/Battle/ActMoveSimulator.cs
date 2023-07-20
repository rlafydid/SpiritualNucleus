using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Act.Simulator
{
    class ActMoveSimulator : ActSimulator
    {
        bool isMoveHero = true;

        public override void Start()
        {
            base.Start();
            ActGizmosViewer.Instance.Add(OnDrawGizmos);
        }
        public override void Update()
        {
            base.Update();
            UpdateMove();
            UpdateTargetsCenter();
            if (Input.GetKeyDown(KeyCode.Q))
            {
                GameObject moveTarget = GetMainCharactor().Entity;
                if (moveTarget == null)
                    return;

                moveTarget.transform.position = moveTarget.transform.position + moveTarget.transform.forward * 2;
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                GameObject moveTarget = GetMainCharactor().Entity;
                if (moveTarget == null)
                    return;

                moveTarget.transform.LookAt(BattleFacade.GetTargets()[0].Position);
            }
        }

        void UpdateMove()
        {
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            GameObject moveTarget = GetMainCharactor().Entity;
            if (moveTarget == null)
                return;

            Vector3 toPos = moveTarget.transform.position + new Vector3(x, 0, z) * Time.deltaTime;
            moveTarget.transform.LookAt(toPos);
            moveTarget.transform.position = toPos;
        }

        Vector3 center;

        void UpdateTargetsCenter()
        {
            if (Input.GetMouseButtonDown(1))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                Vector3 o = ray.GetPoint(1);
                float t = Vector3.Dot((Vector3.zero - o), Vector3.up) / Vector3.Dot(ray.direction, Vector3.up);
                Vector3 point = o + ray.direction * t;

                foreach (var item in owner.GetTargets())
                {
                    item.ChangeMove(point);
                }
                center = point;
            }
        }

        public override void OnDrawGizmos()
        {
            if (center != Vector3.zero)
                Gizmos.DrawSphere(center, 0.3f);
        }

        float curRange = ConstParams.moveRange;
        float curSpeed = ConstParams.moveSpeed;
        public override void OnGUI()
        {
            Rect rect = new Rect(100, 10, 80, 30);
            GUI.Label(rect, "移动速度:");
            rect.x += rect.width;
            float speed = GUI.HorizontalSlider(rect, curSpeed, 0, 10);
            if (speed != curSpeed)
            {
                curSpeed = speed;
                foreach (var item in owner.GetTargets())
                {
                    item.moveComp.speed = speed;
                }
            }

            rect = new Rect(100, rect.height + 3, 80, 30);
            GUI.Label(rect, "移动范围:");
            rect.x += rect.width;
            float range = GUI.HorizontalSlider(rect, curRange, 0, 10);
            if (range != curRange)
            {
                curRange = range;
                foreach (var item in owner.GetTargets())
                {
                    item.moveComp.range = range;
                }
            }
        }
    }
}


