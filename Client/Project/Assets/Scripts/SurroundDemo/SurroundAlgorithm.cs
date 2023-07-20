/*================================
 * 类名称: 包围算法 
 * 类描述:
 * 目的:
 * 创建人: Loong
 * 创建时间: 2021.11.18
 * 修改人:
 * 修改时间:
 * 版本: @version 1.0
==================================*/

using System;
using System.Collections;
using System.Collections.Generic;
using LKEngine;
using UnityEngine;

public class SurroundAlgorithm
{
    static SurroundAlgorithm _inst;
    public static SurroundAlgorithm Inst
    {
        get
        {
            if (_inst == null)
                _inst = new SurroundAlgorithm();

            return _inst;
        }
    }

    struct SurroundData
    {
        public float SourceRadius;
        public float Radius;
        public Entity Entity;
        public float Size;
        public Vector3 SurroundPos;
        public Vector3 SurroundTarget;
        public string SurroundHashKey;

        public Vector3 TransitPos; //中转点
    }

    /// <summary>
    /// 包围角度范围
    /// </summary>
    public int SurroundAngle = 360;

    /// <summary>
    /// 到达距离（离计算的目标点小于此距离则算到达目标位置）
    /// </summary>
    public float ReachMinDistance = 0.5f;

    /// <summary>
    /// 存储围绕点
    /// </summary>
    HashSet<string> surroundedHash = new HashSet<string>();

    /// <summary>
    /// 包围的实体和信息
    /// </summary>
    Dictionary<Entity, SurroundData> entityTable = new Dictionary<Entity, SurroundData>();

    float deg2Rad = 3.14f / 180;
    float rad2Deg = 180 / 3.14f;

    const float extendRadius = 0.1f; //扩大半径范围

    /// <summary>
    /// 返回包围者的目标位置
    /// </summary>
    /// <param name="entity"> 包围者 </param>
    /// <param name="surroundTarget"> 包围目标 </param>
    /// <param name="radius"> 半径 </param>
    /// <param name="modelSize"> 模型尺寸 </param>
    public Vector3 FindPos(Entity entity, Vector3 surroundTarget, float radius, float modelSize = 1)
    {
        return FindPos(entity, surroundTarget, radius, modelSize, 0);
    }

    Vector3 FindPos(Entity entity, Vector3 surroundTarget, float radius, float modelSize, float sourceRadius)
    {
        SurroundData data = GetSurroundData(entity);
        if(!data.Equals(default(SurroundData)))
        {
            //目标点是否一致或相似
            if(Vector3.Distance(data.SurroundTarget, surroundTarget) < 0.1f)
            {
                Vector3 sPos = data.SurroundTarget + data.SurroundPos;
                if (IsReachSurroundPoint(entity.Position, sPos, data.SurroundTarget, radius + extendRadius))
                    return surroundTarget;
                else
                {
                    if(IsReachTransitPoint(data))
                    {
                        //获取下一个中转点
                        ToNextTransitPoint(data); 
                    }
                    return data.SurroundTarget  + data.TransitPos;
                }
            }
            else
            {
                //移除实体信息，下次调用则重新获取位置
                Remove(entity);
            }
        }

        data = new SurroundData()
        {
            SourceRadius = sourceRadius > 0 ? sourceRadius : radius,
            Radius = radius,
            Entity = entity,
            Size = modelSize,
            SurroundTarget = surroundTarget
        };

        Vector3 pos = entity.Position;

        //从Z正方向到实体的欧拉角
        float angle = GetEntityAngleFromForward(pos, surroundTarget);

        /*根据模型尺寸计算角度间隔*/
        modelSize *= 0.5f;
        int perAngle = (int)(Math.Asin(modelSize / data.Radius) * rad2Deg) * 2; //角度间隔

        Vector3 surroundPos = surroundTarget + BFSearchPos(angle, data, perAngle);

        if(IsReachSurroundPoint(pos, surroundPos, surroundTarget, radius))
            return surroundTarget;
        else
            return surroundPos;
    }

    bool IsReachSurroundPoint(Vector3 entityPos, SurroundData data)
    {
        return IsReachSurroundPoint(entityPos, data.SurroundTarget + data.SurroundPos, data.SurroundTarget, data.Radius);
    }
    /// <summary>
    /// 是否到达包围点
    /// </summary>
    /// <param name="entityPos"></param>
    /// <param name="surroundPos"></param>
    /// <param name="targetPos"></param>
    /// <param name="radius"></param>
    /// <returns></returns>
    bool IsReachSurroundPoint(Vector3 entityPos, Vector3 surroundPos, Vector3 targetPos, float radius)
    {
        if (Vector3.Distance(entityPos, surroundPos) < ReachMinDistance || Vector3.Distance(entityPos, targetPos) < radius)
            return true;
        else
            return false;
    }

