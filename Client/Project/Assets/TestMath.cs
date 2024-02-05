using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMath : MonoBehaviour
{
    public float width = 2;
    public Transform startPoint;
    public Transform endPoint;
    
    public Transform trans;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnDrawGizmos()
    {
        float height = Vector3.Distance(startPoint.position, endPoint.position);
        Vector3 center = (startPoint.position + endPoint.position) * 0.5f;
        Quaternion look = Quaternion.LookRotation((endPoint.position - startPoint.position).normalized);
        for (int i = 0; i < trans.childCount; i++)
        {
            var t = trans.GetChild(i);
            if (IsInRectangle(width, height, look, center, t.position))
            {
                Gizmos.color = Color.green;
            }
            else
            {
                Gizmos.color = Color.gray;
            }
            Gizmos.DrawSphere(t.position, 0.5f);

        }
        
    }
    
    bool IsInRectangle(float width, float height, Quaternion rotation, Vector3 center, Vector3 point)
    {
        float halfWidth = width * 0.5f;
        float halfHeight = height * 0.5f;
        //左下，左上，右上，右下
        Vector3 p1 =  new Vector3(-halfWidth, 0, -halfHeight);
        Vector3 p2 =  new Vector3(-halfWidth, 0, halfHeight);
        Vector3 p3 = new Vector3(halfWidth, 0, halfHeight);
        Vector3 p4 = new Vector3(halfWidth, 0, -halfHeight);

        point.y = 0;
		
        Vector3[] rectangle = { p1, p2, p3, p4 };

        for (int i = 1; i <= rectangle.Length; i++)
        {
            Vector3 lastP = center + rotation * rectangle[i-1];
            Vector3 curP = center + rotation * rectangle[i == rectangle.Length ? 0 : i];
            Gizmos.DrawLine(lastP, curP);
            if (Vector3.Dot((curP - lastP).normalized, (point - lastP).normalized) < 0)
            {
                return false;
            }
        }

        return true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
