using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TwoDimensional
{
// OBB.cs
public class OBB : MonoBehaviour
{
    public bool enableDebug;
    public int debug_axisIndex;
    int mDebugInternalAxisIndex;
    public Vector2 size;
    public Color gizmosColor = Color.white;

    Vector2 P0 { get { return transform.localToWorldMatrix.MultiplyPoint3x4(-size * 0.5f); } }
    Vector2 P1 { get { return transform.localToWorldMatrix.MultiplyPoint3x4(new Vector3(size.x * 0.5f, -size.y * 0.5f, 0)); } }
    Vector2 P2 { get { return transform.localToWorldMatrix.MultiplyPoint3x4(size * 0.5f); } }
    Vector2 P3 { get { return transform.localToWorldMatrix.MultiplyPoint3x4(new Vector3(-size.x * 0.5f, size.y * 0.5f, 0)); } }
    Vector2 axis1, axis2, axis3, axis4;

    // 较参考博文添加以下变量，用来缓存向量减少gc
    Vector3 xProject_P0;
    Vector3 xProject_P1;
    Vector3 xProject_P2;
    Vector3 xProject_P3;

    Vector3 yProject_P0;
    Vector3 yProject_P1;
    Vector3 yProject_P2;
    Vector3 yProject_P3;

    public bool Intersects(OBB other)
    {
        axis1 = (P1 - P0).normalized;
        axis2 = (P3 - P0).normalized;
        axis3 = (other.P1 - other.P0).normalized;
        axis4 = (other.P3 - other.P0).normalized;
        mDebugInternalAxisIndex = 0;
        bool isNotIntersect = false;
        isNotIntersect |= ProjectionIsNotIntersect(this, other, axis1);
        isNotIntersect |= ProjectionIsNotIntersect(this, other, axis2);
        isNotIntersect |= ProjectionIsNotIntersect(this, other, axis3);
        isNotIntersect |= ProjectionIsNotIntersect(this, other, axis4);
        return isNotIntersect ? false : true;
    }
    bool ProjectionIsNotIntersect(OBB x, OBB y, Vector2 axis)
    {
        xProject_P0 = Vector3.Project(x.P0, axis);
        xProject_P1 = Vector3.Project(x.P1, axis);
        xProject_P2 = Vector3.Project(x.P2, axis);
        xProject_P3 = Vector3.Project(x.P3, axis);
        float x_p0 = xProject_P0.magnitude * Mathf.Sign(Vector3.Dot(xProject_P0, axis));
        float x_p1 = xProject_P1.magnitude * Mathf.Sign(Vector3.Dot(xProject_P1, axis));
        float x_p2 = xProject_P2.magnitude * Mathf.Sign(Vector3.Dot(xProject_P2, axis));
        float x_p3 = xProject_P3.magnitude * Mathf.Sign(Vector3.Dot(xProject_P3, axis));

        yProject_P0 = Vector3.Project(y.P0, axis);
        yProject_P1 = Vector3.Project(y.P1, axis);
        yProject_P2 = Vector3.Project(y.P2, axis);
        yProject_P3 = Vector3.Project(y.P3, axis);

        float y_p0 = yProject_P0.magnitude * Mathf.Sign(Vector3.Dot(yProject_P0, axis));
        float y_p1 = yProject_P1.magnitude * Mathf.Sign(Vector3.Dot(yProject_P1, axis));
        float y_p2 = yProject_P2.magnitude * Mathf.Sign(Vector3.Dot(yProject_P2, axis));
        float y_p3 = yProject_P3.magnitude * Mathf.Sign(Vector3.Dot(yProject_P3, axis));

        float xMin = Mathf.Min(x_p0, x_p1, x_p2, x_p3);
        float xMax = Mathf.Max(x_p0, x_p1, x_p2, x_p3);
        float yMin = Mathf.Min(y_p0, y_p1, y_p2, y_p3);
        float yMax = Mathf.Max(y_p0, y_p1, y_p2, y_p3);

        if (enableDebug)
        {
            if (debug_axisIndex == mDebugInternalAxisIndex)
            {
                Debug.DrawRay(Vector3.Project(x.P0, axis), Vector3.one * 0.1f);
                Debug.DrawRay(Vector3.Project(x.P2, axis), Vector3.one * 0.1f);
                Debug.DrawRay(Vector3.Project(y.P0, axis), Vector3.one * 0.1f, Color.white * 0.9f);
                Debug.DrawRay(Vector3.Project(y.P2, axis), Vector3.one * 0.1f, Color.white * 0.9f);
                Debug.DrawRay(Vector3.zero, Vector3.one * 0.1f, Color.black);
                Debug.DrawRay(Vector3.zero, axis, Color.yellow);
                Debug.DrawRay(xMin * Vector3.right, Vector3.one * 0.1f, Color.blue);
                Debug.DrawRay(xMax * Vector3.right, Vector3.one * 0.1f, Color.cyan);
                Debug.DrawRay(yMin * Vector3.right, Vector3.one * 0.1f, Color.red * 0.5f);
                Debug.DrawRay(yMax * Vector3.right, Vector3.one * 0.1f, Color.red * 0.5f);
                Debug.Log("(yMin >= xMin && yMin <= xMax): " + (yMin >= xMin && yMin <= xMax) + " frame count: " + Time.frameCount); Debug.Log("(yMax >= xMin && yMax <= xMax): " + (yMax >= xMin && yMax <= xMax) + " frame count: " + Time.frameCount); Debug.Log("(xMin >= yMin && xMin <= yMax): " + (xMin >= yMin && xMin <= yMax) + " frame count: " + Time.frameCount); Debug.Log("(xMax >= yMin && xMax <= yMax): " + (xMax >= yMin && xMax <= yMax) + " frame count: " + Time.frameCount);
            }
            mDebugInternalAxisIndex++;
        }
        if (yMin >= xMin && yMin <= xMax) return false;
        if (yMax >= xMin && yMax <= xMax) return false;
        // 此处只需做两次判断即可，参考博文做了四次判断 // if (xMin >= yMin && xMin <= yMax) return false; // if (xMax >= yMin && xMax <= yMax) return false;
        return true;
    }
    void OnDrawGizmos()
    {
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = gizmosColor;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(size.x, size.y, 1f));
    }
}
}
