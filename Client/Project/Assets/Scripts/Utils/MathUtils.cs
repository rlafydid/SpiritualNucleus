using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathUtils
{
    public static bool IsInRectangle(float width, float height, Quaternion rotation, Vector3 center, Vector3 point)
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
            if (Vector3.Dot((curP - lastP).normalized, (point - lastP).normalized) < 0)
            {
                return false;
            }
        }

        return true;
    }
}
