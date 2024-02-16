using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRectangle : MonoBehaviour
{
    public float width;
	
    public float height;

    public Vector3 offset;

    public GameObject target;

    public List<Vector3> targets = new();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private float _timer;
    // Update is called once per frame
    
    void IsInRectangle(Vector3 point)
    {
        Vector3 center = transform.position + transform.localRotation * offset;

        float halfWidth = width * 0.5f;
        float halfHeight = height * 0.5f;
        //左下，左上，右上，右下
        Vector3 p1 =  new Vector3(-halfWidth, 0, -halfHeight);
        Vector3 p2 =  new Vector3(-halfWidth, 0, halfHeight);
        Vector3 p3 = new Vector3(halfWidth, 0, halfHeight);
        Vector3 p4 = new Vector3(halfWidth, 0, -halfHeight);

        var p = point;
        
        point.y = 0;
        Vector3 targetOffset = point - transform.position;
		
        Vector3[] rectangle = { p1, p2, p3, p4 };

        bool isIn = true;
        
        for (int i = 1; i <= rectangle.Length; i++)
        {
            Vector3 lastP = center +transform.localRotation * rectangle[i-1];
            Vector3 curP = center + transform.localRotation * rectangle[i == rectangle.Length ? 0 : i];
            if (Vector3.Dot((curP - lastP).normalized, (point - lastP).normalized) < 0)
            {
                isIn = false;
            }
            Gizmos.DrawLine(lastP, curP);
        }

        Gizmos.color = isIn ? Color.green : Color.gray;

        Gizmos.DrawSphere(p, 0.5f);
    }

    private void OnDrawGizmos()
    {
        if(target != null)
            IsInRectangle(target.transform.position);

        foreach (var point in targets)
        {
            IsInRectangle(point);
        }
    }
}