    bool IsReachTransitPoint(SurroundData data)
    {
        return Distance(data.Entity.Position, data.SurroundTarget + data.TransitPos) < ReachMinDistance;
    }

    void ToNextTransitPoint(SurroundData data)
    {
        AddTrasitPos(ref data);
        RefreshData(data);
    }

    void RefreshData(SurroundData data)
    {
        if (entityTable.ContainsKey(data.Entity))
        {
            entityTable[data.Entity] = data;
        }
    }

    /// <summary>
    /// 计算从目标点Z方向到实体的欧拉角
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    float GetEntityAngleFromForward(Vector3 pos, Vector3 target)
    {
        Vector3 dir = (pos - target).normalized;
        float angle = GetAngle(dir, Vector3.forward);
        angle *= dir.x > 0 ? 1 : -1;
        return angle;
    }

    /// <summary>
    /// 核心算法函数
    ///  BFS(Breadth First Search) 广度优先搜索 在圆圈适合的位置
    /// </summary>
    /// <param name="curAngle"></param>
    /// <param name="data"></param>
    /// <param name="angleInterval"> 每围攻者的角度间隔 </param>
    /// <returns></returns>
    Vector3 BFSearchPos(float curAngle, SurroundData data, int angleInterval)
    {
        if (angleInterval <= 0)
        {
            return Vector3.zero;
        }
        int angle = RoundToInt(curAngle / angleInterval) * angleInterval;
        float r = data.Radius;
        for (int i = 0; i <= SurroundAngle/2 + angleInterval; i += angleInterval)
        {
            int val = Math.Min(i, SurroundAngle/2);

            int surroundAngle = angle + val;
            bool isSuccess = CalculateSurroundPos(surroundAngle, data, out Vector3 pos);

            if (isSuccess)
            {
                data.SurroundPos = pos;
                AddSurroundData(data, surroundAngle);
                return pos;
            }

            surroundAngle = angle - val;
            isSuccess = CalculateSurroundPos(surroundAngle, data, out pos);
            if (isSuccess)
            {
                data.SurroundPos = pos;
                AddSurroundData(data, surroundAngle);
                return pos;
            }
        }

        //当围满时 角度范围÷2 继续查找
        return BFSearchPos(angle, data, angleInterval >> 1);
    }


    /// <summary>
    /// 移除（注：移除后其他实体才能加入此位置）
    /// </summary>
    /// <param name="key"></param>
    public void Remove(Entity entity)
    {
        if (entityTable.TryGetValue(entity, out SurroundData data))
        {
            if (surroundedHash.Contains(data.SurroundHashKey))
                surroundedHash.Remove(data.SurroundHashKey);
    
            entityTable.Remove(entity);
        }
    }

    /// <summary>
    /// 清理
    /// </summary>
    public void Clear()
    {
        surroundedHash.Clear();
        entityTable.Clear();
    }

    /// <summary>
    /// 检查目标是否移动
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="target"></param>
    public void CheckEntity(Entity entity, Vector3 target)
    {
        SurroundData data = GetSurroundData(entity);
        if (Distance(data.SurroundTarget, target) > 0.1f || !IsReachSurroundPoint(entity.Position, data))
        {
            Remove(entity);
        }
    }

    /// <summary>
    /// 刷新所有怪物占据的位置
    /// </summary>
    /// <param name="targetPos"></param>
    public void RefreshTarget(Entity entity, Vector3 target)
    {
        SurroundData data = GetSurroundData(entity);

        if (data.Equals(default))
            return;

        if (Vector3.Distance(data.SurroundTarget, target) < 0.01f)
            return;

        if (surroundedHash.Count == 0)
            return;

        Entity[] keys = new Entity[entityTable.Keys.Count];
        entityTable.Keys.CopyTo(keys, 0);

        foreach (var key in keys)
        {
            data = entityTable[key];

            if (Vector3.Distance(target, data.Entity.Position) > data.SourceRadius + 0.000001f)
            {
                Remove(data.Entity);
            }
        }

        Dictionary<Entity, SurroundData> tempSurroundedTable = new Dictionary<Entity, SurroundData>(entityTable);
        surroundedHash.Clear();
        entityTable.Clear();
        foreach (var item in tempSurroundedTable)
        {
            data = item.Value;
            data.Radius = RoundToInt(Vector3.Distance(target, data.Entity.Position));
            FindPos(data.Entity, target, data.Radius, data.Size, data.SourceRadius);
        }
    }

    void AddSurroundData(SurroundData data, int angle)
    {
        string key = GetHashKey(data, angle);
        if (!surroundedHash.Contains(key))
        {
            AddTrasitPos(ref data);
            data.SurroundHashKey = key;
            entityTable.Add(data.Entity, data);
            surroundedHash.Add(key);
        }
    }

    /// <summary>
    /// 添加中转点
    /// </summary>
    /// <param name="data"></param>
    void AddTrasitPos(ref SurroundData data)
    {
        Vector3 p1 = data.Entity.Position - data.SurroundTarget;
        Vector3 p2 = data.SurroundPos;

        data.TransitPos = FindNotIntersectPos(p1, p2, data.Radius);
    }

    Vector3 FindNotIntersectPos(Vector3 p1, Vector3 p2, float radius)
    {
        float angle = GetAngle(p1, p2);
        if (angle < 0.5f)
            return p2;

        if (IsIntersect(p1.To2D(), p2.To2D(), Vector3.zero, radius))
        {
            float y = Vector3.Cross(p1, p2).normalized.y;
            angle *= y;

            float r = radius + extendRadius;

            float length = (p1.magnitude + r) * 0.5f;
            length = (float)Math.Min(Math.Sqrt(r * r + r * r), length);
            length = Math.Max(length, r);
            //float length = radius + 0.1f;

            Vector3 centerP = p1.normalized * length;

            p2 = GetRotatePos(centerP, angle * 0.5f);

            return FindNotIntersectPos(p1, p2, radius);
        }
        else
        {
            return p2;
        }    
    }

    /// <summary>
    /// 线段是否和圆相交
    /// </summary>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <param name="c"></param>
    /// <param name="radius"></param>
    /// <returns></returns>
    public bool IsIntersect(Vector2 p1, Vector2 p2, Vector2 c, float radius)
    {
        float A, B, C, d;
        A = p1.y - p2.y;
        B = p2.x - p1.x;
        C = p1.x * p2.y - p2.x * p1.y;
        d = Mathf.Abs(A * c.x + B * c.y + C);
        d /= Mathf.Sqrt(A * A + B * B);

        if(d >= radius)
        {
            return false;
        }
        float angle1 = GetAngle(p2 - p1, c - p1);
        float angle2 = GetAngle(p1 - p2, c - p2);

        if (angle1 <= 90 && angle2 <= 90)
            return true;
        else
            return false;
    }

    bool CalculateSurroundPos(int angle, SurroundData data, out Vector3 pos)
    {
        if(surroundedHash.Contains(GetHashKey(data, angle)))
        {
            pos = Vector3.zero;
            return false;
        }
        pos = GetRotatePos(new Vector3(0, 0, data.Radius + extendRadius), angle);
        return true;
    }

    /// <summary>
    /// 旋转矩阵公式计算
    /// </summary>
    /// <param name="angle"></param>
    /// <param name="p"></param>
    /// <returns></returns>
    Vector3 GetRotatePos(Vector3 p, float angle)
    {
        float cos = (float)Math.Cos(deg2Rad * angle);
        float sin = (float)Math.Sin(deg2Rad * angle);
        Vector3 pos = new Vector3(p.x * cos + sin * p.z, 0, -sin * p.x +  cos * p.z);
        return pos;
    }

    SurroundData GetSurroundData(Entity entity)
    {
        if (entityTable.TryGetValue(entity, out var data))
        {
            return data;
        }
        return default;
    }

    string GetHashKey(SurroundData data, int angle)
    {
        angle %= 360;

        if (angle < 0)
            angle = 360 + angle;

        Vector3 intPos = new Vector3(Mathf.FloorToInt(data.SurroundTarget.x), Mathf.FloorToInt(data.SurroundTarget.y), Mathf.FloorToInt(data.SurroundTarget.z));

        return string.Format("{0}{1}{2}", intPos.ToString(), angle, Mathf.FloorToInt(data.Radius * 10));
    }

    int RoundToInt(float val)
    {
        return Mathf.RoundToInt(val);
    }

    float GetAngle(Vector3 from, Vector3 to)
    {
        return Vector3.Angle(from, to);
    }
    
    float Distance(Vector3 p1, Vector3 p2)
    {
        return Vector3.Distance(p1, p2);
    }
    
}

public static class ExtensionSurround
{
    public static Vector2 To2D(this Vector3 p)
    {
        return new Vector2(p.x, p.z);
    }
}

